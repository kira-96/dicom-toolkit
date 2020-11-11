using StyletIoC;
using Polly;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SimpleDICOMToolkit.Logging;

namespace SimpleDICOMToolkit.Services
{
    public class UpdateService : IUpdateService
    {
        private const string GITHUB_API = @"https://api.github.com";

        // get latest release
        // GET /repos/{owner}/{repo}/releases/latest
        private const string Request_url = @"/repos/kira-96/dicom-toolkit/releases/latest";

        private const string Download_Dir = "Update";
        private const string Downloading_Suffix = ".downloading";
        private const int Download_trunk_size = 1024;
        private CancellationTokenSource cancellationTokenSource;

        private ManualResetEventSlim manualResetEventSlim;

        private Task downloadTask;

        private string url;

        private readonly ILoggerService loggerService;

        public Version NewVersion { get; private set; }

        public bool IsCheckingForUpdate { get; private set; } = false;

        public bool IsDownloading { get; private set; } = false;

        public event EventHandler DownloadComplete = delegate { };
        public event Action<Version> VersionAvaliable = delegate { };
        public event Action<Exception> CheckForUpdateError = delegate { };
        public event Action<Exception> DownloadError = delegate { };
        public event Action<int> DownloadPercentChanged = delegate { };

        public UpdateService([Inject("filelogger")]ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public async ValueTask CheckForUpdateAsync()
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8),
                }, 
                (ex, time) =>
                {
                    IsCheckingForUpdate = false;
                });

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string response = null;

            try
            {
                response = await policy.ExecuteAsync<string>(async () => {
                    HttpWebRequest req = WebRequest.Create(GITHUB_API + Request_url) as HttpWebRequest;

                    req.Method = "GET";
                    req.Accept = "application/vnd.github.v3+json";
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36";

                    IsCheckingForUpdate = true;

                    var res = await req.GetResponseAsync();

                    string content;

                    using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                    {
                        content = await reader.ReadToEndAsync();
                    }

                    IsCheckingForUpdate = false;

                    return content;
                });
            }
            catch (Exception ex)
            {
                IsCheckingForUpdate = false;
                NewVersion = new Version(0, 0, 0);
                CheckForUpdateError(ex);

                return;
            }
            
            // Newtonsoft.Json
            //JObject jObject = JObject.Parse(response);

            //Version version = Version.Parse((string)jObject["tag_name"]);

            //NewVersion = version;
            //url = (string)jObject["assets"].First["browser_download_url"];

            int index = response.IndexOf("\"tag_name\":");
            int startIndex = response.IndexOf('"', index + 11 /* "\"tag_name\":".Length */) + 1;
            int endIndex = response.IndexOf('"', startIndex);
            NewVersion = Version.Parse(response.Substring(startIndex, endIndex - startIndex));

            index = response.IndexOf("\"browser_download_url\":");
            startIndex = response.IndexOf('"', index + 23 /* "\"browser_download_url\":".Length */) + 1;
            endIndex = response.IndexOf('"', startIndex);
            url = response.Substring(startIndex, endIndex - startIndex);

            VersionAvaliable(NewVersion);
        }

        public void StartDownload()
        {
            if (IsDownloading)
            {
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            manualResetEventSlim = new ManualResetEventSlim(true);

            downloadTask = GetDownloadTask(cancellationTokenSource.Token);

            downloadTask.Start();
            IsDownloading = true;
        }

        public void StopDownload()
        {
            if (IsDownloading &&
                downloadTask != null &&
                manualResetEventSlim != null)
            {
                manualResetEventSlim.Reset();
                IsDownloading = false;
            }
        }

        public void RestartDownload()
        {
            if (!IsDownloading &&
                downloadTask != null &&
                manualResetEventSlim != null)
            {
                manualResetEventSlim.Set();
                IsDownloading = true;
                return;
            }

            if (!IsDownloading &&
                downloadTask == null)
            {
                StartDownload();
            }
        }

        public void CancelDownload()
        {
            if (downloadTask != null && 
                manualResetEventSlim != null &&
                cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();

                if (!manualResetEventSlim.IsSet)
                {
                    manualResetEventSlim.Set();
                }

                IsDownloading = false;
            }
        }

        private string GetDownloadFileName()
        {
            string ext = url.Split('/').Last().Split('.').Last();

            return NewVersion.ToString() + "." + ext;
        }

        /// <summary>
        /// 从文件头得到远程文件的长度
        /// </summary>
        /// <param name="url">download url</param>
        /// <returns>remote file size</returns>
        private long GetRemoteFileSize(string url)
        {
            long length = 0;

            HttpWebRequest req = null;
            HttpWebResponse rsp = null;

            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                rsp = (HttpWebResponse)req.GetResponse();
                if (rsp.StatusCode == HttpStatusCode.OK)
                    length = rsp.ContentLength;
            }
            catch (Exception ex)
            {
                loggerService.Error(ex, "获取远程文件大小失败。");
                DownloadError(ex);
            }
            finally
            {
                if (rsp != null)
                    rsp.Close();
                if (req != null)
                    req.Abort();
            }

            return length;
        }

        private void DownloadOk(string localfile, string downloadingfile)
        {
            try
            {
                FileInfo file = new FileInfo(downloadingfile);
                file.MoveTo(localfile);
            }
            catch (Exception ex)
            {
                loggerService.Error(ex);
            }
            finally
            {
                DownloadPercentChanged(100);
                DownloadComplete(this, new EventArgs());
            }
        }

        private Task GetDownloadTask(CancellationToken token)
        {
            return new Task(() =>
            {
                DirectoryInfo dir = new DirectoryInfo(Download_Dir);
                if (!dir.Exists)
                    Directory.CreateDirectory(Download_Dir);

                string localfile = Download_Dir + "\\" + GetDownloadFileName();

                if (File.Exists(localfile))
                {
                    loggerService.Info("File already exist, download complete.");
                    DownloadComplete(this, new EventArgs());
                    return;
                }

                string downloadingfile = localfile + Downloading_Suffix;
                long remoteFileLength = GetRemoteFileSize(url);

                if (remoteFileLength == 0)
                {
                    DownloadError(new Exception("Remote file size error."));
                    return;
                }

                FileStream fileStream = null;
                long startPosition = 0;

                // 断点续传
                if (File.Exists(downloadingfile))
                {
                    fileStream = File.OpenWrite(downloadingfile);
                    startPosition = fileStream.Length;

                    if (startPosition > remoteFileLength)  // 文件错误，重新下载
                    {
                        fileStream.Close();
                        File.Delete(downloadingfile);
                        fileStream = new FileStream(downloadingfile, FileMode.Create);
                    }
                    else if (startPosition == remoteFileLength)  // 下载完成
                    {
                        fileStream.Close();
                        DownloadOk(localfile, downloadingfile);
                        return;
                    }
                    else  // 继续下载
                    {
                        fileStream.Seek(startPosition, SeekOrigin.Begin);
                    }
                }
                else  // 重新下载
                {
                    fileStream = new FileStream(downloadingfile, FileMode.Create);
                }

                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                HttpWebResponse res = null;

                try
                {
                    if (startPosition > 0)
                    {
                        req.AddRange(startPosition);
                    }

                    res = req.GetResponse() as HttpWebResponse;

                    using Stream stream = res.GetResponseStream();
                    byte[] buffer = new byte[Download_trunk_size];
                    long currentPosition = startPosition;
                    int bytesRead = 0;

                    while (currentPosition <= remoteFileLength)
                    {
                        if (currentPosition == remoteFileLength)
                        {
                            fileStream.Close();
                            DownloadOk(localfile, downloadingfile);
                            break;
                        }

                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            IsDownloading = false;
                            break;
                        }

                        manualResetEventSlim.Wait();

                        bytesRead = stream.Read(buffer, 0, Download_trunk_size);
                        fileStream.Write(buffer, 0, bytesRead);

                        currentPosition += bytesRead;
                        DownloadPercentChanged((int)(currentPosition * 100 / remoteFileLength));
                    }
                }
                catch (Exception ex)
                {
                    loggerService.Error(ex, "下载更新失败。");
                    DownloadError(ex);
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                    if (req != null)
                        req.Abort();
                    if (res != null)
                        res.Close();
                }
            }, token);
        }

        public void Dispose()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            if (manualResetEventSlim != null)
            {
                manualResetEventSlim.Set();
                manualResetEventSlim.Dispose();
            }
        }
    }
}

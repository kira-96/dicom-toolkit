using System;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface IUpdateService : IDisposable
    {
        /// <summary>
        /// invoke on download complete
        /// </summary>
        event EventHandler DownloadComplete;

        /// <summary>
        /// invoke on check update complete
        /// </summary>
        event Action<Version> VersionAvaliable;

        /// <summary>
        /// invoke on check update error
        /// </summary>
        event Action<Exception> CheckForUpdateError;

        /// <summary>
        /// invoke on download error
        /// </summary>
        event Action<Exception> DownloadError;

        /// <summary>
        /// invoke on download process changed
        /// </summary>
        event Action<int> DownloadPercentChanged;

        /// <summary>
        /// version in remote server
        /// </summary>
        Version NewVersion { get; }

        bool IsCheckingForUpdate { get; }

        bool IsDownloading { get; }

        /// <summary>
        /// Start check for update
        /// </summary>
        /// <returns></returns>
        ValueTask CheckForUpdate();

        /// <summary>
        /// Start download new version
        /// </summary>
        void StartDownload();

        /// <summary>
        /// Stop download
        /// </summary>
        void StopDownload();

        /// <summary>
        /// Restart download
        /// </summary>
        void RestartDownload();

        /// <summary>
        /// Candel download
        /// </summary>
        void CancelDownload();
    }
}

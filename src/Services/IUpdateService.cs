using System;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface IUpdateService : IDisposable
    {
        event EventHandler DownloadComplete;

        event Action<Version> VersionAvaliable;
        event Action<Exception> CheckForUpdateError;
        event Action<Exception> DownloadError;
        event Action<int> DownloadPercentChanged;

        Version NewVersion { get; }

        bool IsCheckingForUpdate { get; }

        bool IsDownloading { get; }

        Task CheckForUpdate();

        void StartDownload();

        void StopDownload();

        void RestartDownload();

        void CancelDownload();
    }
}

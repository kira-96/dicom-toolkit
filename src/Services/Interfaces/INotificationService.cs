using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleDICOMToolkit.Controls;

namespace SimpleDICOMToolkit.Services
{
    public interface INotificationService
    {
        bool IsInitialized { get; }

        bool IsRegisted { get; }

        void Initialize(NotifyIcon notifyIcon);

        void ShowNotification(string content, string title, ToolTipIcon icon = ToolTipIcon.Info);

        void Register(object toaster);

        Task ShowToastAsync(string content, TimeSpan duration, ToastType level = ToastType.Info);
    }
}

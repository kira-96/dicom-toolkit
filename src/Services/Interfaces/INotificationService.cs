using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleDICOMToolkit.Controls;

namespace SimpleDICOMToolkit.Services
{
    public interface INotificationService
    {
        bool CanNotify { get; }

        bool CanToast { get; }

        void RegistNotify(NotifyIcon notifyIcon);

        void RegistToast(object toaster);

        void ShowNotification(string content, string title, ToolTipIcon icon = ToolTipIcon.Info);

        Task ShowToastAsync(string content, TimeSpan duration, ToastType level = ToastType.Info);
    }
}

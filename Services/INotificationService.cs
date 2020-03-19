using System.Windows.Forms;

namespace SimpleDICOMToolkit.Services
{
    public interface INotificationService
    {
        bool IsInitialized { get; }

        void Initialize(NotifyIcon notifyIcon);

        void ShowNotification(string content, string title, ToolTipIcon icon = ToolTipIcon.Info);
    }
}

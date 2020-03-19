using System.Windows.Forms;

namespace SimpleDICOMToolkit.Services
{
    public class NotificationService : INotificationService
    {
        public bool IsInitialized { get; private set; }

        private NotifyIcon notifyIcon;

        public void Initialize(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
            IsInitialized = true;
        }

        public void ShowNotification(string content, string title, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (!IsInitialized) return;

            // BalloonTip 在 Win7上显示为气泡通知
            // 在 Win10上显示为 Toast 通知
            notifyIcon.ShowBalloonTip(0, title, content, icon);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleDICOMToolkit.Controls;

namespace SimpleDICOMToolkit.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Can show notification
        /// </summary>
        bool CanNotify { get; }

        /// <summary>
        /// Can show toast
        /// </summary>
        bool CanToast { get; }

        /// <summary>
        /// Regist for notification
        /// </summary>
        /// <param name="notifyIcon"></param>
        void RegistNotify(NotifyIcon notifyIcon);

        /// <summary>
        /// Regist for toast
        /// </summary>
        /// <param name="toaster"></param>
        void RegistToast(object toaster);

        /// <summary>
        /// Show notification
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="title">title</param>
        /// <param name="icon">icon</param>
        void ShowNotification(string content, string title, ToolTipIcon icon = ToolTipIcon.Info);

        /// <summary>
        /// Show toast
        /// </summary>
        /// <param name="content">content text</param>
        /// <param name="duration">toast duration</param>
        /// <param name="level">toast level<see cref="ToastType"/></param>
        /// <returns></returns>
        Task ShowToastAsync(string content, TimeSpan duration, ToastType level = ToastType.Info);
    }
}

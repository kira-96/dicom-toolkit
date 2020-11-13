using System;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
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
        /// <param name="trayIcon"></param>
        void RegistNotify(TaskbarIcon trayIcon);

        /// <summary>
        /// Regist for toast
        /// </summary>
        /// <param name="toaster"></param>
        void RegistToast(Toaster toaster);

        /// <summary>
        /// Show notification
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="title">title</param>
        /// <param name="icon">icon</param>
        void ShowNotification(string content, string title, BalloonIcon icon = BalloonIcon.Info);

        /// <summary>
        /// Show toast
        /// </summary>
        /// <param name="content">content text</param>
        /// <param name="duration">toast duration</param>
        /// <param name="level">toast level<see cref="ToastType"/></param>
        /// <returns></returns>
        ValueTask ShowToastAsync(string content, TimeSpan duration, ToastType level = ToastType.Info);
    }
}

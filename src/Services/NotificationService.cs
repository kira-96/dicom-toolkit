using System;
using System.Threading;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using SimpleDICOMToolkit.Controls;

namespace SimpleDICOMToolkit.Services
{
    public class NotificationService : INotificationService
    {
        public bool CanNotify { get; private set; }

        public bool CanToast { get; private set; }

        private TaskbarIcon trayIcon;

        private Toaster toaster;

        public void RegisterNotify(TaskbarIcon trayIcon)
        {
            this.trayIcon = trayIcon;
            CanNotify = true;
        }

        public void RegisterToast(Toaster toaster)
        {
            this.toaster = toaster;
            CanToast = true;
        }

        public void ShowNotification(string content, string title, BalloonIcon icon = BalloonIcon.Info)
        {
            if (!CanNotify) return;

            // BalloonTip 在 Win7上显示为气泡通知
            // 在 Win10上显示为 Toast 通知
            trayIcon.ShowBalloonTip(title, content, icon);
        }

        public async ValueTask ShowToastAsync(string content, TimeSpan duration, ToastType level = ToastType.Info)
        {
            if (!CanToast)
            {
                return;
            }

            ToastMessage message = new ToastMessage()
            {
                Content = content,
                Level = level
            };

            toaster.SetCurrentValue(Toaster.MessageProperty, message);
            toaster.SetCurrentValue(Toaster.IsActiveProperty, true);

            var durationWaitHandle = new ManualResetEvent(false);
            StartDuration(duration, durationWaitHandle);

            await WaitForCompletionAsync(durationWaitHandle);

            toaster.SetCurrentValue(Toaster.IsActiveProperty, false);
            await Task.Delay(toaster.DeactivateStoryboardDuration);

            toaster.SetCurrentValue(Toaster.MessageProperty, null);

            durationWaitHandle.Dispose();
        }

        private void StartDuration(TimeSpan duration, EventWaitHandle waitHandle)
        {
            var completionTime = DateTime.Now.Add(duration);

            Task.Run(async () =>
            {
                while (true)
                {
                    if (DateTime.Now >= completionTime)
                    {
                        waitHandle.Set();
                        break;
                    }

                    if (waitHandle.WaitOne(TimeSpan.Zero))
                    {
                        break;
                    }

                    await Task.Delay(10);
                }
            });
        }

        private async ValueTask WaitForCompletionAsync(EventWaitHandle waitHandle)
        {
            var durationTask = Task.Run(() =>
            {
                WaitHandle.WaitAll(new[]
                {
                    waitHandle
                });
            });

            await Task.WhenAny(durationTask);

            waitHandle.Set();
        }
    }
}

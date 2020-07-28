using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleDICOMToolkit.Controls;

namespace SimpleDICOMToolkit.Services
{
    public class NotificationService : INotificationService
    {
        public bool IsInitialized { get; private set; }

        public bool IsRegisted { get; private set; }

        private NotifyIcon notifyIcon;

        private Toaster toaster;

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

        public void Register(object toaster)
        {
            if (toaster is Toaster t)
            {
                this.toaster = t;
                IsRegisted = true;
            }
        }

        public async Task ShowToastAsync(string content, TimeSpan duration, ToastType level = ToastType.Info)
        {
            if (!IsRegisted)
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

        private async Task WaitForCompletionAsync(EventWaitHandle waitHandle)
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

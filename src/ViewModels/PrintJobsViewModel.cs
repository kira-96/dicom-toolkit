namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Models;
    using Server;
    using Services;
    using Helpers;

    public class PrintJobsViewModel : Screen, IHandle<ServerMessageItem>, IDisposable
    {
        [Inject]
        private II18nService i18NService;

        [Inject]
        private INotificationService notificationService;

        private readonly IEventAggregator _eventAggregator;

        private bool _isServerStarted = false;

        public bool IsServerStarted
        {
            get => _isServerStarted;
            set => SetAndNotify(ref _isServerStarted, value);
        }

        public PrintJobsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(PrintJobsViewModel));
        }

        public void Handle(ServerMessageItem message)
        {
            if (IsServerStarted)
            {
                PrintServer.Default.CreateServer(message.ServerPort, message.LocalAET);
                _eventAggregator.Publish(new ServerStateItem(true), nameof(PrintJobsViewModel));
                notificationService.ShowNotification(
                    string.Format(i18NService.GetXmlStringByKey("ServerIsRunning"), "Print", SystemHelper.LocalIPAddress, message.ServerPort),
                    message.LocalAET);
            }
            else
            {
                PrintServer.Default.StopServer();
                _eventAggregator.Publish(new ServerStateItem(false), nameof(PrintJobsViewModel));
            }
        }

        public void Explore()
        {
            ProcessHelper.Explore("PrintJobs");
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);

            PrintServer.Default.StopServer();
        }
    }
}

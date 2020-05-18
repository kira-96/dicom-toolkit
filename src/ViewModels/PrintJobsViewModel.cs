namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Models;
    using Server;
    using Services;
    using Utils;

    public class PrintJobsViewModel : Screen, IHandle<ServerMessageItem>, IDisposable
    {
#pragma warning disable IDE0044, 0649
        [Inject]
        private INotificationService notificationService;
#pragma warning restore IDE0044, 0649

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
            if (_isServerStarted)
            {
                PrintServer.Default.CreateServer(message.ServerPort, message.LocalAET);
                notificationService.ShowNotification($"Print server is running at: {SysUtil.LocalIPAddress}:{message.ServerPort}", message.LocalAET);
            }
            else
            {
                PrintServer.Default.StopServer();
            }
        }

        public void Explore()
        {
            ProcessUtil.Explore("PrintJobs");
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);

            PrintServer.Default.StopServer();
        }
    }
}

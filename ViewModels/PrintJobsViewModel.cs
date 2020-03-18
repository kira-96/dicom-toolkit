namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using Models;
    using Server;
    using Logging;

    public class PrintJobsViewModel : Screen, IHandle<ServerMessageItem>
    {
        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

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
            }
            else
            {
                PrintServer.Default.StopServer();
            }
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);

            PrintServer.Default.StopServer();

            base.OnClose();
        }
    }
}

namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using Models;
    using Server;

    public class CStoreReceivedViewModel : Screen, IHandle<CStoreServerItem>
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _isServerStarted = false;

        public bool IsServerStarted
        {
            get => _isServerStarted;
            set => SetAndNotify(ref _isServerStarted, value);
        }

        public CStoreReceivedViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public void Handle(CStoreServerItem message)
        {
            if (_isServerStarted)
            {
                CStoreServer.Default.CreateServer(message.ServerPort, message.LocalAET);
            }
            else
            {
                CStoreServer.Default.StopServer();
            }
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);

            CStoreServer.Default.StopServer();

            base.OnClose();
        }
    }
}

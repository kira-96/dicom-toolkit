namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom.Network;
    using Stylet;
    using StyletIoC;
    using Models;
    using Server;

    public class CStoreReceivedViewModel : Screen, IHandle<CStoreServerItem>
    {
        private readonly IEventAggregator _eventAggregator;

        private IDicomServer _server;

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
                // Start server
                _server = DicomServer.Create<CStoreSCP>(message.ServerPort);
                // set AET
            }
            else
            {
                // Stop server
                _server.Stop();
                _server.Dispose();
            }
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);

            if (_server?.IsListening == true)
            {
                _server.Stop();
                _server.Dispose();
            }

            base.OnClose();
        }
    }
}

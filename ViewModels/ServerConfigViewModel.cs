namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Logging;
    using Models;

    public class ServerConfigViewModel : Screen, IHandle<BusyStateItem>
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        private Action _doRequestAction;

        private string _serverIP = "localhost";
        private string _serverPort = "6104";
        private string _serverAET = "RIS";
        private string _localAET = "LONWINMRI";
        private string _modality = "MR";

        private bool _isModalityEnabled = true;

        public string ServerIP
        {
            get => _serverIP;
            set
            {
                SetAndNotify(ref _serverIP, value);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string ServerPort
        {
            get => _serverPort;
            set
            {
                SetAndNotify(ref _serverPort, value);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string ServerAET
        {
            get => _serverAET;
            set
            {
                SetAndNotify(ref _serverAET, value);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string LocalAET
        {
            get => _localAET;
            set
            {
                SetAndNotify(ref _localAET, value);
                NotifyOfPropertyChange(() => CanDoRequest);
            }
        }

        public string Modality
        {
            get => _modality;
            set => SetAndNotify(ref _modality, value);
        }

        public bool IsModalityEnabled
        {
            get => _isModalityEnabled;
            private set => SetAndNotify(ref _isModalityEnabled, value);
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetAndNotify(ref _isBusy, value))
                {
                    NotifyOfPropertyChange(() => CanDoRequest);
                }
            }
        }

        public ServerConfigViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);
            base.OnClose();
        }

        public bool CanDoRequest =>
            !string.IsNullOrEmpty(ServerIP) &&
            !string.IsNullOrEmpty(ServerPort) &&
            !string.IsNullOrEmpty(ServerAET) &&
            !string.IsNullOrEmpty(LocalAET) &&
            !IsBusy;

        public void DoRequest()
        {
            _doRequestAction?.Invoke();
        }

        public void Init(IScreen parentViewModel)
        {
            if (parentViewModel is WorklistViewModel)
            {
                _doRequestAction = WorklistQueryRequest;
                _eventAggregator.Subscribe(this, nameof(WorklistResultViewModel));
            }
            else if (parentViewModel is PrintViewModel)
            {
                _doRequestAction = PrintRequest;
                _eventAggregator.Subscribe(this, nameof(PrintPreviewViewModel));
                ServerPort = "104";
                ServerAET = "PRINTSCP";
                IsModalityEnabled = false;
            }
            else if (parentViewModel is CStoreViewModel)
            {
                _doRequestAction = CStoreRequest;
                _eventAggregator.Subscribe(this, nameof(CStoreFileListViewModel));
                ServerPort = "104";
                ServerAET = "PACS";
                IsModalityEnabled = false;
            }
            else
            {
                // ...
            }
        }

        private void WorklistQueryRequest()
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            _eventAggregator.Publish(new WorklistRequestItem(_serverIP, port, _serverAET, _localAET, _modality));
        }

        private void PrintRequest()
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            _eventAggregator.Publish(new PrintRequestItem(_serverIP, port, _serverAET, _localAET));
        }

        private void CStoreRequest()
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            _eventAggregator.Publish(new CStoreRequestItem(_serverIP, port, _serverAET, _localAET));
        }

        private int ParseServerPort()
        {
            if (!int.TryParse(_serverPort, out int port))
            {
                _logger.Warn("非法的端口号：{0}，端口号必须为数字。", _serverPort);
                ServerPort = "";
            }

            return port;
        }

        public void Handle(BusyStateItem message)
        {
            IsBusy = message.IsBusy;
        }
    }
}

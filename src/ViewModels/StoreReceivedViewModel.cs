namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using Logging;
    using Models;
    using Server;
    using Services;
    using Helpers;

    public class StoreReceivedViewModel : Screen, IHandle<DicomServiceEvent>, IDisposable
    {
        [Inject]
        private IWindowManager _windowManager;

        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private II18nService i18NService;

        [Inject]
        private IViewModelFactory viewModelFactory;

        [Inject]
        private INotificationService notificationService;

        private readonly IEventAggregator _eventAggregator;

        private bool _isServerStarted = false;

        public bool IsServerStarted
        {
            get => _isServerStarted;
            set => SetAndNotify(ref _isServerStarted, value);
        }

        public BindableCollection<string> StoredFiles { get; } = new BindableCollection<string>();

        public StoreReceivedViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(StoreReceivedViewModel));
        }

        private void OnFilesSaved(IList<string> files)
        {
            _logger.Debug("files received, count: {0}", files.Count);

            int currentDirPathLength = Environment.CurrentDirectory.Length + 1;

            foreach (string file in files)
            {
                string path = file.Remove(0, currentDirPathLength);
                StoredFiles.Add(path);
            }
        }

        public void Handle(DicomServiceEvent message)
        {
            if (IsServerStarted)
            {
                StoreServer.Default.OnFilesSaved += OnFilesSaved;
                StoreServer.Default.CreateServer(message.ServerPort, message.LocalAET);
                _eventAggregator.Publish(new ServerStateEvent(true), nameof(StoreReceivedViewModel));
                notificationService.ShowNotification(
                    string.Format(i18NService.GetXmlStringByKey("ServerIsRunning"), "C-STORE", SystemHelper.LocalIPAddress, message.ServerPort),
                    message.LocalAET);
            }
            else
            {
                StoreServer.Default.OnFilesSaved -= OnFilesSaved;
                StoreServer.Default.StopServer();
                _eventAggregator.Publish(new ServerStateEvent(false), nameof(StoreReceivedViewModel));
            }
        }

        public void ShowReceivedFile(string file)
        {
            var preview = viewModelFactory.GetPreviewImageViewModel();
            preview.Initialize(file);

            _windowManager.ShowDialog(preview, this);
        }

        public void OpenFolder(string file)
        {
            ProcessHelper.Explore(file);
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);

            if (IsServerStarted)
            {
                StoreServer.Default.OnFilesSaved -= OnFilesSaved;
                StoreServer.Default.StopServer();
                IsServerStarted = false;
            }
        }
    }
}

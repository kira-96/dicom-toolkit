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
    using Utils;

    public class CStoreReceivedViewModel : Screen, IHandle<ServerMessageItem>, IDisposable
    {
        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private IWindowManager _windowManager;

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

        public CStoreReceivedViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(CStoreReceivedViewModel));
            CStoreServer.Default.OnFilesSaved += OnFilesSaved;
        }

        private void OnFilesSaved(IList<string> files)
        {
            _logger.Debug("files received, count: {0}", files.Count);

            int currentDirPathLength = System.Environment.CurrentDirectory.Length + 1;

            foreach (string file in files)
            {
                string path = file.Remove(0, currentDirPathLength);
                StoredFiles.Add(path);
            }
        }

        public void Handle(ServerMessageItem message)
        {
            if (IsServerStarted)
            {
                CStoreServer.Default.CreateServer(message.ServerPort, message.LocalAET);
                _eventAggregator.Publish(new ServerStateItem(true), nameof(CStoreReceivedViewModel));
                notificationService.ShowNotification(
                    string.Format(LanguageHelper.GetXmlStringByKey("ServerIsRunning"), "C-STORE", SysUtil.LocalIPAddress, message.ServerPort),
                    message.LocalAET);
            }
            else
            {
                CStoreServer.Default.StopServer();
                _eventAggregator.Publish(new ServerStateItem(false), nameof(CStoreReceivedViewModel));
            }
        }

        public void ShowReceivedFile(string file)
        {
            _windowManager.ShowDialog(new PreviewImageViewModel(file));
        }

        public void OpenFolder(string file)
        {
            ProcessUtil.Explore(file);
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);

            CStoreServer.Default.OnFilesSaved -= OnFilesSaved;
            CStoreServer.Default.StopServer();
        }
    }
}

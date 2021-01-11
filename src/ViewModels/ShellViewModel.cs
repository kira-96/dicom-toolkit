namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Shell;
    using Logging;
    using Models;
    using Services;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<BusyStateEvent>
    {
        public const string MainWindowName = "Simple DICOM Toolkit";

        private readonly IContainer container;
        private readonly IEventAggregator eventAggregator;
        private readonly IWindowManager windowManager;
        private readonly ILoggerService loggerService;
        private readonly II18nService i18NService;
        private readonly IConfigurationService configurationService;
        private readonly IDataService dataService;
        private readonly INotificationService notificationService;
        private readonly IUpdateService updateService;
        private readonly ISimpleMqttService mqttService;

        public ITaskbarService TaskbarService { get; }

        private bool isUpdateAvailable = false;

        /// <summary>
        /// 是否有更新可用
        /// </summary>
        public bool IsUpdateAvailable
        {
            get => isUpdateAvailable;
            internal set => SetAndNotify(ref isUpdateAvailable, value);
        }

        public ShellViewModel(
            IContainer container,
            IEventAggregator eventAggregator,
            IWindowManager windowManager,
            [Inject("filelogger")]ILoggerService loggerService,
            II18nService i18NService,
            IConfigurationService configurationService,
            IDataService dataService,
            INotificationService notificationService,
            ITaskbarService taskbarService,
            IUpdateService updateService,
            ISimpleMqttService mqttService,
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            WorklistSCPViewModel worklistSCPViewModel,
            QueryRetrieveViewModel queryRetrieveViewModel,
            StoreViewModel storeViewModel,
            StoreSCPViewModel storeSCPViewModel,
            PrintViewModel printViewModel,
            PrintSCPViewModel printSCPViewModel)
        {
            DisplayName = MainWindowName;
            this.container = container;
            this.eventAggregator = eventAggregator;
            this.windowManager = windowManager;
            this.loggerService = loggerService;
            this.i18NService = i18NService;
            this.configurationService = configurationService;
            this.dataService = dataService;
            this.notificationService = notificationService;
            this.TaskbarService = taskbarService;
            this.updateService = updateService;
            this.mqttService = mqttService;

            Items.Add(dcmItemsViewModel);
            Items.Add(worklistViewModel);
            Items.Add(worklistSCPViewModel);
            Items.Add(queryRetrieveViewModel);
            Items.Add(storeViewModel);
            Items.Add(storeSCPViewModel);
            Items.Add(printViewModel);
            Items.Add(printSCPViewModel);
        }

        protected override async void OnViewLoaded()
        {
            base.OnViewLoaded();

            ActiveItem = Items.Count > 0 ? Items[0] : null;

            eventAggregator.Subscribe(this, nameof(WorklistResultViewModel), nameof(QueryResultViewModel));

            updateService.VersionAvaliable += UpdateService_VersionAvaliable;
            updateService.CheckForUpdateError += UpdateService_CheckForUpdateError;
            updateService.DownloadComplete += UpdateService_DownloadComplete;
            updateService.DownloadError += UpdateService_DownloadError;

            await HandleCommandLineArgs(Environment.GetCommandLineArgs());

            configurationService.Load();  // load configuration
            var appConfiguration = configurationService.GetConfiguration<AppConfiguration>();

            await mqttService.StartAsync(appConfiguration.ListenPort);  // start mqtt service
            dataService.ConnectDatabase(appConfiguration.DbConnectionString);  // connect to database

            await CheckForUpdate();  // check for update
        }

        protected override void OnClose()
        {
            base.OnClose();

            updateService.VersionAvaliable -= UpdateService_VersionAvaliable;
            updateService.CheckForUpdateError -= UpdateService_CheckForUpdateError;
            updateService.DownloadComplete -= UpdateService_DownloadComplete;
            updateService.DownloadError -= UpdateService_DownloadError;

            eventAggregator.Unsubscribe(this);
            dataService.DisconnectDatabase();
            container.Get<IMessengerService>().Dispose();
            mqttService.Dispose();
        }

        public void Handle(BusyStateEvent message)
        {
            var worklistResultViewModel = (Items[1] as WorklistViewModel).WorklistResultViewModel;
            var queryResultViewModel = (Items[3] as QueryRetrieveViewModel).QueryResultViewModel;

            TaskbarService.ProgressState = worklistResultViewModel.IsBusy || queryResultViewModel.IsBusy ?
                TaskbarItemProgressState.Indeterminate : TaskbarItemProgressState.None;
        }

        public async void ConfirmUpdate()
        {
            await ConfirmUpdateAsync();
        }

        private async ValueTask ConfirmUpdateAsync()
        {
            var result = windowManager.ShowMessageBox(
                    i18NService.GetXmlStringByKey("UpdateAvailableContent"),
                    i18NService.GetXmlStringByKey("InfoCaption"),
                    System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                updateService.StartDownload();
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("ToastStartDownload"), new TimeSpan(0, 0, 3));
            }
        }

        private async ValueTask HandleCommandLineArgs(string[] args)
        {
            if (args.Length >= 2)
            {
                string filename = args[1];

                if (filename == "-h" ||
                    filename == "--help")
                {
                    Console.WriteLine("Drag a Dicom file and drop on toolkit to open it.");
                }

                if (System.IO.File.Exists(filename))
                {
                    await (Items[0] as DcmItemsViewModel).OpenDicomFileAsync(args[1]);
                }
            }
        }

        private async Task CheckForUpdate()
        {
            await Task.Delay(5_000);  // delay 5s

            if (Helpers.SystemHelper.IsNetworkConnected)
            {
                await updateService.CheckForUpdateAsync();
            }
            else
            {
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("ToastNetworkOffline"), new TimeSpan(0, 0, 3), Controls.ToastType.Error);
            }
        }

        private async void UpdateService_VersionAvaliable(Version newVersion)
        {
            Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (newVersion > currentVersion)
            {
                IsUpdateAvailable = true;  // 有可用更新

                await ConfirmUpdateAsync();
            }
            else
            {
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("ToastAlreadyLatestVersion"), new TimeSpan(0, 0, 3));
            }
        }

        private async void UpdateService_DownloadComplete(object s, EventArgs e)
        {
            string content = string.Format(
                i18NService.GetXmlStringByKey("ToastDownloadComplete"),
                i18NService.GetXmlStringByKey("Failed"));

            loggerService.Info(content);

            await Execute.OnUIThreadAsync(async () => await notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3)));
        }

        private async void UpdateService_DownloadError(Exception ex)
        {
            string content = string.Format(
                i18NService.GetXmlStringByKey("ToastDownloadUpdateResult"),
                i18NService.GetXmlStringByKey("Failed"));

            loggerService.Error(ex, content);

            await Execute.OnUIThreadAsync(async () => await notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3), Controls.ToastType.Error));
        }

        private async void UpdateService_CheckForUpdateError(Exception ex)
        {
            string content = string.Format(
                i18NService.GetXmlStringByKey("ToastCheckUpdateResult"),
                i18NService.GetXmlStringByKey("Failed"));

            loggerService.Error(ex, content);

            await Execute.OnUIThreadAsync(async () => await notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3), Controls.ToastType.Error));
        }
    }
}

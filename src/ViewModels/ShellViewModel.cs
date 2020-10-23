namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Services;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public const string MainWindowName = "Simple DICOM Toolkit";

        private readonly IContainer container;
        private readonly IWindowManager windowManager;
        private readonly ILoggerService loggerService;
        private readonly II18nService i18NService;
        private readonly IConfigurationService configurationService;
        private readonly IDataService dataService;
        private readonly INotificationService notificationService;
        private readonly IUpdateService updateService;
        private readonly ISimpleMqttService mqttService;

        public ShellViewModel(
            IContainer container,
            IWindowManager windowManager,
            [Inject("filelogger")]ILoggerService loggerService,
            II18nService i18NService,
            IConfigurationService configurationService,
            IDataService dataService,
            INotificationService notificationService,
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
            this.windowManager = windowManager;
            this.loggerService = loggerService;
            this.i18NService = i18NService;
            this.configurationService = configurationService;
            this.dataService = dataService;
            this.notificationService = notificationService;
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

            updateService.VersionAvaliable += UpdateService_VersionAvaliable;
            updateService.CheckForUpdateError += UpdateService_CheckForUpdateError;
            updateService.DownloadComplete += UpdateService_DownloadComplete;
            updateService.DownloadError += UpdateService_DownloadError;

            configurationService.Load();  // load configuration
            var appConfiguration = configurationService.GetConfiguration<AppConfiguration>();

            await mqttService.StartAsync(appConfiguration.ListenPort);
            dataService.ConnectDatabase(appConfiguration.DbConnectionString);

            ActiveItem = Items.Count > 0 ? Items[0] : null;

            await HandleCommandLineArgs(Environment.GetCommandLineArgs());

            await CheckForUpdate();
        }

        protected override void OnClose()
        {
            base.OnClose();

            updateService.VersionAvaliable -= UpdateService_VersionAvaliable;
            updateService.CheckForUpdateError -= UpdateService_CheckForUpdateError;
            updateService.DownloadComplete -= UpdateService_DownloadComplete;
            updateService.DownloadError -= UpdateService_DownloadError;

            dataService.DisconnectDatabase();
            container.Get<IMessengerService>().Dispose();
            mqttService.Dispose();
        }

        private async Task HandleCommandLineArgs(string[] args)
        {
            if (args.Length == 2)
            {
                if (System.IO.File.Exists(args[1]))
                {
                    await (Items[0] as DcmItemsViewModel).OpenDicomFileAsync(args[1]);
                }
            }
        }

        private async Task CheckForUpdate()
        {
            if (Helpers.SystemHelper.IsNetworkConnected)
            {
                await updateService.CheckForUpdate();
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

            await Execute.OnUIThreadAsync(() => notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3)));
        }

        private async void UpdateService_DownloadError(Exception ex)
        {
            string content = string.Format(
                i18NService.GetXmlStringByKey("ToastDownloadUpdateResult"),
                i18NService.GetXmlStringByKey("Failed"));

            loggerService.Error(ex, content);

            await Execute.OnUIThreadAsync(() => notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3), Controls.ToastType.Error));
        }

        private async void UpdateService_CheckForUpdateError(Exception ex)
        {
            string content = string.Format(
                i18NService.GetXmlStringByKey("ToastCheckUpdateResult"),
                i18NService.GetXmlStringByKey("Failed"));

            loggerService.Error(ex, content);

            await Execute.OnUIThreadAsync(() => notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3), Controls.ToastType.Error));
        }
    }
}

namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using Logging;
    using Server;
    using Services;
    using Helpers;

    public class PrintSCPViewModel : Screen, IDisposable
    {
        [Inject]
        private IWindowManager _windowManager;

        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private II18nService i18NService;

        [Inject]
        private IConfigurationService configurationService;

        [Inject]
        private IMessengerService messenger;

        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PrintJobsViewModel PrintJobsViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public PrintSCPViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Print SCP";
            this.eventAggregator = eventAggregator;
        }

        protected override async void OnInitialActivate()
        {
            base.OnInitialActivate();

            PrintJobsViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerIP = SystemHelper.LocalIPAddress;
            ServerConfigViewModel.ServerPort = "7104";
            ServerConfigViewModel.LocalAET = ServerConfigViewModel.ServerAET = "PRINTSCP";
            ServerConfigViewModel.IsServerIPEnabled = ServerConfigViewModel.IsServerAETEnabled = ServerConfigViewModel.IsModalityEnabled = false;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishServerRequest(nameof(ViewModels.PrintJobsViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.PrintJobsViewModel));
            PrintServer.Default.PrinterName = configurationService.GetConfiguration<string>("PrinterSettings");
            await messenger.SubscribeAsync(this, "Config", ReloadPrinterSettings);
        }

        public void ShowOptions()
        {
            string configexe = "Config.exe";

            if (!File.Exists(configexe))
            {
                string info = string.Format(i18NService.GetXmlStringByKey("FileNotFound"), configexe);
                _windowManager.ShowMessageBox(info);
                return;
            }

            if (!ProcessHelper.StartProcess(configexe, "1"))
            {
                _logger.Warn("Start process failure. [{0}]", configexe);
            }
        }

        private void ReloadPrinterSettings(string file)
        {
            configurationService.Load("PrinterSettings");
            PrintServer.Default.PrinterName = configurationService.GetConfiguration<string>("PrinterSettings");
        }

        public async void Dispose()
        {
            await messenger.UnsubscribeAsync(this, "Config");
            ServerConfigViewModel.Dispose();
            PrintJobsViewModel.Dispose();
        }
    }
}

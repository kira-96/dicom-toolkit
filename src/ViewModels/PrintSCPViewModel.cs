namespace SimpleDICOMToolkit.ViewModels
{
    using Nett;
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using Logging;
    using MQTT;
    using Server;
    using Services;
    using Utils;

    public class PrintSCPViewModel : Screen, IDisposable
    {
        [Inject]
        private IWindowManager _windowManager;

        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private II18nService i18NService;

        [Inject]
        private IMessenger messenger;

        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PrintJobsViewModel PrintJobsViewModel { get; private set; }

        public PrintSCPViewModel()
        {
            DisplayName = "Print SCP";
        }

        protected override async void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
            PrintJobsViewModel.Parent = this;
            ReloadPrinterSettings("config.toml");
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

            if (!ProcessUtil.StartProcess(configexe, "1"))
            {
                _logger.Warn("Start process failure. [{0}]", configexe);
            }
        }

        private void ReloadPrinterSettings(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            TomlTable table = Toml.ReadFile(file);

            if (table.ContainsKey("PrinterSettings"))
            {
                TomlTable settings = table.Get<TomlTable>("PrinterSettings");
                if (settings.ContainsKey("Printer"))
                {
                    PrintServer.Default.PrinterName = settings.Get<TomlString>("Printer").Value;
                }
            }
        }

        public async void Dispose()
        {
            // TODO
            await messenger.UnsubscribeAsync(this, "Config");
            ServerConfigViewModel.Dispose();
            PrintJobsViewModel.Dispose();
        }
    }
}

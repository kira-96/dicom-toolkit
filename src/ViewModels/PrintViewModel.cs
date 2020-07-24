namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using Client;
    using Logging;
    using MQTT;
    using Services;
    using Utils;

    public class PrintViewModel : Screen, IDisposable
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
        public PrintPreviewViewModel PrintPreviewViewModel { get; private set; }

        public PrintOptions PrintOptions { get; private set; }

        public PrintViewModel()
        {
            DisplayName = "Print";
            PrintOptions = new PrintOptions();
        }

        protected override async void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
            PrintPreviewViewModel.Parent = this;
            await PrintPreviewViewModel.AddSampleImage();
            PrintOptions = configurationService.GetConfiguration<PrintOptions>("PrintOptions");
            await messenger.SubscribeAsync(this, "Config", ReloadPrintOptions);
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

            if (!ProcessUtil.StartProcess(configexe, "0"))
            {
                _logger.Warn("Start process failure. [{0}]", configexe);
            }
        }

        private void ReloadPrintOptions(string file)
        {
            configurationService.Load("PrintOptions");
            PrintOptions = configurationService.GetConfiguration<PrintOptions>("PrintOptions");
        }

        public async void Dispose()
        {
            // TODO
            await messenger.UnsubscribeAsync(this, "Config");
            ServerConfigViewModel.Dispose();
            PrintPreviewViewModel.Dispose();
        }
    }
}

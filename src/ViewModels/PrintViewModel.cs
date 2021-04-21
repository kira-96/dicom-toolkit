namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Client;
    using Infrastructure;
    using Logging;
    using Services;
    using Helpers;

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

        private readonly IEventAggregator eventAggregator;

        public PrintViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Print";
            this.eventAggregator = eventAggregator;
        }

        protected override async void OnInitialActivate()
        {
            base.OnInitialActivate();

            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerPort = "7104";
            ServerConfigViewModel.ServerAET = "PRINT-SCP"; ;
            ServerConfigViewModel.IsModalityEnabled = false;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishClientRequest(nameof(ViewModels.PrintPreviewViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.PrintPreviewViewModel));
            PrintPreviewViewModel.Parent = this;
            var printConfig = configurationService.Get<PrintConfiguration>();
            PrintOptions = new PrintOptions()
            {
                Orientation = printConfig.Orientation,
                FilmSize = printConfig.Size,
                MagnificationType = printConfig.Magnification,
                MediumType = printConfig.Medium
            };
            await messenger.SubscribeAsync(this, "Config", ReloadPrintOptions);
            await Task.Run(async () =>
            {
                await Execute.PostToUIThreadAsync(async () => await PrintPreviewViewModel.AddSampleImageAsync());
            });
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

            if (!ProcessHelper.StartProcess(configexe, "0"))
            {
                _logger.Warn("Start process failure. [{0}]", configexe);
            }
        }

        private void ReloadPrintOptions(string token)
        {
            configurationService.Load(token);
            var printConfig = configurationService.Get<PrintConfiguration>();
            PrintOptions.Orientation = printConfig.Orientation;
            PrintOptions.FilmSize = printConfig.Size;
            PrintOptions.MagnificationType = printConfig.Magnification;
            PrintOptions.MediumType = printConfig.Medium;
        }

        public async void Dispose()
        {
            await messenger.UnsubscribeAsync(this, "Config");
            ServerConfigViewModel.Dispose();
            PrintPreviewViewModel.Dispose();
        }
    }
}

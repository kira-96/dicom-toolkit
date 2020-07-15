namespace SimpleDICOMToolkit.ViewModels
{
    using MQTT;
    using Nett;
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using Client;
    using Logging;
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
        private IMessenger messenger;

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
            ReloadPrintOptions("config.toml");
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
            if (!File.Exists(file))
            {
                return;
            }

            TomlTable table = Toml.ReadFile(file);

            if (table.ContainsKey("PrintOptions"))
            {
                TomlTable options = table.Get<TomlTable>("PrintOptions");
                if (options.ContainsKey("Orientation"))
                    PrintOptions.Orientation = (FilmOrientation)options.Get<TomlInt>("Orientation").Value;
                if (options.ContainsKey("Size"))
                    PrintOptions.FilmSize = (FilmSize)options.Get<TomlInt>("Size").Value;
                if (options.ContainsKey("Magnification"))
                    PrintOptions.MagnificationType = (MagnificationType)options.Get<TomlInt>("Magnification").Value;
                if (options.ContainsKey("Medium"))
                    PrintOptions.MediumType = (MediumType)options.Get<TomlInt>("Medium").Value;
            }
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

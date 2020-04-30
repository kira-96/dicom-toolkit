namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Diagnostics;
    using MQTT;
    using System.IO;
    using Nett;
    using SimpleDICOMToolkit.Client;

    public class PrintViewModel : Screen, IDisposable
    {
        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private IMessenger messenger;

        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        //[Inject]
        //public PrintOptionsViewModel PrintOptionsViewModel { get; private set; }

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
            ReloadPrintOptions("config.toml");
            await messenger.SubscribeAsync(this, "Config", ReloadPrintOptions);
        }

        public void ShowOptions()
        {
            // _windowManager.ShowDialog(PrintOptionsViewModel);

            string configexe = "Config.exe";

            if (!File.Exists(configexe))
            {
                _windowManager.ShowMessageBox("找不到 Config.exe");
                return;
            }

            ProcessStartInfo info = new ProcessStartInfo(configexe, "0");
            Process process = new Process()
            {
                StartInfo = info
            };
            process.Start();
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

        public void Dispose()
        {
            // TODO
            // await messenger.UnsubscribeAsync(this, "Config");
            ServerConfigViewModel.Dispose();
            // PrintOptionsViewModel.Dispose();
            PrintPreviewViewModel.Dispose();
        }
    }
}

namespace SimpleDICOMToolkit.ViewModels
{
    using Nett;
    using SimpleDICOMToolkit.MQTT;
    using SimpleDICOMToolkit.Server;
    using Stylet;
    using StyletIoC;
    using System;
    using System.Diagnostics;
    using System.IO;

    public class PrintSCPViewModel : Screen, IDisposable
    {
#pragma warning disable IDE0044, 0649
        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private IMessenger messenger;
#pragma warning disable IDE0044, 0649

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
            //PrintDialog dlg = new PrintDialog();
            //dlg.ShowDialog();

            string configexe = "Config.exe";

            if (!File.Exists(configexe))
            {
                _windowManager.ShowMessageBox("找不到 Config.exe");
                return;
            }

            ProcessStartInfo info = new ProcessStartInfo(configexe, "1");
            Process process = new Process()
            {
                StartInfo = info
            };
            process.Start();
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

        public void Dispose()
        {
            // TODO
            // await messenger.UnsubscribeAsync(this, "Config");
            ServerConfigViewModel.Dispose();
            PrintJobsViewModel.Dispose();
        }
    }
}

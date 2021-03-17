namespace Config.ViewModels
{
    using Stylet;
    using YamlDotNet.Serialization;
    using System;
    using System.IO;
    using Infrastructure;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private const string CONFIG_FILE = "config.yml";
        private AppConfiguration _config;

        private readonly PrintOptionsViewModel printOptionsViewModel;
        private readonly PrinterSettingsViewModel printerSettingsViewModel;

        public AppConfiguration Configuration
        {
            get
            {
                if (_config == null)
                {
                    if (File.Exists(CONFIG_FILE))
                    {
                        try
                        {
                            string input = File.ReadAllText(CONFIG_FILE);
                            DeserializerBuilder builder = new();
                            _config = builder.Build().Deserialize<AppConfiguration>(input);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (_config == null)
                    {
                        _config = new AppConfiguration()
                        {
                            Print = new PrintConfiguration(),
                            Printer = new PrinterConfiguration(),
                            Misc = new MiscConfiguration()
                        };
                    }
                }

                return _config;
            }
        }

        public ShellViewModel(
            PrintOptionsViewModel printOptionsViewModel,
            PrinterSettingsViewModel printerSettingsViewModel)
        {
            DisplayName = "Config";

            this.printOptionsViewModel = printOptionsViewModel;
            this.printerSettingsViewModel = printerSettingsViewModel;

            Items.Add(printOptionsViewModel);
            Items.Add(printerSettingsViewModel);
        }

        public async void OkCommand()
        {
            // Save and notify
            SaveConfig();
            // Notify
            Messenger.Default.ServerPort = Configuration.Misc.ListenPort;
            await Messenger.Default.PublishAsync("Config", Guid.NewGuid().ToString(), System.Threading.CancellationToken.None);

            RequestClose();
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            if (Items.Count == 0)
                return;

            LoadConfig();

            string[] args = Environment.GetCommandLineArgs();

            ActivateItemByArgs(args);
        }

        private void ActivateItemByArgs(string[] args)
        {
            if (args.Length <= 1)
            {
                ActiveItem = Items[0];
                return;
            }

            if (int.TryParse(args[1], out int index) &&
                index < Items.Count)
            {
                ActiveItem = Items[index];
                return;
            }

            ActiveItem = Items[0];
        }

        private void LoadConfig()
        {
            printOptionsViewModel.Orientation = Configuration.Print.Orientation;
            printOptionsViewModel.Size = Configuration.Print.Size;
            printOptionsViewModel.Magnification = Configuration.Print.Magnification;
            printOptionsViewModel.Medium = Configuration.Print.Medium;
            printerSettingsViewModel.Printer = Configuration.Printer.Printer;
        }

        private void SaveConfig()
        {
            Configuration.Print.Orientation = printOptionsViewModel.Orientation;
            Configuration.Print.Size = printOptionsViewModel.Size;
            Configuration.Print.Magnification = printOptionsViewModel.Magnification;
            Configuration.Print.Medium = printOptionsViewModel.Medium;
            Configuration.Printer.Printer = printerSettingsViewModel.Printer;

            SerializerBuilder builder = new();
            string content = builder.Build().Serialize(Configuration);

            File.WriteAllText(CONFIG_FILE, content);
        }
    }
}

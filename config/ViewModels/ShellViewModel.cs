namespace Config.ViewModels
{
    using Nett;
    using Stylet;
    using System;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private const string CONFIG_FILE = "config.toml";
        private TomlTable _configTable;

        public TomlTable ConfigTable
        {
            get
            {
                if (_configTable == null)
                {
                    if (System.IO.File.Exists(CONFIG_FILE))
                    {
                        try
                        {
                            _configTable = Toml.ReadFile(CONFIG_FILE);
                        }
                        catch (Exception)
                        {
                            _configTable = Toml.Create();
                        }
                    }
                    else
                    {
                        _configTable = Toml.Create();
                    }
                }

                return _configTable;
            }
        }

        public ShellViewModel(
            PrintOptionsViewModel printOptionsViewModel,
            PrinterSettingsViewModel printerSettingsViewModel)
        {
            DisplayName = "Config";

            Items.Add(printOptionsViewModel);
            Items.Add(printerSettingsViewModel);
        }

        public async void OkCommand()
        {
            // Save and notify
            SaveConfig();
            // Notify
            await Messenger.Default.PublishAsync("Config", CONFIG_FILE, System.Threading.CancellationToken.None);

            RequestClose();
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            if (Items.Count == 0)
                return;

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

        private void SaveConfig()
        {
            foreach (var item in Items)
            {
                (item as IConfigViewModel).SaveConfig();
            }

            Toml.WriteFile(ConfigTable, CONFIG_FILE);
        }
    }
}

using Nett;
using Stylet;
using System.Drawing.Printing;

namespace Config.ViewModels
{
    public class PrinterSettingsViewModel : Screen, IConfigViewModel
    {
        public BindableCollection<string> Printers { get; private set; }

        private string _printer;

        public string Printer 
        {
            get => _printer;
            set => SetAndNotify(ref _printer, value);
        }

        public PrinterSettingsViewModel()
        {
            DisplayName = "Printer Settings";
            Printers = new BindableCollection<string>();
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            LoadConfigs((Parent as ShellViewModel).ConfigTable);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            Printers.Clear();

            PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;

            foreach (string printer in printers)
            {
                Printers.Add(printer);
            }

            if (!Printers.Contains(Printer))
            {
                Printer = "Microsoft XPS Document Writer";  // XPS - Windows 系统都有
            }
        }

        private void LoadConfigs(TomlTable table)
        {
            if (table.ContainsKey("PrinterSettings"))
            {
                TomlTable settings = table.Get<TomlTable>("PrinterSettings");
                if (settings.ContainsKey("Printer"))
                {
                    Printer = settings.Get<TomlString>("Printer").Value;
                }
            }
        }

        public void SaveConfig()
        {
            TomlTable settings = Toml.Create();
            settings.Add("Printer", Printer);

            TomlTable table = (Parent as ShellViewModel).ConfigTable;

            if (table.ContainsKey("PrinterSettings"))
                table.Remove("PrinterSettings");

            table.Add("PrinterSettings", settings);
        }

        public void Dispose()
        {
            // TODO
        }
    }
}

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

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            GetLocalPrinters();
        }

        private void GetLocalPrinters()
        {
            string temp = Printer;

            Printers.Clear();

            PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;

            foreach (string printer in printers)
            {
                Printers.Add(printer);
            }

            // 允许保存到图像
            Printers.Add("PNG");

            if (!Printers.Contains(temp))
            {
                Printer = "Microsoft XPS Document Writer";  // XPS - Windows 系统都有
            }
            else
            {
                Printer = temp;
            }
        }

        public void LoadConfigs(TomlTable table)
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

        public void SaveConfig(TomlTable table)
        {
            TomlTable settings = Toml.Create();
            settings.Add("Printer", Printer);

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

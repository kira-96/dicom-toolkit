using StyletIoC;
using Nett;
using System;
using System.IO;
using SimpleDICOMToolkit.Client;
using SimpleDICOMToolkit.Logging;

namespace SimpleDICOMToolkit.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILoggerService loggerService;

        private const string CONFIG_FILE = "config.toml";

        private AppConfiguration appConfiguration;
        private PrintOptions printOptions;
        private string printer;

        public ConfigurationService([Inject("filelogger")] ILoggerService loggerService)
        {
            this.loggerService = loggerService;

            Load();
        }

        public void Load(string section = null)
        {
            TomlTable configTable = null;

            if (File.Exists(CONFIG_FILE))
            {
                try
                {
                    configTable = Toml.ReadFile(CONFIG_FILE);
                }
                catch (Exception ex)
                {
                    loggerService.Error(ex, "Read configuration file error.");
                    configTable = null;
                }
            }
            else
            {
                loggerService.Warn("{0} not exist. use default settings.");
            }

            if (section == null || section == "Application")
            {
                appConfiguration = LoadAppConfiguration(configTable);
            }

            if (section == null || section == "PrintOptions")
            {
                printOptions = LoadPrintConfiguration(configTable);
            }
            
            if (section == null || section == "PrinterSettings")
            {
                printer = LoadPrinterConfiguration(configTable);
            }
        }

        public T GetConfiguration<T>(string section = null)
        {
            if (typeof(T) == typeof(AppConfiguration) ||
                section == "Application")
            {
                return (T)(object)appConfiguration;
            }

            if (typeof(T) == typeof(PrintOptions) ||
                section == "PrintOptions")
            {
                return (T)(object)printOptions;
            }
            
            if (section == "PrinterSettings")
            {
                return (T)(object)printer;
            }

            return default(T);
        }

        private AppConfiguration LoadAppConfiguration(TomlTable configTable)
        {
            AppConfiguration appConfiguration = new AppConfiguration();

            if (configTable != null && configTable.ContainsKey("Application"))
            {
                TomlTable settings = configTable.Get<TomlTable>("Application");

                if (settings.ContainsKey("ListenPort"))
                {
                    appConfiguration.ListenPort = (int)settings.Get<TomlInt>("ListenPort").Value;
                }
            }

            return appConfiguration;
        }

        private PrintOptions LoadPrintConfiguration(TomlTable configTable)
        {
            PrintOptions options = new PrintOptions();

            if (configTable != null && configTable.ContainsKey("PrintOptions"))
            {
                TomlTable settings = configTable.Get<TomlTable>("PrintOptions");

                if (settings.ContainsKey("Orientation"))
                    options.Orientation = (FilmOrientation)settings.Get<TomlInt>("Orientation").Value;
                if (settings.ContainsKey("Size"))
                    options.FilmSize = (FilmSize)settings.Get<TomlInt>("Size").Value;
                if (settings.ContainsKey("Magnification"))
                    options.MagnificationType = (MagnificationType)settings.Get<TomlInt>("Magnification").Value;
                if (settings.ContainsKey("Medium"))
                    options.MediumType = (MediumType)settings.Get<TomlInt>("Medium").Value;
            }

            return options;
        }

        private string LoadPrinterConfiguration(TomlTable configTable)
        {
            if (configTable != null && configTable.ContainsKey("PrinterSettings"))
            {
                TomlTable settings = configTable.Get<TomlTable>("PrinterSettings");

                if (settings.ContainsKey("Printer"))
                {
                    return settings.Get<TomlString>("Printer").Value;
                }
            }

            return "Microsoft XPS Document Writer";
        }
    }
}

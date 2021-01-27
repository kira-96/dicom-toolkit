using StyletIoC;
using YamlDotNet.Serialization;
using System;
using System.IO;
using SimpleDICOMToolkit.Infrastructure;
using SimpleDICOMToolkit.Logging;

namespace SimpleDICOMToolkit.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILoggerService loggerService;

        private const string CONFIG_FILE = "config.yml";

        private AppConfiguration appConfiguration;

        public string Token { get; private set; } = "";

        public ConfigurationService([Inject("filelogger")] ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public void Load(string token)
        {
            if (Token == token)
            {
                // 不需要重新加载
                return;
            }

            if (File.Exists(CONFIG_FILE))
            {
                try
                {
                    string input = File.ReadAllText(CONFIG_FILE);
                    DeserializerBuilder builder = new DeserializerBuilder();
                    appConfiguration = builder.Build().Deserialize<AppConfiguration>(input);
                }
                catch (Exception ex)
                {
                    loggerService.Error(ex, "Read configuration file error.");
                }
            }
            else
            {
                loggerService.Warn("{0} not exist. use default settings.", CONFIG_FILE);
            }

            if (appConfiguration == null)
            {
                appConfiguration = new AppConfiguration()
                {
                    Print = new PrintConfiguration(),
                    Printer = new PrinterConfiguration(),
                    Misc = new MiscConfiguration()
                };
            }

            Token = token;
        }

        public T GetConfiguration<T>(string section = null)
        {
            if (typeof(T) == typeof(MiscConfiguration) ||
                section == "Misc")
            {
                return (T)(object)appConfiguration.Misc;
            }

            if (typeof(T) == typeof(PrintConfiguration) ||
                section == "Print")
            {
                return (T)(object)appConfiguration.Print;
            }
            
            if (typeof(T) == typeof(PrinterConfiguration) ||
                section == "Printer")
            {
                return (T)(object)appConfiguration.Printer;
            }

            return default;
        }
    }
}

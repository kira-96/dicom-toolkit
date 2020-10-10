namespace SimpleDICOMToolkit.Logging
{
    using System;
    using StyletIoC;
    using NLogger = NLog.Logger;

    [Inject(Key = "filelogger")]
    public class LoggerService : ILoggerService
    {
        private readonly NLogger logger = NLog.LogManager.GetLogger("LoggerService");

        public void Trace(string message, params object[] args)
        {
            logger.Trace(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            logger.Debug(message, args);
        }

        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public void Warn(string message, params object[] args)
        {
            logger.Warn(message, args);
        }

        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }

        public void Error(Exception exception, string message = null, params object[] args)
        {
            logger.Error(exception, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            logger.Fatal(message, args);
        }

        public void Fatal(Exception exception, string message = null, params object[] args)
        {
            logger.Fatal(exception, message, args);
        }
    }
}

namespace SimpleDICOMToolkit.Logging
{
    public interface ILoggerService
    {
        void Trace(string message, params object[] args);

        void Debug(string message, params object[] args);

        void Info(string message, params object[] args);

        void Warn(string message, params object[] args);

        void Error(string message, params object[] args);

        void Error(System.Exception exception, string message = null, params object[] args);

        void Fatal(string message, params object[] args);

        void Fatal(System.Exception exception, string message = null, params object[] args);
    }
}

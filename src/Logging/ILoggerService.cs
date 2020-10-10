namespace SimpleDICOMToolkit.Logging
{
    public interface ILoggerService
    {
        /// <summary>
        /// Trace log
        /// </summary>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Trace(string message, params object[] args);

        /// <summary>
        /// Trace log
        /// </summary>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Info log
        /// </summary>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Warn log
        /// </summary>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="exception">exception</param>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Error(System.Exception exception, string message = null, params object[] args);

        /// <summary>
        /// Fatal log
        /// </summary>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Fatal log
        /// </summary>
        /// <param name="exception">exception</param>
        /// <param name="message">format string</param>
        /// <param name="args">args</param>
        void Fatal(System.Exception exception, string message = null, params object[] args);
    }
}

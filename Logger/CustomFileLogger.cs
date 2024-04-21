/// <summary>
/// Date: 14-Apr-2024
/// Course: CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Khoa Minh Ngo and Duke Nguyen - This work may not
///            be copied for use in Academic Coursework.
/// 
/// We, Khoa Minh Ngo and Duke Nguyen, certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source. All references used in the completion 
/// of the assignments are cited in my README file
/// 
/// File Contents
/// 
///     This library class represents a custom file logger.    
/// </summary>

using Microsoft.Extensions.Logging;

namespace Logger
{
    /// <summary>
    /// Represents a custom file logger implementation.
    /// </summary>
    public class CustomFileLogger : ILogger
    {
        public readonly string filename;

        /// <summary>
        /// Construct a Custom File Logger
        /// </summary>
        /// <param name="categoryName">The category name for the logger</param>
        public CustomFileLogger(string categoryName)
        {
            filename = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData)
                + Path.DirectorySeparatorChar
                + $"CS3500-{categoryName}.log";

        }

        /// <summary>
        /// Creates a new scope for logging
        /// </summary>
        /// <typeparam name="TState">type of the scope state</typeparam>
        /// <param name="state">the scope state</param>
        /// <returns>An IDisposable that ends the logging scope when disposed.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the given log level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level to check</param>
        /// <returns>True if the log level is enabled, otherwise false</returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// write a log entry
        /// </summary>
        /// <typeparam name="TState">type of the log state</typeparam>
        /// <param name="logLevel">the log level of the log entry</param>
        /// <param name="eventId">the EventId of the log entry</param>
        /// <param name="state">The state of the log entry</param>
        /// <param name="exception">exception of log entry</param>
        /// <param name="formatter">the function that creates log message</param>

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string logMessage = $"{DateTimeOffset.Now:yyyy-MM-dd h:mm:ss tt} ({Thread.GetDomainID()}) - {logLevel} - This message was produced by a Log{logLevel} call. Content: {formatter(state, exception)}{Environment.NewLine}";

            try
            {
                File.AppendAllText(filename, logMessage);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Writing log file '{filename}' failed: " + ex.Message);
            }
        }
    }
}

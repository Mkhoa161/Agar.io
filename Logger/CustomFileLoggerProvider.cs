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
///     This class represents a custom logger provider that creates logger instances for logging to files.
///     
/// </summary>

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;


namespace Logger
{
    /// <summary>
    /// A custom logger provider that creates logger instances for logging to files   
    /// </summary>
    public class CustomFileLoggerProvider : ILoggerProvider
    {
        // Store created logger instances
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        // minimum log level to be written
        private LogLevel _logLevel = LogLevel.Information;

        /// <summary>
        /// State of the logger provider
        /// </summary>
        public object State
        {
            get => _logLevel;
            set => _logLevel = (LogLevel)value;
        }

        /// <summary>
        /// Create logger instance given an inputted category name
        /// </summary>
        /// <param name="categoryName">category of the logger</param>
        /// <returns>logger instance</returns>
        /// <exception cref="NotImplementedException"></exception>
        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName);
        }

        /// <summary>
        /// Dispose created logger instances
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            foreach (var logger in _loggers.Values)
            {
                (logger as IDisposable)?.Dispose();
            }

            _loggers.Clear();
        }
    }
}

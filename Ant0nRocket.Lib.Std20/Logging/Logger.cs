using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ant0nRocket.Lib.Std20.Logging
{
    /// <summary>
    /// The main idea of this logging class is to provide some basic logging
    /// functionality for all classes inside this library (and only for them if required).<br />
    /// Initialy there is no writing of files, or sending via network, etc.<br />
    /// It's just a mock for logging. But you can subscribe to events of Logger class
    /// and write as much logs in as many places as you need.<br />
    /// For example, in your target project #1 you use NLog, and your project #2 uses log4net.
    /// There is not problem to subscribe to events and send data inside those loggers.<br />
    /// Logger levels are match standards.
    /// </summary>
    public class Logger
    {
        private readonly string ownerClassName;

        public Logger()
        {
            ownerClassName = nameof(Logger);
        }

        public Logger(string ownerClassName)
        {
            this.ownerClassName = ownerClassName;
        }

        public void LogTrace(string message, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Trace, ownerClassName, senderMethodName, sender);
        }

        public void LogDebug(string message, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Debug, ownerClassName, senderMethodName, sender);
        }

        public void LogInformation(string message, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Info, ownerClassName, senderMethodName, sender);
        }

        public void LogWarning(string message, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Warn, ownerClassName, senderMethodName, sender);
        }

        public void LogError(string message, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Error, ownerClassName, senderMethodName, sender);
        }

        public void LogException(Exception ex, string prefix = default, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            const string STD_PREFIX = "Exception";
            var innerExceptionText = ex.InnerException == default ? string.Empty : $" (inner ex.: {ex.InnerException.Message}";
            var message = $"{prefix ?? STD_PREFIX}: {ex.Message}{innerExceptionText}";
            LogError(message, sender, senderMethodName);
        }

        public void LogFatal(string message, object sender = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Fatal, ownerClassName, senderMethodName, sender);
        }

        #region EntityFramework

        private static bool isEntityFrameworkLoggingEnabled = true;

        public static void EnableEntityFrameworkLogging() => isEntityFrameworkLoggingEnabled = true;
        public static void DisableEntityFrameworkLoggins() => isEntityFrameworkLoggingEnabled = false;

        public void LogEF(string value)
        {
            if (isEntityFrameworkLoggingEnabled)
                Log(value, LogLevel.Trace, this.GetType().ToString(), null, this);
        }

        #endregion

        private static LogLevel currentLoggerLevel = LogLevel.Trace;

        private static readonly Dictionary<string, Logger> knownLoggers = new();

        private static Logger RegisterLogger(string ownerClassName = default)
        {
            if (ownerClassName == default)
                ownerClassName = nameof(Logger);

            if (knownLoggers.ContainsKey(ownerClassName))
                return knownLoggers[ownerClassName];
            else
                knownLoggers.Add(ownerClassName, new(ownerClassName));

            return RegisterLogger(ownerClassName);
        }

        public static Logger Create() => RegisterLogger();

        public static Logger Create<T>() => RegisterLogger($"{typeof(T)}");

        public static Logger Create(string ownerClassName) => RegisterLogger(ownerClassName);

        public static event EventHandler<(DateTime Date, string Message, LogLevel Level, string SenderClassName, string SenderMethodName)> OnLog;

        public static bool LogToBasicLogWritter { get; set; } = false;

        public static void Log(string message, LogLevel level = LogLevel.Trace, string senderClassName = default, [CallerMemberName] string senderMethodName = default, object senderInstance = default)
        {
            if ((int)level < (int)currentLoggerLevel) return;

            var dateTimeOfMessage = DateTime.Now;

            OnLog?.Invoke(senderInstance, (dateTimeOfMessage, message, level, senderClassName, senderMethodName));

            if (LogToBasicLogWritter)
                BasicLogWritter.WriteToLog(dateTimeOfMessage, message, level, senderClassName, senderMethodName);

        }

        public static void SetLogLevel(LogLevel level) => currentLoggerLevel = level;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using Ant0nRocket.Lib.Extensions;

namespace Ant0nRocket.Lib.Logging
{
    /// <summary>
    /// The main idea of this logging class is to provide some basic logging
    /// functionality for all classes inside this library.<br />
    /// Initialy there is no writing of files, or sending via network, etc.<br />
    /// It's just a mock for logging. But you can subscribe to events of Logger class
    /// and write as much logs in as many places as you need.<br />
    /// For example, in your target project #1 you use NLog, and your project #2 uses log4net.
    /// There is not problem to subscribe to events and send data inside those loggers.<br />
    /// Logger levels are match standards.
    /// </summary>
    public static class Logger
    {
        public static void LogTrace(string message, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Trace, senderClassName, senderMethodName);
        }

        public static void LogDebug(string message, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            var callerName = GetCallerClassName();
            Log(message, LogLevel.Debug, senderClassName, senderMethodName);
        }

        public static void LogInformation(string message, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Info, senderClassName, senderMethodName);
        }

        public static void LogWarning(string message, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Warn, senderClassName, senderMethodName);
        }

        public static void LogError(string message, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Error, senderClassName, senderMethodName);
        }

        public static void LogException(Exception ex, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            var message = $"EXCEPTION: {ex.GetFullExceptionErrorMessage()}";
            LogError(message, senderClassName, senderMethodName);
        }

        public static void LogFatal(string message, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            Log(message, LogLevel.Fatal, senderClassName, senderMethodName);
        }

        public static void LogObject(object obj, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string senderMethodName = default)
        {
            Log($"{obj.GetType().Name}:\n{obj.AsJson(pretty: true)}", LogLevel.Debug, senderClassName, senderMethodName);
        }

        #region EntityFramework

        private static bool isEntityFrameworkLoggingEnabled = true;

        public static void EnableEntityFrameworkLogging() => isEntityFrameworkLoggingEnabled = true;

        public static void DisableEntityFrameworkLoggins() => isEntityFrameworkLoggingEnabled = false;

        public static void LogEF(string value)
        {
            if (isEntityFrameworkLoggingEnabled)
                Log(value, LogLevel.Trace);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public static event Action<LogEntity> OnLog;

        /// <summary>
        /// Shared (static) logger level. If you create an instance of a logger
        /// you will have your own level.
        /// </summary>
        private static LogLevel __currentLoggerLevel = LogLevel.All;

        /// <summary>
        /// Set enabled or disabled
        /// </summary>
        public static bool LogToBasicLogWritter { get; set; } = false;

        /// <summary>
        /// There are tow ways of dynamically calculate caller class name:
        /// (1) Using a StackFrame at runtime. Very presice method but very slow.
        /// (2) Using a file name of a caller. Could be inaccurate if file have two or more classes.
        /// By default a caller filename used.
        /// </summary>
        public static bool UseCallerFilePath { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <param name="senderMethodName"></param>
        /// <param name="senderClassName"></param>
        public static void Log(string message, LogLevel logLevel = LogLevel.All, [CallerFilePath] string? senderClassName = default, [CallerMemberName] string? senderMethodName = default)
        {
            if (logLevel < __currentLoggerLevel) return; //nothing to log, level limit

            var dateLogEntityCreated = DateTime.UtcNow;

            senderClassName = UseCallerFilePath ? Path.GetFileNameWithoutExtension(senderClassName) : GetCallerClassName();

            if (OnLog != default)
            {
                var logEntity = new LogEntity
                {
                    Message = message,
                    DateTimeUtc = dateLogEntityCreated,
                    LogLevel = logLevel,
                    SenderMethodName = senderMethodName,
                    SenderClassName = senderClassName
                };

                OnLog(logEntity);
            }


            if (LogToBasicLogWritter)
                BasicLogWritter.WriteToLog(dateLogEntityCreated, message, logLevel, senderClassName, senderMethodName);

        }

        private static string GetCallerClassName()
        {
            var stackTrace = new StackTrace(skipFrames: 3);
            var frame = stackTrace.GetFrame(0);

            return frame?.GetMethod()?.DeclaringType?.Name ?? "UnknownClass";
        }

    }
}

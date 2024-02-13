using Ant0nRocket.Lib.Std20.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ant0nRocket.Lib.Std20.StandardImplimentations.Logging
{
    /// <summary>
    /// Simple file logger
    /// </summary>
    public class FileLogEntityHandler : ILogEntityHandler
    {
        private const string DEFAULT_LOG_FILE_EXTENSION = ".log";

        private readonly string _logDirectory = string.Empty;
        private readonly string _logFileExtension = DEFAULT_LOG_FILE_EXTENSION;

        private static readonly Dictionary<int, StreamWriter> __logWriters = new();

        private static int GetDayNumber(DateTime dateTime)
        {
            var dayNumber = dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
            return dayNumber;
        }

        public FileLogEntityHandler(string logDirectory, string logFileExtension = DEFAULT_LOG_FILE_EXTENSION)
        {
            _logDirectory = logDirectory;
            _logFileExtension = logFileExtension;

            try
            {
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                // Creating of a directory can cause exception.
                // So only when directory created - subscribe.
                SignalBus.OnSignalCode += SignalBus_OnSignalCode;
            }
            catch (Exception ex)
            {
                SignalBus.Send(ex);
            }
        }

        private void SignalBus_OnSignalCode(SignalCode signalCode)
        {
            // if we have SignalCode.ExitApp then go throgh all
            // loggers and close them
            if (signalCode == SignalCode.ExitApp)
            {
                foreach (var kvp in __logWriters)
                {
                    kvp.Value.Close();
                }
            }
        }

        public void Handle(LogEntity logEntity)
        {
            var logDayNumber = GetDayNumber(logEntity.DateTimeLocal); // use local date!

            var logWriterExists = __logWriters.ContainsKey(logDayNumber);

            if (logWriterExists == false)
            {
                var logFileName = Path.Combine(_logDirectory, $"{logDayNumber}{_logFileExtension}");

                try
                {
                    var logStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    var logStreamWriter = new StreamWriter(logStream, Encoding.UTF8) { AutoFlush = true };
                    __logWriters.Add(logDayNumber, logStreamWriter);
                    logWriterExists = true;
                }
                catch (Exception ex)
                {
                    SignalBus.Send(ex);
                    return;
                }
            }

            if (logWriterExists)
            {
                var logMessage =
                    $"{logEntity.DateTimeLocal:yyyy-MM-dd HH:mm:ss}|" +
                    $"{logEntity.LogLevel.ToString().ToUpper()}|" +
                    $"{logEntity.Message}";

                __logWriters[logDayNumber].WriteLine(logMessage);
            } // no logger - no log record
        }
    }
}

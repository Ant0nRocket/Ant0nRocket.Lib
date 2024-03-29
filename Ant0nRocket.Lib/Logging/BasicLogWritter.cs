﻿using System;
using System.IO;
using System.Text;

namespace Ant0nRocket.Lib.Logging
{
    [Obsolete]
    public static class BasicLogWritter
    {
        private static string logDirectory = "Logs";
        public static string LogDirectory
        {
            get => logDirectory;
            set
            {
                if (value.Trim().Length > 0)
                {
                    logDirectory = value;
                }
                else
                {
                    throw new ApplicationException($"{nameof(LogDirectory)} could not be empty string");
                }
            }
        }

        public static string LogFileNamePrefix { get; set; } = string.Empty;

        public static string CurrentFileName { get; private set; }

        public static event EventHandler<BasicLogWritterEventArgs> OnLogMessageWritten;

        private static FileStream logStream;
        private static StreamWriter logStreamWriter;

        public static void WriteToLog(DateTime date, string message, LogLevel level, string senderClassName, string senderMethodName)
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory); // yeahhh, I know, could throw an exception

            var logFileName = Path.Combine(
                LogDirectory,
                $"{LogFileNamePrefix ?? string.Empty}{date:yyyyMMdd}.log");

            // this will auto change logfile every day (even if program didn't shutdown)
            if (logStream == default || logFileName != CurrentFileName)
            {
                logStream?.Close();
                CurrentFileName = logFileName;
                logStream = new FileStream(CurrentFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                logStreamWriter = new StreamWriter(logStream, Encoding.UTF8) { AutoFlush = true };
            }

            var logMessage = $"{date:yyyy-MM-ddTHH:mm:ss:fff}|{level.ToString().ToUpper()}|" +
                $"{senderClassName}.{senderMethodName}|{message}";
            logStreamWriter.WriteLine(logMessage);

            if (OnLogMessageWritten != null)
            {
                var args = new BasicLogWritterEventArgs
                {
                    LogFileName = CurrentFileName,
                    LogFileStream = logStream,
                    LogMessage = logMessage
                };

                OnLogMessageWritten(null, args);
            }
        }
    }
}

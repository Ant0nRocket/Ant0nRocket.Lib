using System;
using System.IO;
using System.Text;

namespace Ant0nRocket.Lib.Std20.Logging
{
    public static class BasicLogWritter
    {
        private static string currentFileName;
        private static FileStream logStream;
        private static StreamWriter logStreamWriter;

        public static void WriteToLog(DateTime date, string message, LogLevel level, string senderClassName, string senderMethodName)
        {
            var fileName = GetFileNameForDate(date);

            // this will auto change logfile every day (even if program didn't shutdown)
            if (logStream == default || fileName != currentFileName) 
            {
                logStream?.Close();
                currentFileName = fileName;
                logStream = new FileStream(currentFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                logStreamWriter = new StreamWriter(logStream, Encoding.UTF8) { AutoFlush = true };
            }

            logStreamWriter.WriteLine($"{date:yyyy-MM-ddTHH:mm:ss:fff}|{level.ToString().ToUpper()}|{senderClassName}.{senderMethodName}|{message}");
        }

        private static string GetFileNameForDate(DateTime date)
        {
            var logDir = "Logs";
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
            return Path.Combine(logDir, $"{date:yyyyMMdd}.log");
        }
    }
}

using Ant0nRocket.Lib.Std20.Logging;

using NUnit.Framework;

using System;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class _Init
    {
        [Test]
        public void Initialize()
        {
            Logger.OnLog += Logger_OnLog;
        }

        private void Logger_OnLog(object? sender, (DateTime Date, string Message, LogLevel Level, string SenderClassName, string SenderMethodName) e)
        {
            BasicLogWritter.WriteToLog(e.Date, e.Message, e.Level, e.SenderClassName, e.SenderMethodName);
        }
    }
}

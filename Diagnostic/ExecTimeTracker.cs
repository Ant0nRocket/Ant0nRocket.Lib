using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.Diagnostics;

namespace Ant0nRocket.Lib.Std20.Diagnostic
{
    public class ExecTimeTracker
    {
        private readonly Logger logger;

        private readonly Stopwatch stopwatch = new();

        public ExecTimeTracker(Logger logger = default)
        {
            this.logger = logger ?? Logger.Create<ExecTimeTracker>();
        }

        public ExecTimeTracker Track(string description, Action action)
        {
            stopwatch.Restart();
            action();
            stopwatch.Stop();

            logger.LogTrace($"{description}: execution time = {stopwatch.ElapsedMilliseconds} ms");

            return this;
        }
    }
}

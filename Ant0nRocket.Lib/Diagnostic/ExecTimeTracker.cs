using Ant0nRocket.Lib.Logging;
using System;
using System.Diagnostics;

namespace Ant0nRocket.Lib.Diagnostic
{
    [Obsolete]
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

using Ant0nRocket.Lib.Logging;

namespace Ant0nRocket.Lib.Tests
{
    public abstract class _TestsBase
    {
        private static bool _isInitilized = false;

        public _TestsBase()
        {
            if (!_isInitilized)
            {
                BasicLogWritter.LogFileNamePrefix = "Ant0nRocket.Lib.Tests_";
                Logger.LogToBasicLogWritter = true;
                Logger.UseCallerFilePath = false;

                Logger.LogDebug("fucc you!");

                _isInitilized = true;
            }
        }
    }
}

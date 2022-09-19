using Ant0nRocket.Lib.Std20.Logging;
using Ant0nRocket.Lib.Std20.Tests.Serialization;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public abstract class _TestsBase
    {
        private static bool _isInitilized = false;

        public _TestsBase()
        {
            if (!_isInitilized)
            {
                Ant0nRocketLibConfig.RegisterJsonSerializer(new JsonSerializer());
                BasicLogWritter.LogFileNamePrefix = "Ant0nRocket.Lib.Std20.Tests_";
                Logger.LogToBasicLogWritter = true;
                _isInitilized = true;
            }
        }
    }
}

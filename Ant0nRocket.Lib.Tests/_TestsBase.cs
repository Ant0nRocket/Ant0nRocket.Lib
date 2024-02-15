using Ant0nRocket.Lib.Logging;
using Ant0nRocket.Lib.Tests.Serialization;

namespace Ant0nRocket.Lib.Tests
{
    public abstract class _TestsBase
    {
        private static bool _isInitilized = false;

        public _TestsBase()
        {
            if (!_isInitilized)
            {
                Ant0nRocketLibConfig.RegisterJsonSerializer(new JsonSerializer());
                BasicLogWritter.LogFileNamePrefix = "Ant0nRocket.Lib.Tests_";
                Logger.LogToBasicLogWritter = true;
                _isInitilized = true;
            }
        }
    }
}

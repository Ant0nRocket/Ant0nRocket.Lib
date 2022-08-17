using System.Runtime.CompilerServices;

using Ant0nRocket.Lib.Std20.Logging;

namespace Ant0nRocket.Lib.Std20.Testing
{
#nullable enable
    public abstract class TestBase
    {
        protected void LogStart([CallerMemberName] string? callerMemberName = default)
        {
            Logger.Log($"Test '{callerMemberName}' started", senderMethodName: callerMemberName);
        }

        protected void LogEnd([CallerMemberName] string? callerMemberName = default)
        {
            Logger.Log($"Test '{callerMemberName}' ended", senderMethodName: callerMemberName);
        }
    }
}

using Ant0nRocket.Lib.Std20.Logging;
using Ant0nRocket.Lib.Std20.StandardImplimentations.Logging;

namespace Ant0nRocket.Lib.Std20.TestsV3
{
    public class Tests
    {
        [Test]
        public void T001_RegisterLogEntityHandler()
        {
            Logger.RegisterLogEntityHandler(new FileLogEntityHandler(logDirectory: "Logs"));
            Logger.RegisterLogEntityHandler(new UdpLogEntityHandler("127.0.0.1", 49000));
        }

        [Test]
        public void T002_LogSomething()
        {
            Logger.Log("Something");
        }
    }
}
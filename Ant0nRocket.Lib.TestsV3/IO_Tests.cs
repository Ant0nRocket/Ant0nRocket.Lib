using Ant0nRocket.Lib.Logging;
using Ant0nRocket.Lib.StandardImplimentations.Logging;

namespace Ant0nRocket.Lib.TestsV6
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
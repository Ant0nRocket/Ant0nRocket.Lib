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
            BasicLogWritter.LogFileNamePrefix = "Ant0nRocket.Lib.Std20.Tests_";
            Logger.LogToBasicLogWritter = true;
        }
    }
}

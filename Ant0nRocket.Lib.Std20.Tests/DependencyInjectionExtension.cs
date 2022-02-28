
//using Ant0nRocket.Lib.Std20.DependencyInjection;

using Ant0nRocket.Lib.Std20.Tests.MockClasses;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using System;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class DependencyInjectionExtension
    {
        [Test]
        public void T001_ServiceCollection()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<BasicClass>();
        }
    }
}

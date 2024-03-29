﻿using Ant0nRocket.Lib.Reflection;

namespace Ant0nRocket.Lib.TestsV6
{
    internal class Utils_Tests
    {
        [Test]
        public void T001_ReflectionUtils_SetAppName()
        {
            var testName = "TestApp";
            ReflectionUtils.SetAppName(testName);
            Assert.That(ReflectionUtils.GetAppName(), Is.EqualTo(testName));
        }

        [Test]
        public void T002_ReflectionUtils_FindType()
        {
            var type = ReflectionUtils.FindType("SomeNamespace.SomeClass");
            Assert.That(type, Is.Null);
        }
    }
}

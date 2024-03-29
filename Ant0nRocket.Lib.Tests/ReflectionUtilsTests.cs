﻿using Ant0nRocket.Lib.Reflection;
using Ant0nRocket.Lib.Tests.MockAttributes;
using Ant0nRocket.Lib.Tests.MockClasses;
using Ant0nRocket.Lib.Tests.MockInterfaces;
using NUnit.Framework;

using System;
using System.Linq;

namespace Ant0nRocket.Lib.Tests
{
    internal class ReflectionUtilsTests : _TestsBase
    {
        [Test]
        public void T001_FindTypeAccrossAppDomain()
        {
            var typeName = typeof(BasicClass).FullName;
            var type = ReflectionUtils.FindTypeAccrossAppDomain(typeName!);
            Assert.IsNotNull(type);
        }

        [Test]
        public void T002_GetClassesThatImplementsInterface()
        {
            var classesThatImplementsIMockInterface = ReflectionUtils
                .GetClassesThatImplementsInterface<IMockInterface>();

            Assert.That(classesThatImplementsIMockInterface.Count() == 1);

            var testInstance = (IMockInterface?)Activator
                .CreateInstance(classesThatImplementsIMockInterface.First());

            Assert.That(testInstance is not null);
            Assert.That(testInstance?.SomeInt == 10);

            TestDelegate exceptionAction = () => ReflectionUtils.GetClassesThatImplementsInterface<BasicClass>();
            Assert.Throws<ArgumentException>(exceptionAction);
        }

        [Test]
        public void T003_GetAttribute()
        {
            var someCustumAttr = ReflectionUtils.GetAttribute<SomeCustomAttribute>(typeof(BasicClass));
            Assert.IsNotNull(someCustumAttr);

            someCustumAttr = ReflectionUtils.GetAttribute<SomeCustomAttribute>(typeof(string));
            Assert.IsNull(someCustumAttr);
        }
    }
}

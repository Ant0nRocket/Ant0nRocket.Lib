using Ant0nRocket.Lib.Std20.Reflection;
using Ant0nRocket.Lib.Std20.Tests.MockAttributes;
using Ant0nRocket.Lib.Std20.Tests.MockClasses;

using NUnit.Framework;

namespace Ant0nRocket.Lib.Std20.Tests
{
    internal class AttributeUtilsTests
    {
        [Test]
        public void T001_GetAttribute()
        {
            var basicClass = new BasicClass();
            var someCustumAttr = AttributeUtils.GetAttribute<SomeCustomAttribute>(basicClass.GetType());
            Assert.IsNotNull(someCustumAttr);
        }

        [Test]
        public void T002_FindTypeAccrossAppDomain()
        {
            var typeName = typeof(BasicClass).FullName;
            var type = AttributeUtils.FindTypeAccrossAppDomain(typeName);
            Assert.IsNotNull(type);
        }

        [Test]
        public void T003_GetTypesAccrossAppDomainWithAttribute()
        {
            var classesList = AttributeUtils.GetTypesAccrossAppDomainWithAttribute<SomeCustomAttribute>();
            Assert.That(classesList.Count == 2);
        }
    }
}

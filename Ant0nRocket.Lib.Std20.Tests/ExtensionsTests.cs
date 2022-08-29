using Ant0nRocket.Lib.Std20.Extensions;
using Ant0nRocket.Lib.Std20.Tests.Enums;
using Ant0nRocket.Lib.Std20.Tests.MockClasses;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class ExtensionsTests
    {
        [Test]
        public void ByteArray_StrictlyEquals()
        {
            var arrayA = new byte[] { 1, 2, 3, 4, 5 };
            var arrayB = new byte[] { 1, 2, 3, 4, 5 };
            Assert.AreNotSame(arrayB, arrayA);
            Assert.IsTrue(arrayA.StrictlyEquals(arrayB));

            byte[]? arrayC = default;
            Assert.IsFalse(arrayA.StrictlyEquals(arrayC));

            arrayB[0] = 2;
            Assert.IsFalse(arrayA.StrictlyEquals(arrayB));
        }

        [Test]
        public void Enum_GetPosibleValues()
        {
            var e = TestEnum.SomeFirstValue;
            var posibleValues = e.GetPosibleValues();
            Assert.That(posibleValues.Count() == 3);
            Assert.That(posibleValues.Any(v => v == TestEnum.SomeFirstValue));
            Assert.That(posibleValues.Any(v => v == TestEnum.SomeSecondValue));
            Assert.That(posibleValues.Any(v => v == TestEnum.SomeThirdValue));

            var s = e.GetValueDescriptionsDict();
            Assert.That(s[TestEnum.SomeFirstValue] == "First");
            Assert.That(s[TestEnum.SomeSecondValue] == "Second");
            Assert.That(s[TestEnum.SomeThirdValue] == "SomeThirdValue");
        }

        [Test]
        public void HashSet_AddSecure()
        {
            var h = new HashSet<string>();
            Assert.True(h.AddSecure("1"));
            Assert.False(h.AddSecure("1"));
            Assert.True(h.AddSecure("2"));
            Assert.That(h.Count == 2); // 1 and 2
        }

        [Test]
        public void Object_GetPropertyValue()
        {
            var inst = new StoreClass();
            var pv = inst.GetPropertyValue("TestString");

            Assert.That(pv is string s && s == "Hello");
            Assert.Throws(typeof(ArgumentException), () => pv.GetPropertyValue(null));
            Assert.Throws(typeof(MissingMemberException), () => pv.GetPropertyValue("BlaBlaBla"));
        }

        [Test]
        public void String_Left_And_Right()
        {
            var str = "Hello! My name is Anton!";
            Assert.AreEqual("Hello", str.Left(5));
            Assert.AreEqual("Anton!", str.Right(6));

            str = "ooops";
            Assert.AreEqual(str, str.Left(30));
            Assert.AreEqual(str, str.Right(30));
        }

        [Test]
        public void String_Skip()
        {
            var testStr = "HHello";
            Assert.That(testStr.SkipChars(1) == "Hello");
        }

        [Test]
        public void String_FromLatest()
        {
            var result = "Some.Test.String".FromLatest('r');
            Assert.AreEqual("ing", result);

            result = "123456789".FromLatest('x');
            Assert.That(result == "123456789");

            result = "Ant0nRocket.Lib.Std20.Tests".FromLatest('.');
            Assert.AreEqual("Tests", result);
        }

    }
}

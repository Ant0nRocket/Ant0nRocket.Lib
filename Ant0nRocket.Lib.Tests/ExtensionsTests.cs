using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.Tests.MockClasses;
using Ant0nRocket.Lib.Tests;
using Ant0nRocket.Lib.Tests.Enums;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class ExtensionsTests : _TestsBase
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
        public void Exception_GetFullExceptionErrorMessage()
        {
            ApplicationException testex = null!;
            Assert.AreEqual(string.Empty, testex.GetFullExceptionErrorMessage());

            var ex1 = new ApplicationException("ex1");
            var ex2 = new ApplicationException("ex2", ex1);
            var ex3 = new ApplicationException("ex3", ex2);
            Assert.AreEqual("ex3 -> ex2 -> ex1", ex3.GetFullExceptionErrorMessage());
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
        public void Object_SetPropertyValue()
        {
            var obj = new BasicClass();
            obj.SetPropertyValue("SomeInt", 300);
            Assert.AreEqual(300, obj.SomeInt);

            Assert.Throws<InvalidOperationException>(() => obj.SetPropertyValue("some other fields", null));
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

            Assert.That(testStr.SkipChars(-1) == testStr);

            string? testStr2 = default;
            Assert.That(testStr2.SkipChars(1) == string.Empty);
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

        [Test]
        public void String_GetFirstWord()
        {
            var sentance1 = "Hello from simple string";
            Assert.AreEqual("Hello", sentance1.GetFirstWord());

            sentance1 = "some\tother string";
            Assert.AreEqual("some", sentance1.GetFirstWord());

            sentance1 = "word";
            Assert.AreEqual("word", sentance1.GetFirstWord());
        }

    }
}

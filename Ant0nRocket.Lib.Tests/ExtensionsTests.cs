using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.Tests.MockClasses;
using Ant0nRocket.Lib.Tests.Enums;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Tests
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
        public void String_ComputeHashMD5()
        {
            Assert.AreEqual(
                "0fd3dbec9730101bff92acc820befc34",
                "Test string".ComputeMD5Hash().ToHexString());

            Assert.AreEqual(
                "d41d8cd98f00b204e9800998ecf8427e",
                string.Empty.ComputeMD5Hash().ToHexString());

            Assert.AreEqual(
                "a5d9de4499c5c850c1c69695c7b5b88d",
                "Салют!".ComputeMD5Hash().ToHexString());
        }

        [Test]
        public void String_ComputeHashSHA256()
        {
            Assert.AreEqual(
                "a3e49d843df13c2e2a7786f6ecd7e0d184f45d718d1ac1a8a63e570466e489dd",
                "Test string".ComputeSHA256Hash().ToHexString());

            Assert.AreEqual(
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                string.Empty.ComputeSHA256Hash().ToHexString());

            Assert.AreEqual(
                "5229152262a70bb4e7882e3a442c1f91556be1d986a385053d557f3d8aa9a9da",
                "Салют!".ComputeSHA256Hash().ToHexString());

            // There is no need to check SHA-512 'cause the only difference in
            // algorythm is a hasher implementation (both are system).
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

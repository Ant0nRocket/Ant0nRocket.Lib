using Ant0nRocket.Lib.Std20.Extensions;
using Ant0nRocket.Lib.Std20.Tests.Enums;
using NUnit.Framework;
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
        public void Enum_GetPosibleValues()
        {
            var e = TestEnum.SomeFirstValue;
            var posibleValues = e.GetPosibleValues();
            Assert.That(posibleValues.Count() == 3);
            Assert.That(posibleValues.Any(v => v == TestEnum.SomeFirstValue));
            Assert.That(posibleValues.Any(v => v == TestEnum.SomeSecondValue));
            Assert.That(posibleValues.Any(v => v == TestEnum.SomeThirdValue));

            var s = e.GetDescriptions();
        }
    }
}

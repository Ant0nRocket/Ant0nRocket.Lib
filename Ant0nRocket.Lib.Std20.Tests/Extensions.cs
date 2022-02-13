using Ant0nRocket.Lib.Std20.Extensions;

using NUnit.Framework;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class Extensions
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
    }
}

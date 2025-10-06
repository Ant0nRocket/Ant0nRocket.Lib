using Ant0nRocket.Lib.Extensions;
using NUnit.Framework;
using System;
using System.Security.Cryptography;

namespace Ant0nRocket.Lib.Tests.Extensions
{
    [TestFixture]
    public class ByteArrayExtensionsTests
    {
        #region ComputeHash Tests

        [Test]
        public void ComputeHash_WithNullSource_ReturnsHashOfEmptyArray()
        {
            byte[]? source = null;
            var expectedHash = SHA256.HashData(Array.Empty<byte>());

            var result = source.ComputeHash();

            Assert.That(result, Is.EqualTo(expectedHash));
        }

        [Test]
        public void ComputeHash_WithCustomData_ReturnsCorrectHash()
        {
            var source = new byte[] { 1, 2, 3, 4, 5 };
            var expectedHash = SHA256.HashData(source);

            var result = source.ComputeHash();

            Assert.That(result, Is.EqualTo(expectedHash));
        }

        [Test]
        public void ComputeHash_WithCustomHashAlgorithm_UsesProvidedAlgorithm()
        {
            var source = new byte[] { 1, 2, 3 };
            using var sha1 = SHA1.Create();
            var expectedHash = sha1.ComputeHash(source);

            var result = source.ComputeHash(sha1);

            Assert.That(result, Is.EqualTo(expectedHash));
        }

        [Test]
        public void ComputeHash_WithNullHashAlgorithm_UsesSHA256()
        {
            var source = new byte[] { 1, 2, 3 };
            HashAlgorithm? algorithm = null;
            var expectedHash = SHA256.HashData(source);

            var result = source.ComputeHash(algorithm);

            Assert.That(result, Is.EqualTo(expectedHash));
        }

        #endregion

        #region StrictlyEquals Tests

        [Test]
        public void StrictlyEquals_WithIdenticalArrays_ReturnsTrue()
        {
            var arrayA = new byte[] { 1, 2, 3, 4, 5 };
            var arrayB = new byte[] { 1, 2, 3, 4, 5 };

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.True);
        }

        [Test]
        public void StrictlyEquals_WithDifferentArrays_ReturnsFalse()
        {
            var arrayA = new byte[] { 1, 2, 3, 4, 5 };
            var arrayB = new byte[] { 1, 2, 3, 4, 6 };

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.False);
        }

        [Test]
        public void StrictlyEquals_WithDifferentLengths_ReturnsFalse()
        {
            var arrayA = new byte[] { 1, 2, 3 };
            var arrayB = new byte[] { 1, 2, 3, 4 };

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.False);
        }

        [Test]
        public void StrictlyEquals_WithFirstArrayNull_ReturnsFalse()
        {
            byte[]? arrayA = null;
            var arrayB = new byte[] { 1, 2, 3 };

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.False);
        }

        [Test]
        public void StrictlyEquals_WithSecondArrayNull_ReturnsFalse()
        {
            var arrayA = new byte[] { 1, 2, 3 };
            byte[]? arrayB = null;

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.False);
        }

        [Test]
        public void StrictlyEquals_WithBothArraysNull_ReturnsFalse()
        {
            byte[]? arrayA = null;
            byte[]? arrayB = null;

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.False);
        }

        [Test]
        public void StrictlyEquals_WithEmptyArrays_ReturnsTrue()
        {
            var arrayA = Array.Empty<byte>();
            var arrayB = Array.Empty<byte>();

            var result = arrayA.StrictlyEquals(arrayB);

            Assert.That(result, Is.True);
        }

        #endregion

        #region ToHexString Tests

        [Test]
        public void ToHexString_WithNullArray_ReturnsEmptyString()
        {
            byte[]? array = null;

            var result = array.ToHexString();

            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ToHexString_WithEmptyArray_ReturnsEmptyString()
        {
            var array = Array.Empty<byte>();

            var result = array.ToHexString();

            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ToHexString_WithData_ReturnsCorrectHexString()
        {
            var array = new byte[] { 0xAB, 0xCD, 0xEF, 0x12 };

            var result = array.ToHexString();

            Assert.That(result, Is.EqualTo("abcdef12"));
        }

        [Test]
        public void ToHexString_WithUpperCase_ReturnsUpperCase()
        {
            var array = new byte[] { 0xAB, 0xCD, 0xEF };

            var result = array.ToHexString(resultInLowerCase: false);

            Assert.That(result, Is.EqualTo("ABCDEF"));
        }

        [Test]
        public void ToHexString_WithDashes_ReturnsStringWithDashes()
        {
            var array = new byte[] { 0xAB, 0xCD, 0xEF };

            var result = array.ToHexString(removeDashes: false);

            Assert.That(result, Is.EqualTo("ab-cd-ef"));
        }

        [Test]
        public void ToHexString_WithDashesAndUpperCase_ReturnsUpperCaseWithDashes()
        {
            var array = new byte[] { 0xAB, 0xCD, 0xEF };

            var result = array.ToHexString(resultInLowerCase: false, removeDashes: false);

            Assert.That(result, Is.EqualTo("AB-CD-EF"));
        }

        [Test]
        public void ToHexString_WithSingleByte_ReturnsCorrectHex()
        {
            var array = new byte[] { 0xFF };

            var result = array.ToHexString();

            Assert.That(result, Is.EqualTo("ff"));
        }

        [Test]
        public void ToHexString_WithAllZeroBytes_ReturnsCorrectHex()
        {
            var array = new byte[] { 0x00, 0x00, 0x00 };

            var result = array.ToHexString();

            Assert.That(result, Is.EqualTo("000000"));
        }

        #endregion
    }
}

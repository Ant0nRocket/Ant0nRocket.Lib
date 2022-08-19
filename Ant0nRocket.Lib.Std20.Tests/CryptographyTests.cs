using Ant0nRocket.Lib.Std20.Cryptography;
using Ant0nRocket.Lib.Std20.Extensions;

using NUnit.Framework;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class CryptographyTests
    {
        [Test]
        public void T002_Hasher_CalculateHash_SHA256()
        {
            Assert.AreEqual(
                "a3e49d843df13c2e2a7786f6ecd7e0d184f45d718d1ac1a8a63e570466e489dd",
                Hasher.ComputeHash("Test string").ToHexString());

            Assert.AreEqual(
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                Hasher.ComputeHash(string.Empty).ToHexString());

            Assert.AreEqual(
                "5229152262a70bb4e7882e3a442c1f91556be1d986a385053d557f3d8aa9a9da",
                Hasher.ComputeHash("Салют!").ToHexString());

            // There is no need to check SHA-512 'cause the only difference in
            // algorythm is a hasher implementation (both are system).
        }
    }
}
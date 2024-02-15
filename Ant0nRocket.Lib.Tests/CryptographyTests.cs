using Ant0nRocket.Lib.Cryptography;
using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.Tests;
using NUnit.Framework;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class CryptographyTests : _TestsBase
    {
        [Test]
        public void T001_Hasher_CalculateHash_SHA256()
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

        [Test]
        public void T002_Hasher_CalculateHash_MD5()
        {
            Assert.AreEqual(
                "0fd3dbec9730101bff92acc820befc34",
                Hasher.ComputeHash("Test string", hashAlgorithmType: HashAlgorithmType.MD5).ToHexString());

            Assert.AreEqual(
                "d41d8cd98f00b204e9800998ecf8427e",
                Hasher.ComputeHash(string.Empty, hashAlgorithmType: HashAlgorithmType.MD5).ToHexString());

            Assert.AreEqual(
                "a5d9de4499c5c850c1c69695c7b5b88d",
                Hasher.ComputeHash("Салют!", hashAlgorithmType: HashAlgorithmType.MD5).ToHexString());
        }
    }
}
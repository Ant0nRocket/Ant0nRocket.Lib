using Ant0nRocket.Lib.Std20.Cryptography;

using NUnit.Framework;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class T001_Cryptography
    {
        [Test]
        public void T001_Hasher_CalculateHash_SHA512()
        {
            var hash1 = Hasher.CalculateHash("Test string", hashType: HashAlgorithmType.SHA512);
            Assert.AreEqual("811aa0c53c0039b6ead0ca878b096eed1d39ed873fd2d2d270abfb9ca620d3ed561c565d6dbd1114c323d38e3f59c00df475451fc9b30074f2abda3529df2fa7".ToUpper(), hash1.ToUpper());
        }

        [Test]
        public void T002_Hasher_CalculateHash_SHA256()
        {
            var hash1 = Hasher.CalculateHash("Test string");
            Assert.AreEqual("a3e49d843df13c2e2a7786f6ecd7e0d184f45d718d1ac1a8a63e570466e489dd".ToUpper(), hash1.ToUpper());
        }
    }
}
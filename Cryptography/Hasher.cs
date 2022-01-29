using System;
using System.Security.Cryptography;
using System.Text;

namespace Ant0nRocket.Lib.Std20.Cryptography
{
    public static class Hasher
    {
        /// <summary>
        /// Performs SHA-256 or SHA-512 hashing. All other hash algorithms are deprecated.<br />
        /// By default SHA-512 hash will be used.
        /// </summary>
        public static byte[] CalculateHash(byte[] buffer, HashAlgorithmType hashType = HashAlgorithmType.SHA512)
        {
            HashAlgorithm hashAlgorithm = hashType == HashAlgorithmType.SHA256 ? 
                new SHA256Managed() : new SHA512Managed();

            return hashAlgorithm.ComputeHash(buffer);
        }

        public static string CalculateHash(string value, string salt = default, HashAlgorithmType hashType = HashAlgorithmType.SHA512)
        {
            var bytes = Encoding.UTF8.GetBytes(value + salt);
            var result = CalculateHash(bytes, hashType);
            return Convert.ToBase64String(result);
        }
    }
}

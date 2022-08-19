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
        [Obsolete("Use ComputeHash instead")]
        public static byte[] CalculateHash(byte[] buffer, HashAlgorithmType hashType = HashAlgorithmType.SHA256)
        {
            return ComputeHash(buffer, hashType);
        }

        [Obsolete("Use ComputeHash instead")]
        public static string CalculateHash(string value, string salt = default, HashAlgorithmType hashType = HashAlgorithmType.SHA256)
        {
            var bytes = Encoding.UTF8.GetBytes(salt == default ? value : value + salt);
            var result = ComputeHash(bytes, hashType);
            return BitConverter.ToString(result).Replace("-", string.Empty);
        }

        /// <summary>
        /// Performs SHA-256 or SHA-512 hashing. All other hash algorithms are deprecated.<br />
        /// By default SHA-512 hash will be used.
        /// </summary>
        public static byte[] ComputeHash(byte[] buffer, HashAlgorithmType hashType = HashAlgorithmType.SHA256)
        {
            HashAlgorithm hashAlgorithm = hashType == HashAlgorithmType.SHA256 ?
                new SHA256Managed() : new SHA512Managed();

            return hashAlgorithm.ComputeHash(buffer);
        }

        /// <summary>
        /// Calculates hash of a string <paramref name="value"/>.<br />
        /// In addition, password salt could be added with <paramref name="salt"/>.<br />
        /// Default hash algorithm is SHA-256. Use <paramref name="hashType"/> to change it.<br />
        /// Default encoding is <see cref="Encoding.Default"/>.
        /// </summary>
        /// <returns>
        /// Returns byte array of hash. Use extension <see cref="Extensions.StringExtensions"/>
        /// </returns>
        public static byte[] ComputeHash(
            string value,
            string salt = default,
            HashAlgorithmType hashType = HashAlgorithmType.SHA256,
            Encoding encoding = default)
        {
            encoding ??= Encoding.Default;
            var bytes = encoding.GetBytes(salt == default ? value : value + salt);
            var result = ComputeHash(bytes, hashType);
            return result;

        }
    }
}

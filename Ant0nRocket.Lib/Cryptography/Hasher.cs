using System;
using System.Security.Cryptography;
using System.Text;

namespace Ant0nRocket.Lib.Cryptography
{
    /// <summary>
    /// Class for calculating hashes.
    /// </summary>
    public static class Hasher
    {
        /// <summary>
        /// Performs SHA-256, SHA-512 or MD5 hashing. All other hash algorithms are deprecated.<br />
        /// By default SHA-256 hash will be used.
        /// </summary>
        public static byte[] ComputeHash(byte[] buffer, HashAlgorithmType hashAlgorithmType = HashAlgorithmType.SHA256)
        {
            HashAlgorithm hashAlgorithm = hashAlgorithmType switch
            {
                HashAlgorithmType.SHA256 => new SHA256Managed(),
                HashAlgorithmType.SHA512 => new SHA512Managed(),
                HashAlgorithmType.MD5 => MD5.Create(),
                _ => throw new ArgumentOutOfRangeException(nameof(hashAlgorithmType))
            };

            return hashAlgorithm.ComputeHash(buffer);
        }

        /// <summary>
        /// Calculates hash of a string <paramref name="value"/>.<br />
        /// In addition, password salt could be added with <paramref name="salt"/>.<br />
        /// Default hash algorithm is SHA-256. Use <paramref name="hashAlgorithmType"/> to change it.<br />
        /// Default encoding is <see cref="Encoding.Default"/>.
        /// </summary>
        /// <returns>
        /// Returns byte array of hash. Use extension <see cref="Extensions.StringExtensions"/>
        /// </returns>
        public static byte[] ComputeHash(
            string value,
            string? salt = default,
            HashAlgorithmType hashAlgorithmType = HashAlgorithmType.SHA256,
            Encoding? encoding = default)
        {
            encoding ??= Encoding.Default;
            var bytes = encoding.GetBytes(salt == default ? value : value + salt ?? string.Empty);
            var result = ComputeHash(bytes, hashAlgorithmType);
            return result;

        }
    }
}

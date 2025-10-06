using System;
using System.Security.Cryptography;

namespace Ant0nRocket.Lib.Extensions
{
    /// <summary>
    /// Collection of extensions of byte arrays
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Computes a hash with given <paramref name="hashAlgorithm"/>.
        /// If <paramref name="source"/> is null then empty array hash will be computed.
        /// If <paramref name="hashAlgorithm"/> is null then SHA256 algo will be used.
        /// </summary>
        public static byte[] ComputeHash(this byte[] source, HashAlgorithm? hashAlgorithm = default)
        {
            return (hashAlgorithm ?? SHA256.Create()).ComputeHash(source ?? []);
        }

        /// <summary>
        /// Compares all bytes in two byte array and returns equal state
        /// </summary>
        public static bool StrictlyEquals(this byte[] arrayA, byte[] arrayB)
        {
            if (arrayA == null || arrayB == null) return false;
            if (arrayA.Length != arrayB.Length) return false;

            return arrayA.AsSpan().SequenceEqual(arrayB);
        }

        /// <summary>
        /// Converts byte array into hex string.<br />
        /// If null or empty array passed - string.Empty will be returned.
        /// </summary>
        public static string ToHexString(this byte[] array, bool resultInLowerCase = true, bool removeDashes = true)
        {
            if (array == null || array.Length == 0)
                return string.Empty;

            if (!removeDashes)
            {
                var result = BitConverter.ToString(array);
                return resultInLowerCase ? result.ToLower() : result.ToUpper();
            }

            // More efficient for large arrays without dashes
            return string.Create(array.Length * 2, array, (span, bytes) =>
            {
                var format = resultInLowerCase ? "x2" : "X2";
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i].TryFormat(span.Slice(i * 2, 2), out _, format);
                }
            });
        }
    }
}

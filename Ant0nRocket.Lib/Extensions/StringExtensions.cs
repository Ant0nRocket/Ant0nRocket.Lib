﻿using System.Security.Cryptography;
using System.Text;

namespace Ant0nRocket.Lib.Extensions
{
    /// <summary>
    /// Extensions for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Calculates hash bytes (using SHA256 algorythm) of a <paramref name="source"/>.
        /// </summary>
        public static byte[] ComputeSHA256Hash(this string? source, Encoding? encoding = default)
        {
            using var hasher = SHA256.Create();
            return ComputeStringHash(hasher, encoding, source);
        }

        /// <summary>
        /// Calculates hash bytes (using SHA512 algorythm) of a <paramref name="source"/>.
        /// </summary>
        public static byte[] ComputeSHA512Hash(this string? source, Encoding? encoding = default)
        {
            using var hasher = SHA512.Create();
            return ComputeStringHash(hasher, encoding, source);
        }

        /// <summary>
        /// Calculates hash bytes (using MD5 algorythm) of a <paramref name="source"/>.
        /// </summary>
        public static byte[] ComputeMD5Hash(this string? source, Encoding? encoding = default)
        {
            using var hasher = MD5.Create();
            return ComputeStringHash(hasher, encoding, source);
        }

        private static byte[] ComputeStringHash(HashAlgorithm hashAlgorithm, Encoding? encoding, string? source)
        {
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(source ?? string.Empty);
            return hashAlgorithm.ComputeHash(bytes);
        }

        /// <summary>
        /// Returnes left N symbols
        /// </summary>
        public static string Left(this string value, int charsCount)
        {
            if (value.Length <= charsCount)
                return value;
            return value.Substring(0, charsCount);
        }

        /// <summary>
        /// Returnes right N symbols
        /// </summary>
        public static string Right(this string value, int charsCount)
        {
            if (value.Length <= charsCount)
                return value;
            return value.Substring(value.Length - charsCount);
        }

        /// <summary>
        /// Skipes <paramref name="charsCount"/> of original string and returnes result.
        /// If <paramref name="charsCount"/> less or equal zero - <paramref name="value"/> returned.
        /// If <paramref name="value"/> is null - <see cref="string.Empty"/> returned.
        /// </summary>
        public static string SkipChars(this string? value, int charsCount)
        {
            if (charsCount <= 0 || charsCount > value?.Length)
                return value ?? string.Empty;
            return value?.Substring(charsCount) ?? string.Empty;
        }

        /// <summary>
        /// Returnes a substring from last found <paramref name="ch"/> to
        /// the end of the origin.
        /// <code>
        /// "Some.Test.String".FromLatest('r'); // -> "ing"
        /// "123456789".FromLatest('x'); // "123456789", entire origin
        /// </code>
        /// If <paramref name="ch"/> not found - origin returned.
        /// </summary>
        public static string FromLatest(this string value, char ch)
        {
            for (var i = value.Length - 1; i >= 0; i--)
                if (value[i] == ch)
                    return value.Substring(i + 1);
            return value;
        }

        /// <summary>
        /// Chars that split the words in sentances.
        /// </summary>
        public static readonly char[] SPACE_CHARS = { ' ', '\t', '\r', '\n', '\f' };

        /// <summary>
        /// Will return first word from specified string (or whole string if no other words).<br />
        /// Words separators are <see cref="SPACE_CHARS"/>.
        /// </summary>
        public static string GetFirstWord(this string? value)
        {
            if (value == default)
                return string.Empty;

            for (var valueIndex = 0; valueIndex < value.Length; valueIndex++)
            {
                for (var spaceCharsIndex = 0; spaceCharsIndex < SPACE_CHARS.Length; spaceCharsIndex++)
                {
                    // We simply compare each char of value with each char of SPACE_CHARS
                    // If some is space - return substring upto current position.
                    if (value[valueIndex] == SPACE_CHARS[spaceCharsIndex])
                        return value.Substring(0, valueIndex);
                }
            }

            // ... no space chars were found - return origin value
            return value;
        }
    }
}

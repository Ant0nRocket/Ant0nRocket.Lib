using System;

namespace Ant0nRocket.Lib.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Compares all bytes in two byte array and returns equal state
        /// </summary>
        public static bool StrictlyEquals(this byte[] arrayA, byte[] arrayB)
        {
            if (arrayA == null || arrayB == null) return false;
            if (arrayA.Length != arrayB.Length) return false;
            for (int i = 0; i < arrayA.Length; i++)
                if (arrayA[i] != arrayB[i]) return false;
            return true;
        }

        /// <summary>
        /// Converts byte array into hex string.<br />
        /// If null or empty array passed - string.Empty will be returned.
        /// </summary>
        public static string ToHexString(this byte[] array, bool resultInLowerCase = true, bool removeDashes = true)
        {
            if (array == null || array.Length == 0) return string.Empty;
            var arrayAsString = BitConverter.ToString(array);
            arrayAsString = resultInLowerCase ? arrayAsString.ToLower() : arrayAsString.ToUpper();
            arrayAsString = removeDashes ? arrayAsString.Replace("-", string.Empty) : arrayAsString;
            return arrayAsString;
        }
    }
}

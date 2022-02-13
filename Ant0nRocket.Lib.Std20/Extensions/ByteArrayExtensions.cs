namespace Ant0nRocket.Lib.Std20.Extensions
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
    }
}

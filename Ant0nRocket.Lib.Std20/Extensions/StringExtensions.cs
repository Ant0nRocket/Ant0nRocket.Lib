namespace Ant0nRocket.Lib.Std20.Extensions
{
    public static class StringExtensions
    {
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
        /// Skipes <paramref name="charsCount"/> of original string and returnes result
        /// </summary>
        public static string SkipChars(this string value, int charsCount)
        {
            if (charsCount <= 0) charsCount = 0;
            return value.Substring(charsCount);
        }
    }
}

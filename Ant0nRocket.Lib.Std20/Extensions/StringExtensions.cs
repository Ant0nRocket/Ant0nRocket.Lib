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
    }
}

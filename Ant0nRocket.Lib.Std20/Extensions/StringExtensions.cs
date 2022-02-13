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
    }
}

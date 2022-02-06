namespace Ant0nRocket.Lib.Std20.Extensions
{
    public static class StringExtensionMethods
    {
        public static string Left(this string value, int charsCount)
        {
            if (value.Length <= charsCount)
                return value;
            return value.Substring(0, charsCount - 1);
        }

        public static string Right(this string value, int charsCount)
        {
            if (value.Length <= charsCount)
                return value;
            return value.Substring(value.Length - charsCount - 1);
        }
    }
}

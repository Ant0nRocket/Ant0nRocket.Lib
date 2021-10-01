using System;

namespace Ant0nRocket.Lib.Std20.Extensions
{
    public static class DateTimeExtensionMethods
    {
        /// <summary>
        /// 20.01.2021 12:45:00 -> 20.01.2021 23:59:59.999
        /// </summary>
        public static DateTime EndOfTheDay(this DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;
            return new DateTime(year, month, day, 23, 59, 59, 999);
        }
    }
}

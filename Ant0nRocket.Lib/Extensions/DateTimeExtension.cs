using System;

namespace Ant0nRocket.Lib.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 20.01.2021 12:45:00 -> 20.01.2021 00:00:00.000
        /// </summary>
        public static DateTime StartOfTheDay(this DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;
            return new DateTime(year, month, day, 0, 0, 0, 0);
        }

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

namespace TDOC.Common.Server.Http.Headers
{
    public class LastModifiedHeader
    {
        private static string GetDayName(DateTime date)
        {
            string dayName = date.DayOfWeek switch
            {
                DayOfWeek.Monday => "Mon",
                DayOfWeek.Tuesday => "Tue",
                DayOfWeek.Wednesday => "Wed",
                DayOfWeek.Thursday => "Thu",
                DayOfWeek.Friday => "Fri",
                DayOfWeek.Saturday => "Sat",
                DayOfWeek.Sunday => "Sun",
                _ => throw new NotSupportedException($"DayOfWeek \"{date.DayOfWeek}\" is not supported.")
            };

            return dayName;
        }

        private static string GetMonthName(DateTime date)
        {
            string monthName = date.Month switch
            {
                1 => "Jan",
                2 => "Feb",
                3 => "Mar",
                4 => "Apr",
                5 => "May",
                6 => "Jun",
                7 => "Jul",
                8 => "Aug",
                9 => "Sep",
                10 => "Oct",
                11 => "Nov",
                12 => "Dec",
                _ => throw new NotSupportedException($"Month \"{date.Month}\" is not supported.")
            };

            return monthName;
        }

        /// <summary>
        /// Encodes the specified date/time for use in a last-modified header.
        /// </summary>
        /// <param name="lastModifiedGMT">The date and time in GMT.</param>
        /// <returns>The encoded date/time.</returns>
        public static string GetValue(DateTime lastModifiedGMT)
        {
            const string lastModifiedTemplate = "{0}, {1:00} {2} {3:0000} {4:00}:{5:00}:{6:00} GMT";

            return string.Format(lastModifiedTemplate,
                GetDayName(lastModifiedGMT),
                lastModifiedGMT.Day,
                GetMonthName(lastModifiedGMT),
                lastModifiedGMT.Year,
                lastModifiedGMT.Hour,
                lastModifiedGMT.Minute,
                lastModifiedGMT.Second
            );
        }
    }
}
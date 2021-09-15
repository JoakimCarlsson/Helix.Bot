using System;

namespace Helix.Bot.Helpers.Formatters
{
    public static class DateFormatter
    {
        public static string ShortTime(DateTime time) => $"<t:{UnixTimestamp(time)}:t>";
        public static string LongTime(DateTime time) => $"<t:{UnixTimestamp(time)}:T>";
        public static string ShortDate(DateTime time) => $"<t:{UnixTimestamp(time)}:d>";
        public static string ShortDateAndTime(DateTime time) => $"<t:{UnixTimestamp(time)}:f>";
        public static string LongDateAndTime(DateTime time) => $"<t:{UnixTimestamp(time)}:F>";
        public static string RelativeTime(DateTime time) => $"<t:{UnixTimestamp(time)}:R>";

        private static long UnixTimestamp(DateTime time)
        {
            DateTimeOffset dateTimeOffset = time;
            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}

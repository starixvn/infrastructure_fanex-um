namespace ONEbook.UM.Api.Extensions
{
    using System;

    public static class DateTimeExtension
    {
        public static string ToGMTString(this DateTime dateTime, int GMT)
        {
            var signal = GMT > 0 ? "+" : "-";

            return $"{dateTime} {signal}{GMT}:00";
        }
    }
}
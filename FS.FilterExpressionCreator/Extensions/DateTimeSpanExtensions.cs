using Chronic.Core;
using FS.FilterExpressionCreator.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FS.FilterExpressionCreator.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeSpanExtensions
    {
        /// <summary>
        /// Tries to convert a string to date time span.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="dateTimeSpan">The parsed date time span.</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static bool TryConvertStringToDateTimeSpan(this string value, DateTimeOffset now, out DateTimeSpan dateTimeSpan, CultureInfo cultureInfo = null)
        {
            dateTimeSpan = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            if (value == null)
                return false;

            if (TryConvertDateTimeSpanFormattedString(value, cultureInfo, out dateTimeSpan))
                return true;
            if (TryConvertRoundTripFormattedString(value, cultureInfo, out dateTimeSpan))
                return true;
            if (TryConvertChronicSpanFormattedString(value, now, out dateTimeSpan))
                return true;
            if (TryConvertUnknownFormattedString(value, cultureInfo, out dateTimeSpan))
                return true;

            return false;
        }

        /// <summary>
        /// Expands a date to a time span. 2000-01-01 extends to 2020-12-31, 2020-02-01 extends to 2020-02-28 and so on.
        /// </summary>
        /// <param name="start">The date/time to expand.</param>
        public static DateTimeSpan ConvertToDateTimeSpan(this DateTimeOffset start)
        {
            DateTimeOffset end;
            if (start.Second != 0)
                end = start.AddSeconds(1);
            else if (start.Minute != 0)
                end = start.AddMinutes(1);
            else if (start.Hour != 0)
                end = start.AddHours(1);
            else if (start.Day != 1)
                end = start.AddDays(1);
            else if (start.Month != 1)
                end = start.AddMonths(1);
            else
                end = start.AddYears(1);

            return new DateTimeSpan(start, end);
        }

        private static bool TryConvertDateTimeSpanFormattedString(string value, IFormatProvider cultureInfo, out DateTimeSpan dateTimeSpan)
        {
            dateTimeSpan = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            var startAndEnd = Regex.Match(value, "^(?<start>.+?)_(?<end>.+)$");
            if (!startAndEnd.Success)
                return false;

            var startParsed = TryConvertRoundTripFormattedString(startAndEnd.Groups["start"].Value, cultureInfo, out var start);
            var endParsed = TryConvertRoundTripFormattedString(startAndEnd.Groups["end"].Value, cultureInfo, out var end);
            if (!startParsed || !endParsed)
                return false;

            dateTimeSpan = new DateTimeSpan(start.Start, end.Start);
            return true;
        }

        private static bool TryConvertRoundTripFormattedString(string value, IFormatProvider cultureInfo, out DateTimeSpan dateTimeSpan)
        {
            dateTimeSpan = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            const string offsetPattern = @"^(?<datetime>.+?)(?<offset>Z|[\+\-]\d{1,2}:\d{1,2})?$";
            const string dateTimePattern = @"^(?<year>\d\d\d\d)(\D(?<month>\d\d))?(\D(?<day>\d\d))?(\D(?<hour>\d\d))?(\D(?<minute>\d\d))?(\D(?<second>\d\d))?(\D(?<millisecond>\d+))?$";

            var dateTimeAndOffset = Regex.Match(value, offsetPattern);
            if (!dateTimeAndOffset.Success)
                return false;

            var dateTime = dateTimeAndOffset.Groups["datetime"].Value;
            var offset = dateTimeAndOffset.Groups["offset"].Value;

            var dateAndTime = Regex.Match(dateTime, dateTimePattern);
            if (!dateAndTime.Success)
                return false;

            try
            {
                var year = ParseDateTimePart(dateAndTime, "year", cultureInfo) ?? throw new InvalidOperationException($"Year could not be parsed from given value '{value}'");
                var month = ParseDateTimePart(dateAndTime, "month", cultureInfo) ?? 1;
                var day = ParseDateTimePart(dateAndTime, "day", cultureInfo) ?? 1;
                var hour = ParseDateTimePart(dateAndTime, "hour", cultureInfo) ?? 0;
                var minute = ParseDateTimePart(dateAndTime, "minute", cultureInfo) ?? 0;
                var second = ParseDateTimePart(dateAndTime, "second", cultureInfo) ?? 0;
                var offsetTimeSpan = ParseOffset(offset);
                var start = new DateTimeOffset(year, month, day, hour, minute, second, offsetTimeSpan);
                dateTimeSpan = start.ConvertToDateTimeSpan();
                return true;
            }
            catch (ArgumentException) { }

            return false;
        }

        private static bool TryConvertChronicSpanFormattedString(string value, DateTimeOffset now, out DateTimeSpan dateTimeSpan)
        {
            dateTimeSpan = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            var startAndEnd = Regex.Match(value, "^(?<start>.*?)(_(?<end>(.*)))?$");
            var startValue = RemoveHyphenForChronicParse(startAndEnd.Groups["start"].Value);
            var endValue = RemoveHyphenForChronicParse(startAndEnd.Groups["end"].Value);
            var endIsEmpty = string.IsNullOrWhiteSpace(endValue);

            var options = new Options { Clock = () => now.DateTime };
            var start = new Parser(options).Parse(startValue);
            var end = new Parser(options).Parse(endValue);

            if (start != null && endIsEmpty)
                end = new Span(now.DateTime, now.DateTime);

            if (!startAndEnd.Success || start?.Start.HasValue != true || end?.Start.HasValue != true)
                return false;

            dateTimeSpan = new DateTimeSpan(start.Start.Value, end.Start.Value);
            return true;
        }

        private static bool TryConvertUnknownFormattedString(string value, CultureInfo cultureInfo, out DateTimeSpan dateTimeSpan)
        {
            var result = DateTimeOffset.TryParse(value, cultureInfo, DateTimeStyles.AssumeUniversal, out var dateTimeValue);
            dateTimeSpan = dateTimeValue.ConvertToDateTimeSpan();
            return result;
        }

        private static int? ParseDateTimePart(Match lMatch, string name, IFormatProvider cultureInfo)
            => int.TryParse(lMatch.Groups[name].Value, NumberStyles.Any, cultureInfo, out var intValue) ? intValue : (int?)null;

        private static TimeSpan ParseOffset(string offset)
        {
            if (string.IsNullOrEmpty(offset) || offset == "Z")
                return TimeSpan.Zero;

            var offsetHasSign = offset[0] == '+' || offset[0] == '-';
            var absoluteOffset = offsetHasSign ? offset[1..] : offset;
            if (!TimeSpan.TryParse(absoluteOffset, out var timeSpan))
                return TimeSpan.Zero;

            var signMultiplier = offset[0] == '+' ? 1 : -1;
            return timeSpan * signMultiplier;
        }

        private static string RemoveHyphenForChronicParse(string value)
            => Regex.Replace(value, "-([a-z0-9])", " $1");
    }
}

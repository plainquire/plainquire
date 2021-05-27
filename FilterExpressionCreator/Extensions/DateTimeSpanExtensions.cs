using Chronic.Core;
using FilterExpressionCreator.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FilterExpressionCreator.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeSpanExtensions
    {
        private const string DATE_TIME_ROUND_TRIP_STRING_PATTERN = @"(?<year>\d\d\d\d)(\D(?<month>\d\d))?(\D(?<day>\d\d))?(\D(?<hour>\d\d))?(\D(?<minute>\d\d))?(\D(?<second>\d\d))?(\D\d+)?(Z|\+\d\d:\d\d)?";

        /// <summary>
        /// Tries to convert a string to date time span.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="dateTimeSpan">The parsed date time span.</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static bool TryConvertStringToDateTimeSpan(this string value, DateTime now, out DateTimeSpan dateTimeSpan, CultureInfo cultureInfo = null)
        {
            dateTimeSpan = new DateTimeSpan(DateTime.MinValue, DateTime.MinValue);

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
        /// <param name="value">The date/time to expand.</param>
        public static DateTimeSpan ConvertToDateTimeSpan(this DateTime value)
        {
            var start = value;
            DateTime end;
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
            dateTimeSpan = new DateTimeSpan(DateTime.MinValue, DateTime.MinValue);

            var match = Regex.Match(value, $"^(?<start>{DATE_TIME_ROUND_TRIP_STRING_PATTERN})_(?<end>{DATE_TIME_ROUND_TRIP_STRING_PATTERN})$");
            if (!match.Success)
                return false;

            var startParsed = DateTime.TryParse(match.Groups["start"].Value, cultureInfo, DateTimeStyles.AdjustToUniversal, out var start);
            var endParsed = DateTime.TryParse(match.Groups["end"].Value, cultureInfo, DateTimeStyles.AdjustToUniversal, out var end);
            if (!startParsed || !endParsed)
                return false;

            dateTimeSpan = new DateTimeSpan(start, end);
            return true;
        }

        private static bool TryConvertRoundTripFormattedString(string value, IFormatProvider cultureInfo, out DateTimeSpan dateTimeSpan)
        {
            dateTimeSpan = new DateTimeSpan(DateTime.MinValue, DateTime.MinValue);
            int? getNamedGroup(Match lMatch, string name) => int.TryParse(lMatch.Groups[name].Value, NumberStyles.Any, cultureInfo, out var intValue) ? intValue : (int?)null;

            var match = Regex.Match(value, $"^{DATE_TIME_ROUND_TRIP_STRING_PATTERN}$");
            if (!match.Success)
                return false;

            try
            {
                var year = getNamedGroup(match, "year") ?? throw new InvalidOperationException($"Year could not be parsed from given value '{value}'");
                var month = getNamedGroup(match, "month") ?? 1;
                var day = getNamedGroup(match, "day") ?? 1;
                var hour = getNamedGroup(match, "hour") ?? 0;
                var minute = getNamedGroup(match, "minute") ?? 0;
                var second = getNamedGroup(match, "second") ?? 0;
                var start = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                dateTimeSpan = start.ConvertToDateTimeSpan();
                return true;
            }
            catch (ArgumentException) { }

            return false;
        }

        private static bool TryConvertChronicSpanFormattedString(string value, DateTime now, out DateTimeSpan dateTimeSpan)
        {
            dateTimeSpan = new DateTimeSpan(DateTime.MinValue, DateTime.MinValue);

            var match = Regex.Match(value, "^(?<start>.*?)(_(?<end>(.*)))?$");
            var startValue = RemoveHyphenForChronicParse(match.Groups["start"].Value);
            var endValue = RemoveHyphenForChronicParse(match.Groups["end"].Value);
            var endIsEmpty = string.IsNullOrWhiteSpace(endValue);

            var options = new Options { Clock = () => now };
            var start = new Parser(options).Parse(startValue);
            var end = new Parser(options).Parse(endValue);

            if (start != null && endIsEmpty)
                end = new Span(now, now);

            if (!match.Success || start?.Start.HasValue != true || end?.Start.HasValue != true)
                return false;

            dateTimeSpan = new DateTimeSpan(start.Start.Value, end.Start.Value);
            return true;
        }

        private static bool TryConvertUnknownFormattedString(string value, CultureInfo cultureInfo, out DateTimeSpan dateTimeSpan)
        {
            var result = DateTime.TryParse(value, cultureInfo, DateTimeStyles.AdjustToUniversal, out var dateTimeValue);
            dateTimeSpan = dateTimeValue.ConvertToDateTimeSpan();
            return result;
        }

        private static string RemoveHyphenForChronicParse(string value)
            => Regex.Replace(value, "-([a-z0-9])", " $1");
    }
}

using Chronic.Core;
using FS.FilterExpressionCreator.Abstractions.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FS.FilterExpressionCreator.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeRangeExtensions
    {
        /// <summary>
        /// Convert a string to <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static DateTimeOffset ConvertStringToDateTimeOffset(this string value, DateTimeOffset now, CultureInfo cultureInfo = null)
        {
            if (TryConvertStringToDateTimeRange(value, now, out var dateTimeRange, cultureInfo))
                return dateTimeRange.Start;
            throw new ArgumentException($"{value} could not be converted to date/time range", nameof(value));
        }

        /// <summary>
        /// Try to convert a string to <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="dateTimeOffset">The parsed <see cref="DateTimeOffset"/>.</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static bool TryConvertStringToDateTimeOffset(this string value, DateTimeOffset now, out DateTimeOffset dateTimeOffset, CultureInfo cultureInfo = null)
        {
            if (TryConvertStringToDateTimeRange(value, now, out var dateTimeRange, cultureInfo))
            {
                dateTimeOffset = dateTimeRange.Start;
                return true;
            }

            dateTimeOffset = DateTimeOffset.MinValue;
            return false;
        }

        /// <summary>
        /// Convert a string to date time range.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static Range<DateTimeOffset> ConvertStringToDateTimeRange(this string value, DateTimeOffset now, CultureInfo cultureInfo = null)
        {
            if (TryConvertStringToDateTimeRange(value, now, out var dateTimeRange, cultureInfo))
                return dateTimeRange;
            throw new ArgumentException($"{value} could not be converted to date/time range", nameof(value));
        }

        /// <summary>
        /// Try to convert a string to date time range.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="dateTimeRange">The parsed date time range.</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static bool TryConvertStringToDateTimeRange(this string value, DateTimeOffset now, out Range<DateTimeOffset> dateTimeRange, CultureInfo cultureInfo = null)
        {
            dateTimeRange = new Range<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            if (value == null)
                return false;

            if (Abstractions.Extensions.DateTimeRangeExtensions.TryConvertDateTimeRangeFormattedString(value, cultureInfo, out dateTimeRange))
                return true;
            if (Abstractions.Extensions.DateTimeRangeExtensions.TryConvertIso8601FormattedString(value, cultureInfo, out dateTimeRange))
                return true;
            if (TryConvertChronicRangeFormattedString(value, now, out dateTimeRange))
                return true;
            if (Abstractions.Extensions.DateTimeRangeExtensions.TryConvertUnknownFormattedString(value, cultureInfo, out dateTimeRange))
                return true;

            return false;
        }

        private static bool TryConvertChronicRangeFormattedString(string value, DateTimeOffset now, out Range<DateTimeOffset> dateTimeRange)
        {
            dateTimeRange = new Range<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

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

            dateTimeRange = new Range<DateTimeOffset>(start.Start.Value, end.Start.Value);
            return true;
        }

        private static string RemoveHyphenForChronicParse(string value)
            => Regex.Replace(value, "-([a-z0-9])", " $1");
    }
}

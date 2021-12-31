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
    public static class DateTimeSectionExtensions
    {
        /// <summary>
        /// Convert a string to <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static DateTimeOffset ConvertStringToDateTimeOffset(this string value, DateTimeOffset now, CultureInfo cultureInfo = null)
        {
            if (TryConvertStringToDateTimeSection(value, now, out var dateTimeSection, cultureInfo))
                return dateTimeSection.Start;
            throw new ArgumentException($"{value} could not be converted to date/time section", nameof(value));
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
            if (TryConvertStringToDateTimeSection(value, now, out var dateTimeSection, cultureInfo))
            {
                dateTimeOffset = dateTimeSection.Start;
                return true;
            }

            dateTimeOffset = DateTimeOffset.MinValue;
            return false;
        }

        /// <summary>
        /// Convert a string to date time section.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static Section<DateTimeOffset> ConvertStringToDateTimeSection(this string value, DateTimeOffset now, CultureInfo cultureInfo = null)
        {
            if (TryConvertStringToDateTimeSection(value, now, out var dateTimeSection, cultureInfo))
                return dateTimeSection;
            throw new ArgumentException($"{value} could not be converted to date/time section", nameof(value));
        }

        /// <summary>
        /// Try to convert a string to date time section.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
        /// <param name="dateTimeSection">The parsed date time section.</param>
        /// <param name="cultureInfo">The culture to use when parsing.</param>
        public static bool TryConvertStringToDateTimeSection(this string value, DateTimeOffset now, out Section<DateTimeOffset> dateTimeSection, CultureInfo cultureInfo = null)
        {
            dateTimeSection = new Section<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            if (value == null)
                return false;

            if (Abstractions.Extensions.DateTimeSectionExtensions.TryConvertDateTimeSectionFormattedString(value, cultureInfo, out dateTimeSection))
                return true;
            if (Abstractions.Extensions.DateTimeSectionExtensions.TryConvertIso8601FormattedString(value, cultureInfo, out dateTimeSection))
                return true;
            if (TryConvertChronicSectionFormattedString(value, now, out dateTimeSection))
                return true;
            if (Abstractions.Extensions.DateTimeSectionExtensions.TryConvertUnknownFormattedString(value, cultureInfo, out dateTimeSection))
                return true;

            return false;
        }

        private static bool TryConvertChronicSectionFormattedString(string value, DateTimeOffset now, out Section<DateTimeOffset> dateTimeSection)
        {
            dateTimeSection = new Section<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

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

            dateTimeSection = new Section<DateTimeOffset>(start.Start.Value, end.Start.Value);
            return true;
        }

        private static string RemoveHyphenForChronicParse(string value)
            => Regex.Replace(value, "-([a-z0-9])", " $1");
    }
}

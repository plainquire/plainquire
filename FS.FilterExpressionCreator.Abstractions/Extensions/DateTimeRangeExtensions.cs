using FS.FilterExpressionCreator.Abstractions.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FS.FilterExpressionCreator.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="DateTime"/>.
/// </summary>
public static class DateTimeRangeExtensions
{
    /// <summary>
    /// Creates a new <see cref="Range{TType}"/> using given values. Values not given are expanded to start and end of its period.
    /// </summary>
    private static Range<DateTimeOffset> CreateDateTimeRange(int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null, int? millisecond = null, TimeSpan offset = default)
    {
        var start = new DateTimeOffset(
            year ?? DateTimeOffset.MinValue.Year,
            month ?? 1,
            day ?? 1,
            hour ?? 0,
            minute ?? 0,
            second ?? 0,
            millisecond ?? 0,
            offset);

        var end = start;
        if (year == null)
            end = DateTimeOffset.MaxValue;
        else if (month == null)
            end = end.AddYears(1);
        else if (day == null)
            end = end.AddMonths(1);
        else if (hour == null)
            end = end.AddDays(1);
        else if (minute == null)
            end = end.AddHours(1);
        else if (second == null)
            end = end.AddMinutes(1);
        else if (millisecond == null)
            end = end.AddSeconds(1);
        else
            end = end.AddMilliseconds(millisecond.Value);

        return new Range<DateTimeOffset>(start, end);
    }

    internal static bool TryConvertDateTimeRangeFormattedString(string value, IFormatProvider? cultureInfo, out Range<DateTimeOffset> dateTimeRange)
    {
        dateTimeRange = new Range<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

        var startAndEnd = Regex.Match(value, "^(?<start>.+?)_(?<end>.+)$");
        if (!startAndEnd.Success)
            return false;

        var startParsed = TryConvertIso8601FormattedString(startAndEnd.Groups["start"].Value, cultureInfo, out var start);
        var endParsed = TryConvertIso8601FormattedString(startAndEnd.Groups["end"].Value, cultureInfo, out var end);
        if (!startParsed || !endParsed)
            return false;

        dateTimeRange = new Range<DateTimeOffset>(start.Start, end.End);
        return true;
    }

    internal static bool TryConvertIso8601FormattedString(string value, IFormatProvider? cultureInfo, out Range<DateTimeOffset> dateTimeRange)
    {
        dateTimeRange = new Range<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

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
            var month = ParseDateTimePart(dateAndTime, "month", cultureInfo);
            var day = ParseDateTimePart(dateAndTime, "day", cultureInfo);
            var hour = ParseDateTimePart(dateAndTime, "hour", cultureInfo);
            var minute = ParseDateTimePart(dateAndTime, "minute", cultureInfo);
            var second = ParseDateTimePart(dateAndTime, "second", cultureInfo);
            var offsetTimeSpan = ParseOffset(offset);
            dateTimeRange = CreateDateTimeRange(year, month, day, hour, minute, second, null, offsetTimeSpan);
            return true;
        }
        catch (ArgumentException) { }

        return false;
    }

    internal static bool TryConvertUnknownFormattedString(string value, CultureInfo? cultureInfo, out Range<DateTimeOffset> dateTimeRange)
    {
        var result = DateTimeOffset.TryParse(value, cultureInfo, DateTimeStyles.AssumeUniversal, out var startDate);

        DateTimeOffset endDate;
        if (startDate.Second != 0)
            endDate = startDate.AddSeconds(1);
        else if (startDate.Minute != 0)
            endDate = startDate.AddMinutes(1);
        else if (startDate.Hour != 0)
            endDate = startDate.AddHours(1);
        else if (startDate.Day != 1)
            endDate = startDate.AddDays(1);
        else if (startDate.Month != 1)
            endDate = startDate.AddMonths(1);
        else
            endDate = startDate.AddYears(1);

        dateTimeRange = new Range<DateTimeOffset>(startDate, endDate);
        return result;
    }

    private static int? ParseDateTimePart(Match lMatch, string name, IFormatProvider? cultureInfo)
        => int.TryParse(lMatch.Groups[name].Value, NumberStyles.Any, cultureInfo, out var intValue) ? intValue : null;

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
}
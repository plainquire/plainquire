﻿using Chronic.Core;
using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Plainquire.Filter;

/// <summary>
/// Extension methods for <see cref="DateTime"/>.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Provided as library, can be used from outside")]
public static class DateTimeRangeExtensions
{
    /// <summary>
    /// Convert a string to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="now">Value used for 'now' when parsing relative date/time values (e.g. one-week-ago).</param>
    /// <param name="cultureInfo">The culture to use when parsing.</param>
    public static DateTimeOffset ConvertStringToDateTimeOffset(this string? value, DateTimeOffset now, CultureInfo? cultureInfo = null)
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
    public static bool TryConvertStringToDateTimeOffset(this string? value, DateTimeOffset now, out DateTimeOffset dateTimeOffset, CultureInfo? cultureInfo = null)
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
    public static Range<DateTimeOffset> ConvertStringToDateTimeRange(this string? value, DateTimeOffset now, CultureInfo? cultureInfo = null)
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
    public static bool TryConvertStringToDateTimeRange(this string? value, DateTimeOffset now, out Range<DateTimeOffset> dateTimeRange, CultureInfo? cultureInfo = null)
    {
        dateTimeRange = new Range<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

        if (value == null)
            return false;

        if (Abstractions.DateTimeRangeExtensions.TryConvertDateTimeRangeFormattedString(value, cultureInfo, out dateTimeRange))
            return true;
        if (Abstractions.DateTimeRangeExtensions.TryConvertIso8601FormattedString(value, cultureInfo, out dateTimeRange))
            return true;
        if (TryConvertChronicRangeFormattedString(value, now, out dateTimeRange))
            return true;
        if (Abstractions.DateTimeRangeExtensions.TryConvertUnknownFormattedString(value, cultureInfo, out dateTimeRange))
            return true;

        return false;
    }

    [SuppressMessage("Design", "MA0132:Do not convert implicitly to DateTimeOffset", Justification = "Used library Chronic.Core can only handle DateTime")]
    private static bool TryConvertChronicRangeFormattedString(string value, DateTimeOffset now, out Range<DateTimeOffset> dateTimeRange)
    {
        dateTimeRange = new Range<DateTimeOffset>(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

        var startAndEnd = Regex.Match(value, "^(?<start>.*?)(_(?<end>(.*)))?$", RegexOptions.ExplicitCapture, RegexDefaults.Timeout);
        var startValue = RemoveHyphenForChronicParse(startAndEnd.Groups["start"].Value);
        var endValue = RemoveHyphenForChronicParse(startAndEnd.Groups["end"].Value);
        var endIsEmpty = string.IsNullOrWhiteSpace(endValue);

        var options = new Options { Clock = () => now.DateTime };
        var start = new Parser(options).Parse(startValue);
        var end = new Parser(options).Parse(endValue);

        if (start != null && endIsEmpty)
            end = new Span(now.DateTime, now.DateTime);

        if (!startAndEnd.Success || start?.Start == null || end?.Start == null)
            return false;

        dateTimeRange = new Range<DateTimeOffset>(start.Start.Value, end.Start.Value);
        return true;
    }

    private static string RemoveHyphenForChronicParse(string value)
        => Regex.Replace(value, "-(?<word>[a-z0-9])", " ${word}", RegexOptions.ExplicitCapture, RegexDefaults.Timeout);
}
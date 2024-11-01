using System;

namespace Plainquire.Demo.Extensions;

internal static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime date)
        => DateOnly.FromDateTime(date);

    public static DateOnly? ToDateOnly(this DateTime? date)
        => date?.ToDateOnly();
}

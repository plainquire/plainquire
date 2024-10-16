using System;

namespace Plainquire.Filter.Abstractions;

internal static class StringExtensions
{
    public static bool EqualsOrdinal(this string? left, string? right)
        => string.Equals(left, right, StringComparison.Ordinal);
}
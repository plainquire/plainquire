using System;
using System.Diagnostics.CodeAnalysis;
using Plainquire.Filter.Abstractions.Models;
using Range = Plainquire.Filter.Abstractions.Models.Range;

namespace Plainquire.Filter.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="Range"/>.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Class has library style and can be shared between projects")]
public static class RangeExtensions
{
    /// <summary>
    /// Checks if two <see name="Range{TType}"/> intersects with each other.
    /// </summary>
    /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
    /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
    public static bool Intersect<TType>(this Range<TType>? val1, Range<TType>? val2)
        where TType : IComparable<TType>
    {
        if (val1 == null || val2 == null)
            return false;

        var val1StartIsLowerOrEqualThanVal2End = val1.Start == null || val2.End == null || val1.Start.CompareTo(val2.End) <= 0;
        var val2StartIsLowerOrEqualThanVal1End = val2.Start == null || val1.End == null || val2.Start.CompareTo(val1.End) <= 0;
        return val1StartIsLowerOrEqualThanVal2End && val2StartIsLowerOrEqualThanVal1End;
    }

    /// <summary>
    /// Returns the intersected range of two <see name="Range{TType}"/>.
    /// </summary>
    /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
    /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
    public static Range<TType>? Intersection<TType>(this Range<TType>? val1, Range<TType>? val2)
        where TType : IComparable<TType>
    {
        if (val1 == null)
            return val2;

        if (val2 == null)
            return val1;

        if (!Intersect(val1, val2))
            return default;

        var start = Max(val1.Start, val2.Start);
        var end = Min(val1.End, val2.End);
        return new Range<TType>(start, end);
    }

    /// <summary>
    /// Returns the union range of two <see name="Range{TType}"/>.
    /// </summary>
    /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
    /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
    public static Range<TType>? Union<TType>(this Range<TType>? val1, Range<TType>? val2)
        where TType : IComparable<TType>
    {
        if (val1 == null)
            return val2;

        if (val2 == null)
            return val1;

        var start = Min(val1.Start, val2.Start);
        var end = Max(val1.End, val2.End);
        return new Range<TType>(start, end);
    }

    /// <summary>
    /// Checks if one <see name="Range{TType}"/> contains another.
    /// </summary>
    /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
    /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
    public static bool Contains<TType>(this Range<TType>? val1, Range<TType>? val2)
        where TType : IComparable<TType>
    {
        if (val1 == null || val2 == null)
            return false;

        var val1StartIsLowerOrEqualThanVal2Start = val1.Start == null || (val2.Start != null && val1.Start.CompareTo(val2.Start) <= 0);
        var val1EndIsGreaterOrEqualThanVal2End = val1.End == null || (val2.End != null && val1.End.CompareTo(val2.End) >= 0);
        return val1StartIsLowerOrEqualThanVal2Start && val1EndIsGreaterOrEqualThanVal2End;
    }

    /// <summary>
    /// Returns the lower of two <typeparamref name="TType"/>.
    /// </summary>
    /// <param name="val1">The first of two <typeparamref name="TType"/> to compare.</param>
    /// <param name="val2">The second of two <typeparamref name="TType"/> to compare.</param>
    public static TType? Min<TType>(TType? val1, TType? val2)
        where TType : IComparable<TType>
    {
        if (val1 == null)
            return val1;

        if (val2 == null)
            return val2;

        return val1.CompareTo(val2) <= 0 ? val1 : val2;
    }

    /// <summary>
    /// Returns the greater of two <typeparamref name="TType"/>.
    /// </summary>
    /// <param name="val1">The first of two <typeparamref name="TType"/> to compare.</param>
    /// <param name="val2">The second of two <typeparamref name="TType"/> to compare.</param>
    public static TType? Max<TType>(TType? val1, TType? val2)
        where TType : IComparable<TType>
    {
        if (val1 == null)
            return val2;

        if (val2 == null)
            return val1;

        return val1.CompareTo(val2) >= 0 ? val1 : val2;
    }
}
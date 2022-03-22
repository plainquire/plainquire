using FS.FilterExpressionCreator.Abstractions.Models;
using System;
using Range = FS.FilterExpressionCreator.Abstractions.Models.Range;

namespace FS.FilterExpressionCreator.Abstractions.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Range"/>.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Checks if two <see name="Range{TType}"/> intersects with each other.
        /// </summary>
        /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
        /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
        public static bool Intersect<TType>(this Range<TType> val1, Range<TType> val2)
            where TType : IComparable<TType>
        {
            var val1StartIsLowerThanVal2End = val1.Start.CompareTo(val2.End) <= 0;
            var val2StartIsLowerThanVal1End = val2.Start.CompareTo(val1.End) <= 0;
            return val1StartIsLowerThanVal2End && val2StartIsLowerThanVal1End;
        }

        /// <summary>
        /// Returns the intersected range of two <see name="Range{TType}"/>.
        /// </summary>
        /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
        /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
        public static Range<TType> Intersection<TType>(this Range<TType> val1, Range<TType> val2)
            where TType : IComparable<TType>
        {
            if (!Intersect(val1, val2))
                return default;

            var start = Max(val1.Start, val2.Start);
            var end = Min(val1.End, val2.End);
            return new Range<TType>(start, end);
        }

        /// <summary>
        /// Checks if one <see name="Range{TType}"/> contains another.
        /// </summary>
        /// <param name="val1">The first of two <see name="Range{TType}"/> to compare.</param>
        /// <param name="val2">The second of two <see name="Range{TType}"/> to compare.</param>
        public static bool Contains<TType>(this Range<TType> val1, Range<TType> val2)
            where TType : IComparable<TType>
        {
            if (val2 == null)
                throw new ArgumentNullException(nameof(val2));

            var thisStartIsLowerThanRhsStart = val1.Start.CompareTo(val2.Start) <= 0;
            var thisEndIsGreaterThanRhsEnd = val1.End.CompareTo(val2.End) >= 0;
            return thisStartIsLowerThanRhsStart && thisEndIsGreaterThanRhsEnd;
        }

        /// <summary>
        /// Returns the lower of two <typeparamref name="TType"/>.
        /// </summary>
        /// <param name="val1">The first of two <typeparamref name="TType"/> to compare.</param>
        /// <param name="val2">The second of two <typeparamref name="TType"/> to compare.</param>
        public static TType Min<TType>(TType val1, TType val2)
            where TType : IComparable<TType>
            => val1.CompareTo(val2) <= 0 ? val1 : val2;

        /// <summary>
        /// Returns the greater of two <typeparamref name="TType"/>.
        /// </summary>
        /// <param name="val1">The first of two <typeparamref name="TType"/> to compare.</param>
        /// <param name="val2">The second of two <typeparamref name="TType"/> to compare.</param>
        public static TType Max<TType>(TType val1, TType val2)
            where TType : IComparable<TType>
            => val1.CompareTo(val2) >= 0 ? val1 : val2;
    }
}

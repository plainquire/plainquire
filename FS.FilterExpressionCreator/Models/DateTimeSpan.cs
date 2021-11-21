using System;

namespace FS.FilterExpressionCreator.Models
{
    /// <summary>
    /// Represents a date time range for filtering
    /// </summary>
    public struct DateTimeSpan : IEquatable<DateTimeSpan>
    {
        /// <summary>
        /// Gets or sets the start of range.
        /// </summary>
        public DateTimeOffset Start { get; set; }

        /// <summary>
        /// Gets or sets the end of range.
        /// </summary>
        public DateTimeOffset End { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSpan"/> class.
        /// </summary>
        /// <param name="start">The start of range.</param>
        /// <param name="end">The end of range.</param>
        public DateTimeSpan(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> between <see cref="Start"/> and <see cref="End"/>/>.
        /// </summary>
        public TimeSpan Duration => End - Start;

        /// <inheritdoc />
        public override string ToString()
            => $"{Start:o}_{End:o}";

        /// <inheritdoc />
        public override bool Equals(object obj)
            => obj is DateTimeSpan span && Equals(span);

        /// <inheritdoc />
        public bool Equals(DateTimeSpan other)
            => Start.Equals(other.Start) && End.Equals(other.End);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(Start, End);

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static bool operator ==(DateTimeSpan left, DateTimeSpan right) => left.Equals(right);

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public static bool operator !=(DateTimeSpan left, DateTimeSpan right) => !(left == right);

        /// <summary>
        /// Returns the smaller of two <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="val1">The first of two <see cref="DateTimeOffset"/> to compare.</param>
        /// <param name="val2">The second of two <see cref="DateTimeOffset"/> to compare.</param>
        public static DateTimeOffset Min(DateTimeOffset val1, DateTimeOffset val2)
            => val1.CompareTo(val2) <= 0 ? val1 : val2;

        /// <summary>
        /// Returns the larger of two <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="val1">The first of two <see cref="DateTimeOffset"/> to compare.</param>
        /// <param name="val2">The second of two <see cref="DateTimeOffset"/> to compare.</param>
        public static DateTimeOffset Max(DateTimeOffset val1, DateTimeOffset val2)
            => val1.CompareTo(val2) >= 0 ? val1 : val2;

        /// <summary>
        /// Checks if two <see cref="DateTimeSpan"/> intersects with each other.
        /// </summary>
        /// <param name="val1">The first of two <see cref="DateTimeOffset"/> to compare.</param>
        /// <param name="val2">The second of two <see cref="DateTimeOffset"/> to compare.</param>
        public static bool Intersect(DateTimeSpan val1, DateTimeSpan val2)
        {
            var val1StartIsLowerThanVal2End = val1.Start.CompareTo(val2.End) <= 0;
            var val2StartIsLowerThanVal1End = val2.Start.CompareTo(val1.End) <= 0;
            return val1StartIsLowerThanVal2End && val2StartIsLowerThanVal1End;
        }

        /// <summary>
        /// Checks if a second <see cref="DateTimeSpan"/> intersects with this one.
        /// </summary>
        /// <param name="other">The <see cref="DateTimeSpan"/> to check intersection with.</param>
        public bool Intersect(DateTimeSpan other)
            => Intersect(this, other);

        /// <summary>
        /// Returns the intersected date/time span of two <see cref="DateTimeSpan"/>.
        /// </summary>
        /// <param name="val1">The first of two <see cref="DateTimeOffset"/> to compare.</param>
        /// <param name="val2">The second of two <see cref="DateTimeOffset"/> to compare.</param>
        public static DateTimeSpan? Intersection(DateTimeSpan val1, DateTimeSpan val2)
        {
            if (!Intersect(val1, val2))
                return null;

            var start = Max(val1.Start, val2.Start);
            var end = Min(val1.End, val2.End);
            return new DateTimeSpan(start, end);
        }

        /// <summary>
        /// Returns the intersected date/time span of this and a second <see cref="DateTimeSpan"/>.
        /// </summary>
        /// <param name="other">The <see cref="DateTimeSpan"/> to check intersection with.</param>
        public DateTimeSpan? Intersection(DateTimeSpan other)
            => Intersection(this, other);

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="other">The <see cref="DateTimeSpan"/> to check containment with.</param>
        public bool Contains(DateTimeSpan other)
        {
            var thisStartIsLowerThanRhsStart = Start.CompareTo(other.Start) <= 0;
            var thisEndIsGreaterThanRhsEnd = End.CompareTo(other.End) >= 0;
            return thisStartIsLowerThanRhsStart && thisEndIsGreaterThanRhsEnd;
        }
    }
}

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
        /// Checks if the given <see cref="DateTimeSpan"/> intersects with this one.
        /// </summary>
        /// <param name="other">The <see cref="DateTimeSpan"/> to check intersection with.</param>
        public bool Intersect(DateTimeSpan other)
        {
            var thisStartIsLowerThanRhsEnd = Start.CompareTo(other.End) <= 0;
            var rhsStartIsLowerThanThisEnd = other.Start.CompareTo(End) <= 0;
            return thisStartIsLowerThanRhsEnd && rhsStartIsLowerThanThisEnd;
        }

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

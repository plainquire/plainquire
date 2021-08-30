using System;

namespace FS.FilterExpressionCreator.Models
{
    /// <summary>
    /// Represents a date time range for filtering
    /// </summary>
    public struct DateTimeSpan
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
        public override string ToString() => $"{Start:o}_{End:o}";
    }
}

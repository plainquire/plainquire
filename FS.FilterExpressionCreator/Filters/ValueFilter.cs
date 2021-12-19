using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.JsonConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace FS.FilterExpressionCreator.Filters
{
    /// <summary>
    /// Defines a filter for a property.
    /// </summary>
    [JsonConverter(typeof(ValueFilterConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ValueFilter
    {
        /// <summary>
        /// Map between <see cref="Operator"/> and the string prefix used for JSON API calls.
        /// </summary>
        private static readonly Dictionary<FilterOperator, string> _filterOperatorToPrefixMap = new Dictionary<FilterOperator, string> {
            {FilterOperator.Default, string.Empty},
            {FilterOperator.Contains, "~"},
            {FilterOperator.EqualCaseInsensitive, "="},
            {FilterOperator.EqualCaseSensitive, "=="},
            {FilterOperator.NotEqual, "!"},
            {FilterOperator.GreaterThan, ">"},
            {FilterOperator.GreaterThanOrEqual, ">="},
            {FilterOperator.LessThan, "<"},
            {FilterOperator.LessThanOrEqual, "<="},
            {FilterOperator.IsNull, "ISNULL"},
            {FilterOperator.NotNull, "NOTNULL"},
        };

        /// <summary>
        /// Gets the filter operator. See <see cref="Operator"/> for details.
        /// </summary>
        public FilterOperator Operator { get; private set; }

        /// <summary>
        /// Gets the values to filter for. Multiple values are combined with logical OR.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Indicates whether this filter is empty.
        /// </summary>
        public bool IsEmpty => Operator != FilterOperator.IsNull && Operator != FilterOperator.NotNull && Value == null;

        private ValueFilter() { }

        /// <summary>
        /// Creates the specified filter
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="filterOperator">The filter operator.</param>
        /// <param name="value">The value to filter for. Unused, if <paramref name="filterOperator"/> is <see cref="FilterOperator.IsNull"/> or <see cref="FilterOperator.NotNull"/>; otherwise at least one value is required.</param>
        public static ValueFilter Create<TValue>(FilterOperator filterOperator, TValue value)
        {
            var isNullableFilterOperator = filterOperator == FilterOperator.IsNull || filterOperator == FilterOperator.NotNull;
            if (isNullableFilterOperator)
                value = default;
            else if (value == null)
                throw new ArgumentException($"Filter values cannot be null. If filtering for NULL is intended, use filter operator '{FilterOperator.IsNull}' or '{FilterOperator.NotNull}'");
            else if (!typeof(TValue).IsFilterableProperty())
                throw new ArgumentException($"The type '{typeof(TValue)}' is not filterable by any known expression creator");

            return new ValueFilter()
            {
                Operator = filterOperator,
                Value = ValueToFilterString(value)
            };
        }

        /// <summary>
        /// Creates the specified filter
        /// </summary>
        /// <param name="filterOperator">The filter operator.</param>
        public static ValueFilter Create(FilterOperator filterOperator)
        {
            var isNullableFilterOperator = filterOperator == FilterOperator.IsNull || filterOperator == FilterOperator.NotNull;
            if (!isNullableFilterOperator)
                throw new InvalidOperationException("A value is required for operators other than NULL/NOT NULL.");

            return Create<object>(filterOperator, null);
        }

        /// <summary>
        /// Creates the specified filter using the default operator.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to filter for.</param>
        public static ValueFilter Create<TValue>(TValue value)
            => Create(FilterOperator.Default, value);

        /// <summary>
        /// Creates the specified filter.
        /// </summary>
        /// <param name="filterSyntax">The filter micro syntax to create the filter from.</param>
        public static ValueFilter Create(string filterSyntax)
        {
            var (filterOperator, value) = ExtractPropertyFilterOperator(filterSyntax);
            return Create(filterOperator, value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_filterOperatorToPrefixMap[Operator]) && Value == null)
                return null;
            return _filterOperatorToPrefixMap[Operator] + Value;
        }

        private static string ValueToFilterString(object value)
        {
            var result = value switch
            {
                null => string.Empty,
                DateTime dateTime => dateTime.ToString("o"),
                DateTimeOffset dateTime => dateTime.ToString("o"),
                _ => value.ToString(),
            };

            return result;
        }

        private static (FilterOperator, string) ExtractPropertyFilterOperator(string filter)
        {
            var filterOperator = FilterOperator.Default;

            if (filter == null)
                return (filterOperator, null);

            var trimmedFilter = filter.TrimStart();

            // Order of if-statements mus be from longest to shortest filter operator, e.g. '==' must be parsed before '=' matches
            if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.IsNull]))
                filterOperator = FilterOperator.IsNull;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.NotNull]))
                filterOperator = FilterOperator.NotNull;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.EqualCaseSensitive]))
                filterOperator = FilterOperator.EqualCaseSensitive;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.GreaterThanOrEqual]))
                filterOperator = FilterOperator.GreaterThanOrEqual;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.LessThanOrEqual]))
                filterOperator = FilterOperator.LessThanOrEqual;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.GreaterThan]))
                filterOperator = FilterOperator.GreaterThan;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.LessThan]))
                filterOperator = FilterOperator.LessThan;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.EqualCaseInsensitive]))
                filterOperator = FilterOperator.EqualCaseInsensitive;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.NotEqual]))
                filterOperator = FilterOperator.NotEqual;
            else if (trimmedFilter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.Contains]))
                filterOperator = FilterOperator.Contains;

            var hasFilterOperator = !string.IsNullOrEmpty(_filterOperatorToPrefixMap[filterOperator]);
            filter = hasFilterOperator ? trimmedFilter : filter;

            var filterValue = filter[_filterOperatorToPrefixMap[filterOperator].Length..];

            return (filterOperator, filterValue);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => ToString();
    }
}

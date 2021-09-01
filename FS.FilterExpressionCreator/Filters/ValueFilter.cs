using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.JsonConverters;

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
        public string[] Values { get; private set; }

        /// <summary>
        /// Indicates whether this filter is empty.
        /// </summary>
        public bool IsEmpty => Values.All(x => x == null);

        private ValueFilter() { }

        /// <summary>
        /// Creates the specified filter
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="filterOperator">The filter operator.</param>
        /// <param name="values">
        /// The values to filter for. Multiple values are combined with conditional OR.
        /// Unused, if <paramref name="filterOperator"/> is <see cref="FilterOperator.IsNull"/> or <see cref="FilterOperator.NotNull"/>; otherwise at least one value is required.
        /// </param>
        public static ValueFilter Create<TValue>(FilterOperator filterOperator, params TValue[] values)
        {
            var isNullableFilterOperator = filterOperator == FilterOperator.IsNull || filterOperator == FilterOperator.NotNull;
            if (isNullableFilterOperator)
                values = null;
            else if (values == null || values.Length == 0)
                throw new ArgumentException("At least one value is required");
            else if (values.Any(x => x == null))
                throw new ArgumentException($"Filter values cannot be null. If filtering for NULL is intended, use filter operator '{FilterOperator.IsNull}' or '{FilterOperator.NotNull}'");
            else if (!typeof(TValue).IsFilterableProperty())
                throw new ArgumentException($"The type '{typeof(TValue)}' is not filterable by any known expression creator");

            return new ValueFilter()
            {
                Operator = filterOperator,
                Values = values?.Select(x => ValueToFilterString(x)).ToArray() ?? new string[] { }
            };
        }

        /// <summary>
        /// Creates the specified filter.
        /// </summary>
        /// <param name="filterSyntax">The filter micro syntax to create the filter from.</param>
        public static ValueFilter Create(string filterSyntax)
        {
            var filterOperator = ExtractPropertyFilterOperator(ref filterSyntax);
            var values = SplitValues(filterSyntax);
            return Create(filterOperator, values);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return !string.IsNullOrEmpty(_filterOperatorToPrefixMap[Operator]) || Values.Any()
               ? _filterOperatorToPrefixMap[Operator] + string.Join(",", Values.Select(x => x.Replace(",", "\\,")))
               : null;
        }

        private static string ValueToFilterString(object value)
        {
            var result = value switch
            {
                null => string.Empty,
                DateTime dateTime => dateTime.ToString("o"),
                _ => value.ToString(),
            };

            return result;
        }

        private static FilterOperator ExtractPropertyFilterOperator(ref string filter)
        {
            var result = FilterOperator.Default;

            if (filter == null)
                return result;

            // Order of if-statements mus be from longest to shortest filter operator, e.g. '==' must be parsed before '=' matches
            if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.IsNull]))
                result = FilterOperator.IsNull;
            if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.NotNull]))
                result = FilterOperator.NotNull;
            if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.EqualCaseSensitive]))
                result = FilterOperator.EqualCaseSensitive;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.GreaterThanOrEqual]))
                result = FilterOperator.GreaterThanOrEqual;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.LessThanOrEqual]))
                result = FilterOperator.LessThanOrEqual;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.GreaterThan]))
                result = FilterOperator.GreaterThan;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.LessThan]))
                result = FilterOperator.LessThan;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.EqualCaseInsensitive]))
                result = FilterOperator.EqualCaseInsensitive;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.NotEqual]))
                result = FilterOperator.NotEqual;
            else if (filter.StartsWith(_filterOperatorToPrefixMap[FilterOperator.Contains]))
                result = FilterOperator.Contains;

            filter = filter[_filterOperatorToPrefixMap[result].Length..];

            return result;
        }

        private static string[] SplitValues(string filterSyntax)
        {
            if (filterSyntax == null)
                return null;

            return Regex
                .Split(filterSyntax, @"(?<!\\)[\|,]")
                .Select(element => element
                    .Replace(@"\|", @"|")
                    .Replace(@"\,", @",")
                    .Replace(@"\\", @"\")
                )
                .ToArray();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => ToString();
    }
}

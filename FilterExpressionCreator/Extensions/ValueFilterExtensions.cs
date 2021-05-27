using FilterExpressionCreator.Enums;
using FilterExpressionCreator.Models;
using System;

namespace FilterExpressionCreator.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ValueFilter"/>.
    /// </summary>
    public static class ValueFilterExtensions
    {
        /// <summary>
        /// Humanizes the filter syntax.
        /// </summary>
        /// <typeparam name="TValue">The type of the filtered value.</typeparam>
        /// <param name="filterSyntax">The filter syntax.</param>
        /// <param name="valueName">Name of the value.</param>
        public static string HumanizeFilterSyntax<TValue>(this string filterSyntax, string valueName)
        {
            if (string.IsNullOrWhiteSpace(filterSyntax))
                return $"{valueName} is unfiltered";

            var filter = ValueFilter.Create(filterSyntax);
            var operatorName = filter.GetOperatorName<TValue>();
            if (filter.Operator == FilterOperator.IsNull || filter.Operator == FilterOperator.NotNull)
                return $"{valueName} {operatorName}";

            var valuesButLast = filter.Values[..^1];
            var prefixValueList = string.Join("', '", valuesButLast);
            var concatKey = filter.Operator == FilterOperator.NotEqual ? "nor" : "or";
            var valueList = !string.IsNullOrEmpty(prefixValueList)
                ? $"'{prefixValueList}' {concatKey} '{filter.Values[^1]}'"
                : $"'{filter.Values[^1]}'";

            return $"{valueName} {operatorName} {valueList}";
        }

        private static string GetOperatorName<TValue>(this ValueFilter filter)
        {
            var filterOperator = filter.Operator == FilterOperator.Default ? GetDefaultOperator<TValue>() : filter.Operator;
            return filterOperator switch
            {
                FilterOperator.Contains => "contains",
                FilterOperator.EqualCaseSensitive => "is (case sensitive)",
                FilterOperator.EqualCaseInsensitive => "is",
                FilterOperator.NotEqual => "is not",
                FilterOperator.LessThanOrEqual => "is less than or equal to",
                FilterOperator.LessThan => "is less than",
                FilterOperator.GreaterThanOrEqual => "is greater than or equal to",
                FilterOperator.GreaterThan => "is greater than",
                FilterOperator.IsNull => "is null",
                FilterOperator.NotNull => "is not null",
                _ => throw new ArgumentOutOfRangeException(nameof(filter))
            };
        }

        private static FilterOperator GetDefaultOperator<TValue>()
        {
            if (typeof(TValue) == typeof(string))
                return FilterOperator.Contains;
            return FilterOperator.EqualCaseInsensitive;
        }
    }
}

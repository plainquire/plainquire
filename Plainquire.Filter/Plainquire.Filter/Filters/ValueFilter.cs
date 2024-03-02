using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Filter;

/// <summary>
/// Defines a single filter.
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class ValueFilter
{
    /// <summary>
    /// Gets the filter operator. See <see cref="Operator"/> for details.
    /// </summary>
    public FilterOperator Operator { get; private set; }

    /// <summary>
    /// JSON or Chronic representation of the value to filter for. Unused, if <see cref="Operator"/> is <see cref="FilterOperator.IsNull"/> or <see cref="FilterOperator.NotNull"/>; otherwise at least one value is required.
    /// </summary>
    public string? Value { get; private set; }

    /// <summary>
    /// Gets or sets the syntax configuration.
    /// </summary>
    public SyntaxConfiguration? SyntaxConfiguration { get; set; }

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
    /// <param name="syntaxConfiguration">Configuration of the micro syntax.</param>
    public static ValueFilter Create<TValue>(FilterOperator filterOperator, TValue? value, SyntaxConfiguration? syntaxConfiguration = null)
    {
        var isNullableFilterOperator = filterOperator is FilterOperator.IsNull or FilterOperator.NotNull;
        if (isNullableFilterOperator)
            value = default;
        else if (value == null)
            throw new ArgumentException($"Filter values cannot be null. If filtering for NULL is intended, use filter operator '{FilterOperator.IsNull}' or '{FilterOperator.NotNull}'", nameof(value));
        else if (!typeof(TValue).IsFilterableProperty())
            throw new ArgumentException($"The type '{typeof(TValue)}' is not filterable by any known expression creator", nameof(value));

        return new ValueFilter
        {
            Operator = filterOperator,
            Value = ValueToFilterString(value),
            SyntaxConfiguration = syntaxConfiguration
        };
    }

    /// <summary>
    /// Creates the specified filter
    /// </summary>
    /// <param name="filterOperator">The filter operator.</param>
    /// <param name="syntaxConfiguration">Configuration of the micro syntax.</param>
    public static ValueFilter Create(FilterOperator filterOperator, SyntaxConfiguration? syntaxConfiguration = null)
    {
        var isNullableFilterOperator = filterOperator is FilterOperator.IsNull or FilterOperator.NotNull;
        if (!isNullableFilterOperator)
            throw new InvalidOperationException("A value is required for operators other than NULL/NOT NULL.");

        return Create<object>(filterOperator, null, syntaxConfiguration);
    }

    /// <summary>
    /// Creates the specified filter using the default operator.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to filter for.</param>
    /// <param name="syntaxConfiguration">Configuration of the micro syntax.</param>
    public static ValueFilter Create<TValue>(TValue value, SyntaxConfiguration? syntaxConfiguration = null)
        => Create(FilterOperator.Default, value, syntaxConfiguration);

    /// <summary>
    /// Creates the specified filter.
    /// </summary>
    /// <param name="filterSyntax">The filter micro syntax to create the filter from.</param>
    /// <param name="syntaxConfiguration">Configuration of the micro syntax.</param>
    public static ValueFilter Create(string? filterSyntax, SyntaxConfiguration? syntaxConfiguration = null)
    {
        var (filterOperator, value) = ExtractFilterOperator(filterSyntax, syntaxConfiguration);
        return Create(filterOperator, value, syntaxConfiguration);
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        var syntaxConfiguration = SyntaxConfiguration ?? new SyntaxConfiguration();
        var operatorSyntax = syntaxConfiguration.FilterOperatorMap.FirstOrDefault(x => x.Value == Operator).Key;
        if (string.IsNullOrEmpty(operatorSyntax) && Value == null)
            return null;

        return operatorSyntax + Value;
    }

    private static string ValueToFilterString(object? value)
    {
        var result = value switch
        {
            null => string.Empty,
            DateTime dateTime => dateTime.ToString("o"),
            DateTimeOffset dateTime => dateTime.ToString("o"),
            _ => value.ToString()
        };

        return result;
    }

    private static (FilterOperator, string?) ExtractFilterOperator(string? filter, SyntaxConfiguration? syntaxConfiguration)
    {
        syntaxConfiguration ??= new SyntaxConfiguration();

        if (filter == null)
            return (FilterOperator.Default, null);

        var trimmedFilter = filter.TrimStart();

        var (filterSyntax, filterOperator) = syntaxConfiguration
            .FilterOperatorMap
            .OrderByDescending(x => x.Key.Length)
            .FirstOrDefault(x => trimmedFilter.StartsWith(x.Key));

        var hasFilterOperator = !string.IsNullOrEmpty(filterSyntax);
        filter = hasFilterOperator ? trimmedFilter : filter;

        var filterValue = filter[filterSyntax.Length..];

        return (filterOperator, filterValue);
    }

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string? DebuggerDisplay => ToString();
}
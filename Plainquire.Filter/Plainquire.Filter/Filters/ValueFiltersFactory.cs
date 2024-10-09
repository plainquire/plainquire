using Plainquire.Filter.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plainquire.Filter;

/// <summary>
/// Factory to create <see cref="ValueFilter"/> for all values from filterSyntax.
/// </summary>
public static class ValueFiltersFactory
{
    /// <summary>
    /// Create <see cref="ValueFilter"/> from filterSyntax.
    /// </summary>
    /// <param name="filterSyntax">The filter micro syntax to create the filter from.</param>
    /// <param name="configuration">The filter configuration to use.</param>
    public static ValueFilter[] Create(string filterSyntax, FilterConfiguration? configuration = null)
    {
        var filters = SplitValues(filterSyntax);
        return filters.Select(filter => ValueFilter.Create(filter, configuration)).ToArray();
    }

    private static IEnumerable<string> SplitValues(string? filterSyntax)
    {
        if (filterSyntax == null)
            return [];

        return Regex
            .Split(filterSyntax, @"(?<!\\)[\|,;]")
            .Select(element => element
                .Replace(@"\|", @"|")
                .Replace(@"\,", @",")
                .Replace(@"\;", @";")
            )
            .ToArray();
    }
}
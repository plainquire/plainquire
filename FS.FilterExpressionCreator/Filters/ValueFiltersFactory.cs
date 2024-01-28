using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FS.FilterExpressionCreator.Filters;

/// <summary>
/// Factory to create <see cref="ValueFilter"/> for all values from filterSyntax.
/// </summary>
public class ValueFiltersFactory
{
    /// <summary>
    /// Create <see cref="ValueFilter"/> from filterSyntax.
    /// </summary>
    /// <param name="filterSyntax">The filter micro syntax to create the filter from.</param>
    public static ValueFilter[] Create(string filterSyntax)
    {
        var filters = SplitValues(filterSyntax);
        return filters.Select(ValueFilter.Create).ToArray();
    }

    private static IEnumerable<string> SplitValues(string? filterSyntax)
    {
        if (filterSyntax == null)
            return Enumerable.Empty<string>();

        return Regex
            .Split(filterSyntax, @"(?<!\\)[\|,;]")
            .Select(element => element
                .Replace(@"\|", @"|")
                .Replace(@"\,", @",")
                .Replace(@"\;", @";")
                .Replace(@"\\", @"\")
            )
            .ToArray();
    }
}
using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plainquire.Filter;

/// <summary>
/// Factory to create <see cref="ValueFilter"/> for all values from filterSyntax.
/// </summary>
public static class ValueFilterFactory
{
    /// <summary>
    /// Create <see cref="ValueFilter"/> from filter syntax.
    /// </summary>
    /// <param name="filterSyntax">The filter micro syntax to create the filter from.</param>
    /// <param name="configuration">The filter configuration to use.</param>
    public static ValueFilter[] Create(string filterSyntax, FilterConfiguration? configuration = null)
    {
        var filters = SplitValues(filterSyntax, configuration);
        return filters.Select(filter => ValueFilter.Create(filter, configuration)).ToArray();
    }

    private static IEnumerable<string> SplitValues(string? filterSyntax, FilterConfiguration? configuration)
    {
        if (filterSyntax == null)
            return [];

        configuration ??= FilterConfiguration.Default ?? new FilterConfiguration();

        var escapeCharacter = Regex.Escape(configuration.EscapeCharacter.ToString());
        var separatorCharacters = configuration.ValueSeparatorChars.Select(x => Regex.Escape(x.ToString())).ToList();

        var splitRegex = $"(?<!{escapeCharacter})[{string.Join(string.Empty, separatorCharacters)}]";
        var values = Regex
            .Split(filterSyntax, splitRegex, RegexOptions.None, RegexDefaults.Timeout)
            .ToList();

        return values;
    }
}

/// <inheritdoc cref="ValueFilterFactory"/>
[Obsolete($"Use {nameof(ValueFilterFactory)} instead.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "Obsolete compatibility")]
public static class ValueFiltersFactory
{
    /// <inheritdoc cref="ValueFilterFactory.Create(string, FilterConfiguration?)"/>
    [Obsolete($"Use {nameof(ValueFilterFactory)}.{nameof(ValueFilterFactory.Create)} instead.")]
    public static ValueFilter[] Create(string filterSyntax, FilterConfiguration? configuration = null)
        => ValueFilterFactory.Create(filterSyntax, configuration);
}
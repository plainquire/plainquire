using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Plainquire.Filter.Abstractions;

/// <summary>
/// Configuration for micro syntax used to parse HTTP query parameters.
/// </summary>
[ExcludeFromCodeCoverage]
public class FilterConfiguration
{
    /// <summary>
    /// Default configuration used when no other configuration is provided.
    /// </summary>
    public static FilterConfiguration? Default { get; set; }

    /// <summary>
    /// The culture used for paring in the format languagecode2-country/regioncode2 (e.g. 'en-US').
    /// </summary>
    public string CultureName { get; set; } = CultureInfo.CurrentCulture.Name;

    /// <inheritdoc cref="FilterConditionalAccess"/>/>
    public FilterConditionalAccess UseConditionalAccess { get; set; } = FilterConditionalAccess.WhenCompiled;

    /// <summary>
    /// Return <c>x => true</c> in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }

    /// <summary>
    /// Map between micro syntax and filter operator. Micro syntax is case-sensitive.
    /// </summary>
    public IDictionary<string, FilterOperator> FilterOperatorMap { get; set; } = new Dictionary<string, FilterOperator>(StringComparer.Ordinal)
    {
        {string.Empty, FilterOperator.Default},
        {"~", FilterOperator.Contains},
        {"^" , FilterOperator.StartsWith},
        {"$" , FilterOperator.EndsWith},
        {"=" , FilterOperator.EqualCaseInsensitive},
        {"==" , FilterOperator.EqualCaseSensitive},
        {"!" , FilterOperator.NotEqual},
        {">" , FilterOperator.GreaterThan},
        {">=" , FilterOperator.GreaterThanOrEqual},
        {"<" , FilterOperator.LessThan},
        {"<=" , FilterOperator.LessThanOrEqual},
        {"ISNULL" , FilterOperator.IsNull},
        {"NOTNULL" , FilterOperator.NotNull}
    };

    /// <summary>
    /// Map between string and boolean value. Strings are case-insensitive.
    /// </summary>
    public IDictionary<string, bool> BooleanMap { get; set; } = new Dictionary<string, bool>(StringComparer.InvariantCulture)
    {
        {"NO", false},
        {"0", false},
        {"YES", true},
        {"1", true},
    };

    /// <summary>
    /// Characters used to split values in micro syntax.
    /// </summary>
    public ICollection<char> ValueSeparatorChars { get; set; } = [',', ';', '|'];

    /// <summary>
    /// Character used as escape character in micro syntax.
    /// </summary>
    public char EscapeCharacter { get; set; } = '\\';
}
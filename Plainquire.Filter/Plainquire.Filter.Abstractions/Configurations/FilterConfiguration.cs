using System.Collections.Generic;
using System.Globalization;

namespace Plainquire.Filter.Abstractions;

/// <summary>
/// Configuration for micro syntax used to parse HTTP query parameters.
/// </summary>
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

    /// <inheritdoc cref="ConditionalAccess"/>/>
    public ConditionalAccess UseConditionalAccess { get; set; } = ConditionalAccess.WhenCompiled;

    /// <summary>
    /// Return <c>x => true</c> in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }

    /// <summary>
    /// Map between micro syntax and filter operator. Micro syntax is case-sensitive.
    /// </summary>
    public Dictionary<string, FilterOperator> FilterOperatorMap { get; set; } = new()
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
    public Dictionary<string, bool> BooleanMap { get; set; } = new()
    {
        {"NO", false},
        {"0", false},
        {"YES", true},
        {"1", true},
    };
}
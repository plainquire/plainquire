using System.Collections.Generic;

namespace Plainquire.Filter.Abstractions;

/// <summary>
/// Configuration for micro syntax used to parse HTTP query parameters.
/// </summary>
public class SyntaxConfiguration
{
    /// <summary>
    /// Map between micro syntax and filter operator.
    /// </summary>
    public Dictionary<string, FilterOperator> FilterOperatorMap { get; set; } = new()
    {
        {string.Empty, FilterOperator.Default},
        {"~", FilterOperator.Contains},
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
    /// Strings that should be handled as boolean <c>false</c>
    /// </summary>
    public string[] BoolFalseStrings { get; set; } = ["NO", "0"];

    /// <summary>
    /// Strings that should be handled as boolean <c>true</c>
    /// </summary>
    public string[] BoolTrueStrings { get; set; } = ["YES", "1"];
}
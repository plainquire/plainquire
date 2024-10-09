using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plainquire.Sort.Abstractions;

/// <summary>
/// Holds sort order specific configuration.
/// </summary>
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class SortConfiguration
{
    /// <summary>
    /// Default configuration used when no other configuration is provided.
    /// </summary>
    public static SortConfiguration? Default { get; set; }

    /// <summary>
    /// Prefixes used to identify an ascending sort order.
    /// </summary>
    public List<string> AscendingPrefixes { get; set; } = ["asc-", "asc ", "+"];

    /// <summary>
    /// Postfixes used to identify an ascending sort order.
    /// </summary>
    public List<string> AscendingPostfixes { get; set; } = ["-asc", " asc", "+"];

    /// <summary>
    /// Prefixes used to identify a descending sort order.
    /// </summary>
    public List<string> DescendingPrefixes { get; set; } = ["desc-", "desc ", "dsc-", "dsc ", "-", "~"];

    /// <summary>
    /// Postfixes used to identify a descending sort order.
    /// </summary>
    public List<string> DescendingPostfixes { get; set; } = ["-desc", " desc", "-dsc", " dsc", "-", "~"];

    /// <summary>
    /// Return <c>source.OrderBy(x => 0)</c> in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }

    /// <inheritdoc cref="ConditionalAccess"/>/>
    public ConditionalAccess UseConditionalAccess { get; set; } = ConditionalAccess.WhenEnumerableQuery;

    /// <summary>
    /// Indicates whether to use case-insensitive property matching.
    /// </summary>
    public bool CaseInsensitivePropertyMatching { get; set; } = true;

    /// <summary>
    /// The primary ascending sort postfix.
    /// </summary>
    public string PrimaryAscendingPostfix => AscendingPostfixes[0];

    /// <summary>
    /// The primary descending sort postfix.
    /// </summary>
    public string PrimaryDescendingPostfix => DescendingPostfixes[0];

    /// <summary>
    /// Regex of allowed prefixes to define sort direction.   
    /// </summary>
    public string SortDirectionPrefixPattern => CreateSortDirectionPrefixPattern();

    /// <summary>
    /// Regex of allowed postfixes to define sort direction.   
    /// </summary>
    public string SortDirectionPostfixPattern => CreateSortDirectionPostfixPattern();

    /// <summary>
    /// Regex pattern to match the sort direction.
    /// </summary>
    public string SortDirectionPattern
        => $"^(?<prefix>{SortDirectionPrefixPattern})(?<propertyPath>.*?)(?<postfix>{SortDirectionPostfixPattern})$";

    private string CreateSortDirectionPrefixPattern()
    {
        var sortDirectionPrefixes = AscendingPrefixes.Concat(DescendingPrefixes);
        var prefixRegex = $"({string.Join('|', sortDirectionPrefixes.Select(Regex.Escape))})?";
        return prefixRegex;
    }

    private string CreateSortDirectionPostfixPattern()
    {
        var sortDirectionPostfixes = AscendingPostfixes.Concat(DescendingPostfixes);
        var postfixRegex = $"({string.Join('|', sortDirectionPostfixes.Select(Regex.Escape))})?";
        return postfixRegex;
    }
}
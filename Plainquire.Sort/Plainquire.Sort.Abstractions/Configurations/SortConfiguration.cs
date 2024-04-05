using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plainquire.Sort.Abstractions;

/// <summary>
/// Holds sort order specific configuration.
/// </summary>
public class SortConfiguration
{
    private List<string> _ascendingPostfixes = ["-asc", " asc", "+"];
    private List<string> _ascendingPrefixes = ["asc-", "asc ", "+"];
    private List<string> _descendingPrefixes = ["desc-", "desc ", "dsc-", "dsc ", "-", "~"];
    private List<string> _descendingPostfixes = ["-desc", " desc", "-dsc", " dsc", "-", "~"];

    /// <summary>
    /// Prefixes used to identify an ascending sort order.
    /// </summary>
    public List<string> AscendingPrefixes { get => _ascendingPrefixes; set => SetAscendingPrefixes(value); }

    /// <summary>
    /// Postfixes used to identify an ascending sort order.
    /// </summary>
    public List<string> AscendingPostfixes { get => _ascendingPostfixes; set => SetAscendingPostfixes(value); }

    /// <summary>
    /// Prefixes used to identify a descending sort order.
    /// </summary>
    public List<string> DescendingPrefixes { get => _descendingPrefixes; set => SetDescendingPrefixes(value); }

    /// <summary>
    /// Postfixes used to identify a descending sort order.
    /// </summary>
    public List<string> DescendingPostfixes { get => _descendingPostfixes; set => SetDescendingPostfixes(value); }

    /// <summary>
    /// The name of the query parameter used to define the sort order.
    /// </summary>
    public string HttpQueryParameterName { get; set; } = "orderBy";

    /// <summary>
    /// Return <c>source.OrderBy(x => 0)</c> in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }

    /// <inheritdoc cref="ConditionalAccess"/>/>
    public ConditionalAccess UseConditionalAccess { get; set; } = ConditionalAccess.WhenEnumerableQuery;

    /// <summary>
    /// Gets or sets a value indicating whether to use case-insensitive property matching.
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
    public string SortDirectionPrefixPattern { get; private set; }

    /// <summary>
    /// Regex of allowed postfixes to define sort direction.   
    /// </summary>
    public string SortDirectionPostfixPattern { get; private set; }

    /// <summary>
    /// Regex pattern to match the sort direction.
    /// </summary>
    public string SortDirectionPattern
        => $"^(?<prefix>{SortDirectionPrefixPattern})(?<propertyPath>.*?)(?<postfix>{SortDirectionPostfixPattern})$";

    /// <summary>
    /// Creates a new instance of <see cref="SortConfiguration"/>.
    /// </summary>
    public SortConfiguration()
    {
        SortDirectionPrefixPattern = CreateSortDirectionPrefixPattern();
        SortDirectionPostfixPattern = CreateSortDirectionPostfixPattern();
    }

    private void SetAscendingPrefixes(List<string> value)
    {
        _ascendingPrefixes = value;
        SortDirectionPrefixPattern = CreateSortDirectionPrefixPattern();
    }

    private void SetAscendingPostfixes(List<string> value)
    {
        _ascendingPostfixes = value;
        SortDirectionPostfixPattern = CreateSortDirectionPostfixPattern();
    }

    private void SetDescendingPrefixes(List<string> value)
    {
        _descendingPrefixes = value;
        SortDirectionPrefixPattern = CreateSortDirectionPrefixPattern();
    }

    private void SetDescendingPostfixes(List<string> value)
    {
        _descendingPostfixes = value;
        SortDirectionPostfixPattern = CreateSortDirectionPostfixPattern();
    }

    private string CreateSortDirectionPrefixPattern()
    {
        var sortDirectionPrefixes = _ascendingPrefixes.Concat(_descendingPrefixes);
        var prefixRegex = $"({string.Join('|', sortDirectionPrefixes.Select(Regex.Escape))})?";
        return prefixRegex;
    }

    private string CreateSortDirectionPostfixPattern()
    {
        var sortDirectionPostfixes = _ascendingPostfixes.Concat(_descendingPostfixes);
        var postfixRegex = $"({string.Join('|', sortDirectionPostfixes.Select(Regex.Escape))})?";
        return postfixRegex;
    }
}
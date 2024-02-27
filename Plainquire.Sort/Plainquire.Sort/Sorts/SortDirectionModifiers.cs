using System.Linq;
using System.Text.RegularExpressions;

namespace Plainquire.Sort.Sorts;

/// <summary>
/// Accepted modifiers for sort direction.
/// </summary>
public static class SortDirectionModifiers
{
    /// <summary>
    /// Prefixes used to identify an ascending sort order.
    /// </summary>
    public static readonly string[] AscendingPrefixes = ["asc-", "asc ", "+"];

    /// <summary>
    /// Prefixes used to identify a descending sort order.
    /// </summary>
    public static readonly string[] DescendingPrefixes = ["desc-", "desc ", "dsc-", "dsc ", "-", "~"];

    /// <summary>
    /// Postfixes used to identify an ascending sort order.
    /// </summary>
    public static readonly string[] AscendingPostfixes = ["-asc", " asc", "+"];

    /// <summary>
    /// Postfixes used to identify a descending sort order.
    /// </summary>
    public static readonly string[] DescendingPostfixes = ["-desc", " desc", "-dsc", " dsc", "-", "~"];

    /// <summary>
    /// The default postfix for ascending sort order.
    /// </summary>
    public static readonly string DefaultAscendingPostfix = AscendingPostfixes[0];

    /// <summary>
    /// The default postfix for descending sort order.
    /// </summary>
    public static readonly string DefaultDescendingPostfix = DescendingPostfixes[0];

    /// <summary>
    /// Regex of allowed prefixes to define sort direction.   
    /// </summary>
    public static string PrefixPattern { get; } = CreateSortDirectionPrefixRegex();

    /// <summary>
    /// Regex of allowed postfixes to define sort direction.   
    /// </summary>
    public static string PostfixPattern { get; } = CreateSortDirectionPostfixRegex();

    private static string CreateSortDirectionPrefixRegex()
    {
        var sortDirectionPrefixes = AscendingPrefixes.Concat(DescendingPrefixes);
        var prefixRegex = $"({string.Join('|', sortDirectionPrefixes.Select(Regex.Escape))})?";
        return prefixRegex;
    }

    private static string CreateSortDirectionPostfixRegex()
    {
        var sortDirectionPostfixes = AscendingPostfixes.Concat(DescendingPostfixes);
        var postfixRegex = $"({string.Join('|', sortDirectionPostfixes.Select(Regex.Escape))})?";
        return postfixRegex;
    }
}
namespace Plainquire.Sort.Abstractions.Configurations;

/// <summary>
/// Holds sort order specific configuration.
/// </summary>
public class SortConfiguration
{
    /// <inheritdoc cref="ConditionalAccess"/>/>
    public ConditionalAccess UseConditionalAccess { get; set; } = ConditionalAccess.WhenEnumerableQuery;

    /// <summary>
    /// Gets or sets a value indicating whether to use case-insensitive property matching.
    /// </summary>
    public bool CaseInsensitivePropertyMatching { get; set; } = true;

    /// <summary>
    /// Return <c>source.OrderBy(x => 0)</c> in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }
}
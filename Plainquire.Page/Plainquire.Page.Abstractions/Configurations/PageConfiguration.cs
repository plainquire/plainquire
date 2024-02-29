namespace Plainquire.Page.Abstractions;

/// <summary>
/// Holds paging specific configuration.
/// </summary>
public class PageConfiguration
{
    /// <summary>
    /// Omit paging in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }
}
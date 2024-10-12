using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Abstractions;

/// <summary>
/// Holds paging specific configuration.
/// </summary>
[ExcludeFromCodeCoverage]
public class PageConfiguration
{
    /// <summary>
    /// Default configuration used when no other configuration is provided.
    /// </summary>
    public static PageConfiguration? Default { get; set; }

    /// <summary>
    /// Omit paging in case of any exception while parsing the value
    /// </summary>
    public bool IgnoreParseExceptions { get; set; }
}
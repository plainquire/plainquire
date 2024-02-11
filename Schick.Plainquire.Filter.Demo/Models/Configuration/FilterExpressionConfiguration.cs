using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Demo.Models.Configuration;

/// <summary>
/// Global application configuration
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class FilterExpressionConfiguration
{
    /// <summary>
    /// The configuration section bind to.
    /// </summary>
    public const string CONFIGURATION_SECTION = "FilterExpressionCreator";
}
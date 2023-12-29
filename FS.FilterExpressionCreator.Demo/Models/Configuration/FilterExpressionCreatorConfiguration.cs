using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Demo.Models.Configuration
{
    /// <summary>
    /// Global application configuration
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FilterExpressionCreatorConfiguration
    {
        /// <summary>
        /// The configuration section bind to.
        /// </summary>
        public const string CONFIGURATION_SECTION = "FilterExpressionCreator";
    }
}

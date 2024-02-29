using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Lower-cases the first character.
    /// </summary>
    /// <param name="value">The value.</param>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? LowercaseFirstChar(this string? value)
        => !string.IsNullOrEmpty(value)
            ? char.ToLowerInvariant(value[0]) + value[1..]
            : value;
}
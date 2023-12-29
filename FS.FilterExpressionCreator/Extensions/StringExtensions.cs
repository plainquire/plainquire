using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Lower-cases the first character.
    /// </summary>
    /// <param name="value">The value.</param>
    [return: NotNullIfNotNull("value")]
    public static string? LowercaseFirstChar(this string? value)
        => !string.IsNullOrEmpty(value)
            ? char.ToLowerInvariant(value[0]) + value[1..]
            : value;

    /// <summary>
    /// Upper-cases the first character.
    /// </summary>
    /// <param name="value">The value.</param>
    [return: NotNullIfNotNull("value")]
    public static string? UppercaseFirstChar(this string? value)
        => !string.IsNullOrEmpty(value)
            ? char.ToUpperInvariant(value[0]) + value[1..]
            : value;
}
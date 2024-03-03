using Plainquire.Sort.Abstractions;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Plainquire.Sort;

/// <summary>
/// A sort order for a property.
/// </summary>
public class PropertySort
{
    private readonly SortConfiguration? _configuration;

    /// <summary>
    /// Path used when no member is specified in a member access expression (<c>person => person</c> instead of <c>person => person.Name</c>).
    /// </summary>
    public const string PATH_TO_SELF = "$SELF$";

    /// <summary>
    /// The path to the property.
    /// </summary>
    public string PropertyPath { get; }

    /// <summary>
    /// The sort direction.
    /// </summary>
    public SortDirection Direction { get; }

    /// <summary>
    /// The position of the sort order.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// Creates a new instance of <see cref="PropertySort"/>.
    /// </summary>
    /// <param name="propertyPath">Path to the property to be sorted by.</param>
    /// <param name="direction">Sort direction to use.</param>
    /// <param name="position">Position in the final sorting sequence.</param>
    /// <param name="configuration">The configuration to use.</param>
    /// <exception cref="ArgumentException"></exception>
    [JsonConstructor]
    internal PropertySort(string propertyPath, SortDirection direction, int position, SortConfiguration? configuration)
    {
        PropertyPath = propertyPath ?? throw new ArgumentNullException(nameof(propertyPath));
        Direction = direction;
        Position = position;
        _configuration = configuration;
    }

    /// <summary>
    /// Creates a new instance of <see cref="PropertySort"/>.
    /// </summary>
    /// <param name="propertyPath">Path to the property to be sorted by.</param>
    /// <param name="direction">Sort direction to use.</param>
    /// <param name="position">Position in the final sorting sequence.</param>
    /// <param name="configuration">The configuration to use.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static PropertySort Create(string propertyPath, SortDirection direction, int? position = null, SortConfiguration? configuration = null)
        => new(propertyPath, direction, position ?? 0, configuration);

    /// <summary>
    /// Creates a new instance of <see cref="PropertySort"/>.
    /// </summary>
    /// <param name="sortSyntax">The sort order syntax.</param>
    /// <param name="position">Position in the final sorting sequence.</param>
    /// <param name="configuration">The configuration to use.</param>
    /// <returns></returns>
    public static PropertySort Create(string sortSyntax, int? position = null, SortConfiguration? configuration = null)
    {
        if (string.IsNullOrEmpty(sortSyntax))
            throw new ArgumentException("Value cannot be null or empty.", nameof(sortSyntax));

        var (propertyPath, sortDirection) = ParseSortSyntax(sortSyntax, configuration);
        return Create(propertyPath, sortDirection, position, configuration);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var configuration = _configuration ?? new SortConfiguration();

        var directionPostfix = Direction == SortDirection.Ascending
            ? configuration.PrimaryAscendingPostfix
            : configuration.PrimaryDescendingPostfix;

        return $"{PropertyPath}{directionPostfix}";
    }

    private static (string PropertyPath, SortDirection Direction) ParseSortSyntax(string sortSyntax, SortConfiguration? configuration)
    {
        configuration ??= new SortConfiguration();

        var sortSyntaxPattern = $"^(?<prefix>{configuration.SortDirectionPrefixPattern})(?<propertyPath>.*?)(?<postfix>{configuration.SortDirectionPostfixPattern})$";
        var match = Regex.Match(sortSyntax, sortSyntaxPattern, RegexOptions.IgnoreCase);

        var hasDescendingPrefix = configuration.DescendingPrefixes.Contains(match.Groups["prefix"].Value);
        var hasDescendingPostfix = configuration.DescendingPostfixes.Contains(match.Groups["postfix"].Value);

        var sortDirection = hasDescendingPrefix || hasDescendingPostfix
            ? SortDirection.Descending
            : SortDirection.Ascending;

        var propertyPath = match.Groups["propertyPath"].Value;

        return (propertyPath, sortDirection);
    }
}
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
    internal static readonly string _sortSyntaxPattern = $"^(?<prefix>{SortDirectionModifiers.PrefixPattern})(?<propertyPath>.*?)(?<postfix>{SortDirectionModifiers.PostfixPattern})$";

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
    /// <exception cref="ArgumentException"></exception>
    [JsonConstructor]
    private PropertySort(string propertyPath, SortDirection direction, int position)
    {
        PropertyPath = propertyPath ?? throw new ArgumentNullException(nameof(propertyPath));
        Direction = direction;
        Position = position;
    }

    /// <summary>
    /// Creates a new instance of <see cref="PropertySort"/>.
    /// </summary>
    /// <param name="propertyPath">Path to the property to be sorted by.</param>
    /// <param name="direction">Sort direction to use.</param>
    /// <param name="position">Position in the final sorting sequence.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static PropertySort Create(string propertyPath, SortDirection direction, int? position = null)
        => new(propertyPath, direction, position ?? 0);

    /// <summary>
    /// Creates a new instance of <see cref="PropertySort"/>.
    /// </summary>
    /// <param name="sortSyntax">The sort order syntax.</param>
    /// <param name="position">Position in the final sorting sequence.</param>
    /// <returns></returns>
    public static PropertySort Create(string sortSyntax, int? position = null)
    {
        if (string.IsNullOrEmpty(sortSyntax))
            throw new ArgumentException("Value cannot be null or empty.", nameof(sortSyntax));

        var (propertyPath, sortDirection) = ParseSortSyntax(sortSyntax);
        return Create(propertyPath, sortDirection, position);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var directionPostfix = Direction == SortDirection.Ascending
            ? SortDirectionModifiers.AscendingPostfixes[0]
            : SortDirectionModifiers.DescendingPostfixes[0];

        return $"{PropertyPath}{directionPostfix}";
    }

    private static (string PropertyPath, SortDirection Direction) ParseSortSyntax(string sortSyntax)
    {
        var match = Regex.Match(sortSyntax, _sortSyntaxPattern, RegexOptions.IgnoreCase);

        var hasDescendingPrefix = SortDirectionModifiers.DescendingPrefixes.Contains(match.Groups["prefix"].Value);
        var hasDescendingPostfix = SortDirectionModifiers.DescendingPostfixes.Contains(match.Groups["postfix"].Value);

        var sortDirection = hasDescendingPrefix || hasDescendingPostfix
            ? SortDirection.Descending
            : SortDirection.Ascending;

        var propertyPath = match.Groups["propertyPath"].Value;

        return (propertyPath, sortDirection);
    }
}
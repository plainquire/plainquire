using System.Text.RegularExpressions;
using FS.SortQueryableCreator.Sorts;

namespace FS.SortQueryableCreator.Extensions;

internal static class PropertySortQueryableExtensions
{
    public static bool BelongsTo(this PropertySort sort, string propertyName)
        => Regex.IsMatch(
               input: sort.PropertyPath,
               pattern: @$"^{Regex.Escape(propertyName)}(\..+)?$",
               options: RegexOptions.IgnoreCase
           );
}
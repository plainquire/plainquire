using FS.SortQueryableCreator.Sorts;
using System.Text.RegularExpressions;

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
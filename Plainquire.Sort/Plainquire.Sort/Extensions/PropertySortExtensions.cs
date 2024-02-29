using System.Text.RegularExpressions;

namespace Plainquire.Sort;

internal static class PropertySortExtensions
{
    public static bool BelongsTo(this PropertySort sort, string propertyName)
        => Regex.IsMatch(
               input: sort.PropertyPath,
               pattern: @$"^{Regex.Escape(propertyName)}(\..+)?$",
               options: RegexOptions.IgnoreCase
           );
}
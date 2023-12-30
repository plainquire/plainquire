using FS.FilterExpressionCreator.Abstractions.Attributes;
using System.Reflection;

namespace FS.SortQueryableCreator.Extensions;

internal static class MemberInfoExtensions
{
    /// <summary>
    /// Gets the (MVC controller action) parameter name of the filter.
    /// </summary>
    /// <param name="member">The property to get the name for.</param>
    /// <param name="prefix">A prefix to use.</param>
    public static string GetSortQueryableParameterName(this MemberInfo member, string? prefix = null)
    {
        var filterAttribute = member.GetCustomAttribute<FilterAttribute>();
        return $"{prefix ?? member.ReflectedType?.Name}{filterAttribute?.Name ?? member.Name}".LowercaseFirstChar();
    }
}
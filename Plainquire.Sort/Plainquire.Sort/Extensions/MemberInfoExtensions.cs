using Plainquire.Filter.Abstractions;
using System.Reflection;

namespace Plainquire.Sort;

internal static class MemberInfoExtensions
{
    /// <summary>
    /// Gets the (MVC controller action) parameter name of the sort.
    /// </summary>
    /// <param name="member">The property to get the name for.</param>
    /// <param name="prefix">A prefix to use.</param>
    public static string GetSortParameterName(this MemberInfo member, string? prefix = null)
    {
        var filterAttribute = member.GetCustomAttribute<FilterAttribute>();
        return $"{prefix ?? member.ReflectedType.ExpandTypeName()}{filterAttribute?.Name ?? member.Name}".LowercaseFirstChar();
    }
}
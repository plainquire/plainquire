using System;
using System.Linq;

namespace Plainquire.Filter.Abstractions;

internal static class TypeExtensions
{
    /// <summary>
    /// Expands the name of the type in the form of <c>Dictionary{int, string}</c> => <c>DictionaryInt32String</c>.
    /// </summary>
    /// <param name="type">The type to get the name for.</param>
    public static string? ExpandTypeName(this Type? type)
    {
        if (type == null)
            return null;

        if (!type.IsGenericType)
            return type.Name;

        if (type.IsGenericTypeDefinition)
            return type.Name.Remove(type.Name.IndexOf('`'));

        // If type is a generic type, expand the type name
        var genericTypeName = ExpandTypeName(type.GetGenericTypeDefinition());
        var genericArgumentNames = type.GetGenericArguments().Select(ExpandTypeName);
        return $"{genericTypeName}{string.Join("", genericArgumentNames)}";
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Schick.Plainquire.Filter.Abstractions.Extensions;

internal static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
        => enumerable.OfType<T>();
}
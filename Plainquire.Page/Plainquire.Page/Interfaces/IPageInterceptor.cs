using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page;

/// <summary>
/// Interceptor to provide custom page logic.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Provided as library, can be used from outside")]
public interface IPageInterceptor
{
    /// <summary>
    /// Default page interceptor used when no other interceptor is provided.
    /// </summary>
    public static IPageInterceptor? Default { get; set; }

    /// <summary>
    /// Pages the elements of a sequence according to the given <paramref name="page"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">The elements to page.</param>
    /// <param name="page">The <see cref="EntityPage"/> used to page the elements.</param>
    IQueryable<TEntity> Page<TEntity>(IQueryable<TEntity> source, EntityPage page);
}
using System.Linq;

namespace Plainquire.Page;

/// <summary>
/// Interceptor to provide custom page logic.
/// </summary>
public interface IPageInterceptor
{
    /// <summary>
    /// Pages the elements of a sequence according to the given <paramref name="page"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">The elements to page.</param>
    /// <param name="page">The <see cref="EntityPage"/> used to page the elements.</param>
    IQueryable<TEntity> Page<TEntity>(IQueryable<TEntity> source, EntityPage page);
}
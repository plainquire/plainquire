using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page;

/// <summary>
/// Extension methods for <see cref="IQueryable{TEntity}"/>
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Class has library style and can be shared between projects")]
public static class QueryableExtensions
{
    /// <inheritdoc cref="Page{TEntity}(IQueryable{TEntity}, int?, int?, IPageInterceptor?)"/>
    public static IEnumerable<TEntity> Page<TEntity>(this IEnumerable<TEntity> source, int? pageNumber, int? pageSize, IPageInterceptor? interceptor = null)
        => source.AsQueryable().Page(new EntityPage(pageNumber, pageSize), interceptor);

    /// <inheritdoc cref="Page{TEntity}(IQueryable{TEntity}, EntityPage, IPageInterceptor?)"/>"
    public static IEnumerable<TEntity> Page<TEntity>(this IEnumerable<TEntity> source, EntityPage page, IPageInterceptor? interceptor = null)
        => source.AsQueryable().Page(page, interceptor);

    /// <summary>
    /// Pages the elements of a sequence according to the given page number and size.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">The elements to page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The page size to use.</param>
    /// <param name="interceptor">An interceptor to manipulate the generated page.</param>
    /// <returns></returns>
    public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> source, int? pageNumber, int? pageSize, IPageInterceptor? interceptor = null)
        => source.Page(new EntityPage(pageNumber, pageSize), interceptor);

    /// <summary>
    /// Pages the elements of a sequence according to the given <paramref name="page"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">The elements to page.</param>
    /// <param name="page">The <see cref="EntityPage"/> used to page the elements.</param>
    /// <param name="interceptor">An interceptor to manipulate the generated page.</param>
    public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> source, EntityPage page, IPageInterceptor? interceptor = null)
    {
        var result = interceptor?.Page(source, page);
        if (result != null)
            return result;

        result = source;

        var (skip, take) = page.GetSkipAndTake();

        if (skip is > 0)
            result = result.Skip(skip.Value);

        if (take is > 0)
            result = result.Take(take.Value);

        return result;
    }
}
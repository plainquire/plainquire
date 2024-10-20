﻿using Plainquire.Sort.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Plainquire.Sort;

/// <summary>
/// Extension methods for <see cref="IQueryable{TEntity}"/>
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Provided as library, can be used from outside")]
public static class QueryableExtensions
{
    /// <inheritdoc cref="OrderBy{TEntity}(IQueryable{TEntity}, EntitySort{TEntity}, ISortInterceptor?)"/>
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IEnumerable<TEntity> source, EntitySort<TEntity> sort, ISortInterceptor? interceptor = null)
        => source.AsQueryable().OrderBy(sort, interceptor);

    /// <summary>
    /// Sorts the elements of a sequence according to the given <paramref name="sort"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">The elements to sort.</param>
    /// <param name="sort">The <see cref="EntitySort{TEntity}"/> used to sort the elements.</param>
    /// <param name="interceptor">An interceptor to manipulate the generated sort order.</param>
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, EntitySort<TEntity> sort, ISortInterceptor? interceptor = null)
    {
        var propertySorts = sort.PropertySorts.OrderBy(x => x.Position).ToList();
        if (!propertySorts.Any())
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(source.Expression);

        var configuration = sort.Configuration ?? SortConfiguration.Default ?? new SortConfiguration();
        interceptor ??= ISortInterceptor.Default;

        var first = propertySorts.First();
        var result = interceptor?.OrderBy(source, first);
        if (result == null)
        {
            var ascending = first.Direction == SortDirection.Ascending;
            result ??= ascending
                ? source.OrderBy(first.PropertyPath, configuration)
                : source.OrderByDescending(first.PropertyPath, configuration);
        }

        foreach (var sortedProperty in propertySorts.Skip(1))
        {
            var interceptedResult = interceptor?.ThenBy(result, sortedProperty);
            if (interceptedResult != null)
            {
                result = interceptedResult;
                continue;
            }

            var ascending = sortedProperty.Direction == SortDirection.Ascending;
            result = ascending
                ? result.ThenBy(sortedProperty.PropertyPath, configuration)
                : result.ThenByDescending(sortedProperty.PropertyPath, configuration);
        }

        return result;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a property path.
    /// </summary>
    /// <typeparam name="TEntity">The type of the sorted entity.</typeparam>
    /// <param name="source">A sequence of values to sort.</param>
    /// <param name="propertyPath">Path to the property to sort by.</param>
    /// <param name="configuration">Sort order configuration.</param>
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyPath, SortConfiguration configuration)
        => source.OrderBy(nameof(Queryable.OrderBy), propertyPath, configuration);

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a property path.
    /// </summary>
    /// <typeparam name="TEntity">The type of the sorted entity.</typeparam>
    /// <param name="source">A sequence of values to sort.</param>
    /// <param name="propertyPath">Path to the property to sort by.</param>
    /// <param name="configuration">Sort order configuration.</param>
    public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string propertyPath, SortConfiguration configuration)
        => source.OrderBy(nameof(Queryable.OrderByDescending), propertyPath, configuration);

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a property path.
    /// </summary>
    /// <typeparam name="TEntity">The type of the sorted entity.</typeparam>
    /// <param name="source">A sequence of values to sort.</param>
    /// <param name="propertyPath">Path to the property to sort by.</param>
    /// <param name="configuration">Sort order configuration.</param>
    public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string propertyPath, SortConfiguration configuration)
        => source.OrderBy(nameof(Queryable.ThenBy), propertyPath, configuration);

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order according to a property path.
    /// </summary>
    /// <typeparam name="TEntity">The type of the sorted entity.</typeparam>
    /// <param name="source">A sequence of values to sort.</param>
    /// <param name="propertyPath">Path to the property to sort by.</param>
    /// <param name="configuration">Sort order configuration.</param>
    public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string propertyPath, SortConfiguration configuration)
        => source.OrderBy(nameof(Queryable.ThenByDescending), propertyPath, configuration);

    private static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string methodName, string propertyPath, SortConfiguration configuration)
    {
        var propertyPathParts = propertyPath.Split('.');
        if (!propertyPathParts.Any())
            throw new ArgumentException("Property path must not be empty.", nameof(propertyPath));

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var useConditionalAccess = source.Provider.ConditionalAccessRequested(configuration);
        var caseInsensitive = configuration.CaseInsensitivePropertyMatching;
        var propertyAccess = (Expression)parameter;
        foreach (var part in propertyPathParts)
        {
            try
            {
                propertyAccess = Add(propertyAccess, part, caseInsensitive, useConditionalAccess);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found on type"))
            {
                if (configuration.IgnoreParseExceptions)
                    return source.OrderBy(x => 0);
                throw;
            }
        }

        var propertyAccessLambda = Expression
            .Lambda(propertyAccess, parameter);

        var orderByExpression = Expression
            .Call(
                type: typeof(Queryable),
                methodName: methodName,
                typeArguments: [typeof(TEntity), propertyAccess.Type],
                arguments: [source.Expression, Expression.Quote(propertyAccessLambda)]
            );

        return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(orderByExpression);
    }

    private static bool ConditionalAccessRequested(this IQueryProvider provider, SortConfiguration configuration)
    {
        switch (configuration.UseConditionalAccess)
        {
            case SortConditionalAccess.Never:
                return false;
            case SortConditionalAccess.Always:
                return true;
            case SortConditionalAccess.WhenEnumerableQuery:
                var providerType = provider.GetType();
                var isEnumerableQueryProvider = providerType.IsGenericType && providerType.GetGenericTypeDefinition() == typeof(EnumerableQuery<>);
                return isEnumerableQueryProvider;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static Expression Add(Expression memberAccess, string propertyName, bool caseInsensitive, bool useConditionalAccess)
    {
        if (propertyName == PropertySort.PATH_TO_SELF)
            return memberAccess;

        var memberType = memberAccess.Type;
        var property = memberType.GetProperty(propertyName);
        if (property == null && caseInsensitive)
            property = memberType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{memberType.Name}'.");

        var propertyAccess = Expression.MakeMemberAccess(memberAccess, property);

        var conditionalAccessRequired = useConditionalAccess && property.PropertyType.IsNullable();
        if (!conditionalAccessRequired)
            return propertyAccess;

        var memberNull = Expression.Constant(null, memberAccess.Type);
        var memberIsNull = Expression.Equal(memberAccess, memberNull);
        var propertyNull = Expression.Constant(null, property.PropertyType);
        var conditionalPropertyAccess = Expression.Condition(memberIsNull, propertyNull, propertyAccess);
        return conditionalPropertyAccess;
    }
}
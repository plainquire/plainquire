using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Web;

namespace Plainquire.Filter;

/// <summary>
/// Extension methods for <see cref="EntityFilter{TEntity}"/>
/// </summary>
public static class EntityFilterExtensions
{
    /// <summary>
    /// Adds a filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="filterSyntax">Description of the filter using micro syntax.</param>
    public static EntityFilter<TEntity> Add<TEntity, TProperty>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, string? filterSyntax)
    {
        if (filterSyntax == null)
            return entityFilter;

        var filters = ValueFilterFactory.Create(filterSyntax, entityFilter.Configuration);
        entityFilter.Add(property, filters);
        return entityFilter;
    }

    /// <summary>
    /// Adds a filter for the given property using the default filter operator. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
    public static EntityFilter<TEntity> Add<TEntity, TProperty, TValue>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, params TValue[]? values)
    {
        if (values == null || values.Length == 0)
            return entityFilter;

        var valueFilters = values
            .Select(value => ValueFilter.Create(
                FilterOperator.Default,
                value,
                entityFilter.Configuration
            ))
            .ToArray();

        return entityFilter.Add(property, valueFilters);
    }

    /// <summary>
    /// Adds a filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="filterOperator">The filter operator to use.</param>
    public static EntityFilter<TEntity> Add<TEntity, TProperty>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, FilterOperator filterOperator)
        => entityFilter.Add(property, ValueFilter.Create(filterOperator, entityFilter.Configuration));

    /// <summary>
    /// Adds a filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="filterOperator">The filter operator to use.</param>
    /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
    public static EntityFilter<TEntity> Add<TEntity, TProperty, TValue>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, FilterOperator filterOperator, params TValue[]? values)
    {
        var isNullableFilterOperator = filterOperator is FilterOperator.IsNull or FilterOperator.NotNull;
        if ((values == null || values.Length == 0) && !isNullableFilterOperator)
            return entityFilter;

        var valueFilters = values?
            .Select(value => ValueFilter.Create(
                filterOperator,
                value,
                entityFilter.Configuration
            ))
            .ToArray();

        entityFilter.Add(property, valueFilters);
        return entityFilter;
    }

    /// <summary>
    /// Replaces the filter for the given property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="filterSyntax">Description of the filter using micro syntax.</param>
    public static EntityFilter<TEntity> Replace<TEntity, TProperty>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, string? filterSyntax)
    {
        if (filterSyntax == null)
            return entityFilter.Remove(property);

        var valueFilters = ValueFilterFactory.Create(filterSyntax, entityFilter.Configuration);
        entityFilter.Replace(property, valueFilters);
        return entityFilter;
    }

    /// <summary>
    /// Replaces the filter for the given property using the default filter operator. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the t property.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
    public static EntityFilter<TEntity> Replace<TEntity, TProperty, TValue>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, params TValue[]? values)
    {
        if (values == null || values.Length == 0)
            return entityFilter.Remove(property);

        var valueFilters = values
            .Select(value => ValueFilter.Create(
                FilterOperator.Default,
                value,
                entityFilter.Configuration
                ))
            .ToArray();

        return entityFilter.Replace(property, valueFilters);
    }

    /// <summary>
    /// Replaces the filter for the given property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="filterOperator">The filter operator to use.</param>
    public static EntityFilter<TEntity> Replace<TEntity, TProperty>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, FilterOperator filterOperator)
        => entityFilter.Replace(property, ValueFilter.Create(filterOperator, entityFilter.Configuration));

    /// <summary>
    /// Replaces the filter for the given property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="entityFilter">The entity filter.</param>
    /// <param name="property">The property to filter.</param>
    /// <param name="filterOperator">The filter operator to use.</param>
    /// <param name="values">The values to filter for. Multiple values are combined with conditional OR.</param>
    public static EntityFilter<TEntity> Replace<TEntity, TProperty, TValue>(this EntityFilter<TEntity> entityFilter, Expression<Func<TEntity, TProperty?>> property, FilterOperator filterOperator, params TValue[]? values)
    {
        var isNullableFilterOperator = filterOperator is FilterOperator.IsNull or FilterOperator.NotNull;
        if ((values == null || values.Length == 0) && !isNullableFilterOperator)
            return entityFilter.Remove(property);

        var valueFilters = values?
            .Select(value => ValueFilter.Create(
                filterOperator,
                value,
                entityFilter.Configuration
                ))
            .ToArray();

        entityFilter.Replace(property, valueFilters);
        return entityFilter;
    }

    /// <summary>
    /// Casts this filter to a different type (by creating a deep clone).
    /// Filtered properties are matched by type (check if assignable) and name (case-sensitive).
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TDestination">The type of the destination entity to filter.</typeparam>
    public static EntityFilter<TDestination> Cast<TEntity, TDestination>(this EntityFilter<TEntity> filter)
    {
        var castFilter = JsonSerializer.Deserialize<EntityFilter<TDestination>>(JsonSerializer.Serialize(filter))!;
        var sourceProperties = typeof(TEntity).GetProperties();
        var destinationProperties = typeof(TDestination).GetProperties().ToList();

        foreach (var sourceProperty in sourceProperties)
        {
            var sameDestinationPropertyExists = destinationProperties
                .Exists(x =>
                            x.Name.EqualsOrdinal(sourceProperty.Name) &&
                            x.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                );

            if (!sameDestinationPropertyExists)
            {
                castFilter.PropertyFilters.RemoveAll(x => x.PropertyName.EqualsOrdinal(sourceProperty.Name));
                castFilter.NestedFilters.RemoveAll(x => x.PropertyName.EqualsOrdinal(sourceProperty.Name));
            }
        }

        return castFilter;
    }

    /// <summary>
    /// Converts an entity filter to it's corresponding HTTP query parameters.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <param name="entityFilter">The filter to act on.</param>
    public static string ToQueryParams<TEntity>(this EntityFilter<TEntity>? entityFilter)
    {
        var queryParams = ToQueryParams((EntityFilter?)entityFilter);
        return string.Join('&', queryParams);
    }

    /// <summary>
    /// Converts an entity filter to it's corresponding HTTP query parameters.
    /// </summary>
    /// <param name="entityFilter">The filter to act on.</param>
    private static List<string> ToQueryParams(this EntityFilter? entityFilter)
    {
        if (entityFilter == null)
            return [];

        var entityFilterType = entityFilter.GetType();
        if (!entityFilterType.IsGenericType)
            throw new ArgumentException($"Given filter must be a generic {nameof(EntityFilter)}", nameof(entityFilter));

        var filteredType = entityFilterType.GenericTypeArguments[0];
        var filterableProperties = filteredType.GetFilterableProperties().ToList();
        var entityFilterAttribute = filteredType.GetCustomAttribute<EntityFilterAttribute>();

        var queryParams = new List<string>();
        foreach (var property in filterableProperties)
        {
            var parameterName = HttpUtility.UrlEncode(property.GetFilterParameterName(entityFilterAttribute?.Prefix));
            var propertyFilters = entityFilter.PropertyFilters.Where(x => x.PropertyName.EqualsOrdinal(property.Name));
            foreach (var filter in propertyFilters)
            {
                var values = string.Join(',', filter.ValueFilters.Select(v => HttpUtility.UrlEncode(v.ToString())));
                queryParams.Add($"{parameterName}={values}");
            }
        }

        foreach (var nestedFilter in entityFilter.NestedFilters)
        {
            var nestedQueryParams = nestedFilter.EntityFilter.ToQueryParams();
            queryParams.AddRange(nestedQueryParams);
        }

        return queryParams;
    }
}
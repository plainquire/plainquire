using Plainquire.Filter.Abstractions;
using Plainquire.Filter.JsonConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plainquire.Filter;

/// <summary>
/// Hub to create filter expressions for <typeparamref name="TEntity"/> with fluent API.
/// </summary>
/// <typeparam name="TEntity">The type to be filtered.</typeparam>
[JsonConverter(typeof(EntityFilterConverterFactory))]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class EntityFilter<TEntity> : EntityFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFilter{TEntity}"/> class.
    /// </summary>
    public EntityFilter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFilter{TEntity}"/> class.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    public EntityFilter(FilterConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the filter syntax for the given <paramref name="property"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    public string? GetPropertyFilterSyntax<TProperty>(Expression<Func<TEntity, TProperty?>> property)
        => GetPropertyFilterSyntaxInternal(property);

    /// <summary>
    /// Get the filters applied to the given property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    public ValueFilter[]? GetPropertyFilterValues<TProperty>(Expression<Func<TEntity, TProperty?>> property)
        => GetPropertyFilterValuesInternal(property);

    /// <summary>
    /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested class <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    public EntityFilter<TProperty>? GetNestedFilter<TProperty>(Expression<Func<TEntity, TProperty?>> property)
        => GetNestedFilterInternal(property);

    /// <summary>
    /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested list <typeparamref name="TList"/>.
    /// </summary>
    /// <typeparam name="TList">The type of the list of <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    public EntityFilter<TProperty>? GetNestedFilter<TList, TProperty>(Expression<Func<TEntity, TList?>> property)
        where TList : IEnumerable<TProperty>
        => GetNestedFilterInternal<TEntity, TList, TProperty>(property);

    /// <summary>
    /// Adds a filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="filters">The filters to use.</param>
    public EntityFilter<TEntity> Add<TProperty>(Expression<Func<TEntity, TProperty?>> property, params ValueFilter[]? filters)
    {
        AddInternal(property, filters);
        return this;
    }

    /// <summary>
    /// Adds a nested filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="nestedFilter">The nested class filter.</param>
    public EntityFilter<TEntity> AddNested<TProperty>(Expression<Func<TEntity, TProperty?>> property, EntityFilter<TProperty>? nestedFilter)
    {
        if (nestedFilter == null)
            return this;

        AddNestedInternal(property, nestedFilter);
        return this;
    }

    /// <summary>
    /// Adds a nested filter for the given enumerable property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <typeparam name="TNested">The nested type to be filtered.</typeparam>
    /// <param name="property">The property to filter. Must implement <see cref="IEnumerable{T}"/>.</param>
    /// <param name="nestedFilter">The nested class filter.</param>
    public EntityFilter<TEntity> AddNested<TProperty, TNested>(Expression<Func<TEntity, TProperty?>> property, EntityFilter<TNested>? nestedFilter)
        where TProperty : IEnumerable<TNested>
    {
        if (nestedFilter == null)
            return this;

        AddNestedInternal(property, nestedFilter);
        return this;
    }

    /// <summary>
    /// Replaces the filter for the given property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="filters">The filters to use.</param>
    public EntityFilter<TEntity> Replace<TProperty>(Expression<Func<TEntity, TProperty?>> property, params ValueFilter[]? filters)
    {
        ReplaceInternal(property, filters);
        return this;
    }

    /// <summary>
    /// Replaces the nested filter for the given property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="nestedFilter">The nested class filter.</param>
    public EntityFilter<TEntity> ReplaceNested<TProperty>(Expression<Func<TEntity, TProperty?>> property, EntityFilter<TProperty>? nestedFilter)
    {
        if (nestedFilter == null)
            return Remove(property);

        ReplaceNestedInternal(property, nestedFilter);
        return this;
    }

    /// <summary>
    /// Replaces the nested filter for the given enumerable property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <typeparam name="TNested">The nested type to be filtered.</typeparam>
    /// <param name="property">The property to filter. Must implement <see cref="IEnumerable{T}"/>.</param>
    /// <param name="nestedFilter">The nested class filter.</param>
    public EntityFilter<TEntity> ReplaceNested<TProperty, TNested>(Expression<Func<TEntity, TProperty?>> property, EntityFilter<TNested>? nestedFilter)
        where TProperty : IEnumerable<TNested>
    {
        if (nestedFilter == null)
            return Remove(property);

        ReplaceNestedInternal(property, nestedFilter);
        return this;
    }

    /// <summary>
    /// Remove all filters for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to remove all filters for.</param>
    public EntityFilter<TEntity> Remove<TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        RemoveInternal(property);
        return this;
    }

    /// <summary>
    /// Removes all filters of all properties.
    /// </summary>
    public EntityFilter<TEntity> Clear()
    {
        ClearInternal();
        return this;
    }

    /// <summary>
    /// Creates a deep clone of this filter.
    /// </summary>
    public new EntityFilter<TEntity> Clone()
        => JsonSerializer.Deserialize<EntityFilter<TEntity>>(JsonSerializer.Serialize(this))!;

    /// <summary>
    /// Casts this filter to a different type (by creating a deep clone).
    /// Filtered properties are matched by type (check if assignable) and name (case-sensitive).
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination entity to filter.</typeparam>
    public EntityFilter<TDestination> Cast<TDestination>()
    {
        var castFilter = JsonSerializer.Deserialize<EntityFilter<TDestination>>(JsonSerializer.Serialize(this))!;
        var sourceProperties = typeof(TEntity).GetProperties();
        var destinationProperties = typeof(TDestination).GetProperties().ToList();

        foreach (var sourceProperty in sourceProperties)
        {
            var sameDestinationPropertyExists = destinationProperties
                .Exists(x =>
                    x.Name == sourceProperty.Name &&
                    x.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                );

            if (!sameDestinationPropertyExists)
            {
                castFilter.PropertyFilters.RemoveAll(x => x.PropertyName == sourceProperty.Name);
                castFilter.NestedFilters.RemoveAll(x => x.PropertyName == sourceProperty.Name);
            }
        }

        return castFilter;
    }

    /// <summary>
    /// Creates the filter expression. Returns <c>null</c> when filter is empty.
    /// </summary>
    /// <param name="interceptor">An interceptor to manipulate the generated filters.</param>
    /// <param name="useAsCompiledExpression">Whether the generated expression will be compiled later. Used to determine conditional property access when <c>FilterConfiguration.UseConditionalAccess</c> = <c>ConditionalAccess.WhenCompiled</c>.</param>
    public Expression<Func<TEntity, bool>>? CreateFilter(IFilterInterceptor? interceptor = null, bool useAsCompiledExpression = true)
        => CreateFilter<TEntity>(interceptor, useAsCompiledExpression);

    /// <summary>
    /// Performs an implicit conversion from <see cref="EntityFilter{TEntity}"/> to <see cref="Expression{TDelegate}"/> where <c>TDelegate</c> is <see cref="Func{T, TResult}"/>.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    public static implicit operator Expression<Func<TEntity, bool>>(EntityFilter<TEntity> filter)
        => filter.CreateFilter(useAsCompiledExpression: false) ?? (x => true);

    /// <summary>
    /// Performs an implicit conversion from <see cref="EntityFilter{TEntity}"/> to <see cref="Func{T, TResult}"/>.
    /// </summary>
    /// <param name="filter">The filter to convert.</param>
    public static implicit operator Func<TEntity, bool>(EntityFilter<TEntity> filter)
        => (filter.CreateFilter(useAsCompiledExpression: true) ?? (x => true)).Compile();

    /// <inheritdoc />
    public override string ToString()
        => CreateFilter()?.ToString() ?? string.Empty;

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => CreateFilter()?.ToString() ?? "<EMPTY>";
}

/// <inheritdoc cref="EntityFilter{TEntity}" />
[JsonConverter(typeof(EntityFilterConverter))]
public class EntityFilter : ICloneable
{
    private static readonly MethodInfo _createFilterMethod = typeof(EntityFilter).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.Name == nameof(CreateFilter));

    internal List<PropertyFilter> PropertyFilters;
    internal List<NestedFilter> NestedFilters;

    /// <summary>
    /// Gets or sets the default configuration. Can be used to set a system-wide configuration.
    /// </summary>
    public FilterConfiguration? Configuration { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFilter"/> class.
    /// </summary>
    public EntityFilter()
    {
        PropertyFilters = [];
        NestedFilters = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFilter"/> class.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    public EntityFilter(FilterConfiguration configuration)
        : this()
        => Configuration = configuration;

    /// <inheritdoc />
    public object Clone()
        => JsonSerializer.Deserialize<EntityFilter>(JsonSerializer.Serialize(this))!;

    /// <summary>
    /// Indicates whether this filter is empty.
    /// </summary>
    public bool IsEmpty() => !PropertyFilters.Any() && !NestedFilters.Any();

    /// <summary>
    /// Gets the filter syntax for the given <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    protected string? GetPropertyFilterSyntaxInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyName = property.GetPropertyName();
        var propertyFilter = PropertyFilters.FirstOrDefault(x => x.PropertyName == propertyName);
        return ValueFilterExtensions.ToString(propertyFilter?.ValueFilters);
    }

    /// <summary>
    /// Get the filters applied to the given property.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    protected ValueFilter[]? GetPropertyFilterValuesInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyName = property.GetPropertyName();
        return PropertyFilters.FirstOrDefault(predicate => predicate.PropertyName == propertyName)?.ValueFilters;
    }

    /// <summary>
    /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested class <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    protected EntityFilter<TProperty>? GetNestedFilterInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyName = property.GetPropertyName();
        return (EntityFilter<TProperty>?)NestedFilters.FirstOrDefault(x => x.PropertyName == propertyName)?.EntityFilter;
    }

    /// <summary>
    /// Gets the <see cref="EntityFilter{TProperty}"/> for the given nested list <typeparamref name="TList"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TList">The type of the list of <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to get the filter for.</param>
    protected EntityFilter<TProperty>? GetNestedFilterInternal<TEntity, TList, TProperty>(Expression<Func<TEntity, TList?>> property)
        where TList : IEnumerable<TProperty>
    {
        var propertyName = property.GetPropertyName();
        return (EntityFilter<TProperty>?)NestedFilters.FirstOrDefault(x => x.PropertyName == propertyName)?.EntityFilter;
    }

    /// <summary>
    /// Adds a filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="valueFilters">The filters to use.</param>
    protected void AddInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property, ValueFilter[]? valueFilters)
    {
        var propertyName = property.GetPropertyName();
        PropertyFilters.Add(new PropertyFilter(propertyName, valueFilters));
    }

    /// <summary>
    /// Adds a nested filter for the given property. Existing filters for the same property are preserved.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <typeparam name="TNested">The nested type to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="nestedFilter">The nested class filter.</param>
    protected void AddNestedInternal<TEntity, TProperty, TNested>(Expression<Func<TEntity, TProperty?>> property, EntityFilter<TNested> nestedFilter)
    {
        var propertyName = property.GetPropertyName();
        NestedFilters.Add(new NestedFilter(propertyName, nestedFilter));
    }

    /// <summary>
    /// Replaces the filter for the given property using the default filter operator. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="valueFilters">The filters to use.</param>
    protected void ReplaceInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property, ValueFilter[]? valueFilters)
    {
        var propertyName = property.GetPropertyName();
        PropertyFilters.RemoveAll(x => x.PropertyName == propertyName);
        PropertyFilters.Add(new PropertyFilter(propertyName, valueFilters));
    }

    /// <summary>
    /// Replaces the nested filter for the given property. Existing filters for the same property are removed.
    /// </summary>
    /// <typeparam name="TEntity">The type to be filtered.</typeparam>
    /// <typeparam name="TProperty">The type of the property to be filtered.</typeparam>
    /// <typeparam name="TNested">The nested type to be filtered.</typeparam>
    /// <param name="property">The property to filter.</param>
    /// <param name="nestedFilter">The nested class filter.</param>
    protected void ReplaceNestedInternal<TEntity, TProperty, TNested>(Expression<Func<TEntity, TProperty?>> property, EntityFilter<TNested> nestedFilter)
    {
        var propertyName = property.GetPropertyName();
        NestedFilters.RemoveAll(x => x.PropertyName == propertyName);
        NestedFilters.Add(new NestedFilter(propertyName, nestedFilter));
    }

    /// <inheritdoc cref="EntityFilter{TEntity}.Remove{TProperty}(Expression{Func{TEntity, TProperty}})" />
    protected void RemoveInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyName = property.GetPropertyName();
        PropertyFilters.RemoveAll(x => x.PropertyName == propertyName);
    }

    /// <inheritdoc cref="EntityFilter{TEntity}.Clear" />
    protected void ClearInternal()
        => PropertyFilters.Clear();

    /// <inheritdoc cref="EntityFilter{TEntity}.CreateFilter" />
    protected internal Expression<Func<TEntity, bool>>? CreateFilter<TEntity>(IFilterInterceptor? interceptor, bool useAsCompiledExpression)
    {
        var configuration = Configuration ?? FilterConfiguration.Default ?? new FilterConfiguration();
        interceptor ??= IFilterInterceptor.Default;

        var useConditionalAccess = UseConditionalAccess(configuration, useAsCompiledExpression);

        var properties = typeof(TEntity)
            .GetProperties();

        var propertyFilters = properties
            .Reverse()
            .Join(
                PropertyFilters,
                x => x.Name,
                x => x.PropertyName,
                (propertyInfo, propertyFilter) => new { Property = propertyInfo, propertyFilter.ValueFilters }
            )
            .Select(x =>
                interceptor?.CreatePropertyFilter<TEntity>(x.Property, x.ValueFilters, configuration)
                ?? PropertyFilterExpression.PropertyFilterExpression.CreateFilter<TEntity>(x.Property, x.ValueFilters, configuration, interceptor)
            )
            .ToList();

        var nestedObjectFilters = properties
            .Reverse()
            .Where(x => !x.PropertyType.IsGenericIEnumerable())
            .Join(
                NestedFilters,
                x => x.Name,
                x => x.PropertyName,
                (propertyInfo, nestedFilter) => new { Property = propertyInfo, nestedFilter.EntityFilter }
            )
            .Select(x =>
            {
                var createFilterExpression = _createFilterMethod.MakeGenericMethod(x.Property.PropertyType);
                var nestedFilterExpression = (LambdaExpression)createFilterExpression.Invoke(x.EntityFilter, [interceptor, useAsCompiledExpression]);
                if (nestedFilterExpression == null)
                    return null;

                var propertySelector = typeof(TEntity).CreatePropertySelector(x.Property.Name);
                var propertyMatchesNested = (Expression<Func<TEntity, bool>>)nestedFilterExpression.ReplaceParameter(propertySelector);

                if (!useConditionalAccess)
                    return propertyMatchesNested;

                var propertyIsNotNull = propertySelector.IsNotNull(x.Property.PropertyType);
                var propertyIsNotNullLambda = propertySelector.CreateLambda<TEntity, bool>(propertyIsNotNull);

                var filterExpression = new[] { propertyIsNotNullLambda, propertyMatchesNested }.CombineWithConditionalAnd();
                return filterExpression;
            })
            .ToList();

        var nestedListsFilters = properties
            .Reverse()
            .Where(x => x.PropertyType.IsGenericIEnumerable())
            .Join(
                NestedFilters,
                x => x.Name,
                x => x.PropertyName,
                (propertyInfo, nestedFilter) => new { Property = propertyInfo, nestedFilter.EntityFilter }
            )
            .Select(x =>
            {
                var propertyType = x.Property.PropertyType.GetGenericArguments()[0];
                var createFilterExpression = _createFilterMethod.MakeGenericMethod(propertyType);
                var nestedFilterExpression = (LambdaExpression)createFilterExpression.Invoke(x.EntityFilter, [interceptor, useAsCompiledExpression]);
                if (nestedFilterExpression == null)
                    return null;

                var propertySelector = typeof(TEntity).CreatePropertySelector(x.Property.Name);
                var propertyHasAnyNested = (Expression<Func<TEntity, bool>>)propertySelector.EnumerableAny(propertyType, nestedFilterExpression);

                if (!useConditionalAccess)
                    return propertyHasAnyNested;

                var propertyIsNotNull = propertySelector.IsNotNull(x.Property.PropertyType);
                var propertyIsNotNullLambda = propertySelector.CreateLambda<TEntity, bool>(propertyIsNotNull);

                var filterExpression = new[] { propertyIsNotNullLambda, propertyHasAnyNested }.CombineWithConditionalAnd();
                return filterExpression;
            })
            .ToList();

        return propertyFilters
            .Concat(nestedObjectFilters)
            .Concat(nestedListsFilters)
            .CombineWithConditionalAnd();
    }

    private static bool UseConditionalAccess(FilterConfiguration configuration, bool usedAsCompiledExpression)
        => configuration.UseConditionalAccess switch
        {
            FilterConditionalAccess.Always => true,
            FilterConditionalAccess.Never => false,
            _ => usedAsCompiledExpression
        };
}
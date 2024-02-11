using Schick.Plainquire.Sort.Abstractions.Configurations;
using Schick.Plainquire.Sort.Enums;
using Schick.Plainquire.Sort.Extensions;
using Schick.Plainquire.Sort.Interfaces;
using Schick.Plainquire.Sort.JsonConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Schick.Plainquire.Sort.Sorts;

/// <summary>
/// Hub to create sort order for <typeparamref name="TEntity"/> with fluent API.
/// </summary>
/// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
[JsonConverter(typeof(EntitySortConverterFactory))]
public class EntitySort<TEntity> : EntitySort
{
    /// <summary>
    /// Gets the sort order syntax for the given <paramref name="property"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property sorted by.</typeparam>
    /// <param name="property">The property to get the sort order for.</param>
    public string? GetPropertySortSyntax<TProperty>(Expression<Func<TEntity, TProperty?>> property)
        => GetPropertySortSyntaxInternal(property);

    /// <summary>
    /// Get the sort order applied to the given property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property sorted by.</typeparam>
    /// <param name="property">The property to get the sort order for.</param>
    public SortDirection? GetPropertySortDirection<TProperty>(Expression<Func<TEntity, TProperty?>> property)
        => GetPropertySortDirectionInternal(property);

    /// <summary>
    /// Gets the <see cref="EntitySort{TProperty}"/> for the given nested class <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property sorted by.</typeparam>
    /// <param name="property">The property to get the sort order for.</param>
    public EntitySort<TProperty> GetNested<TProperty>(Expression<Func<TEntity, TProperty?>> property)
        => GetNestedInternal(property);

    /// <summary>
    /// Adds the sort order for the given property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    /// <param name="property">The property to order.</param>
    /// <param name="direction">The sort order to use.</param>
    /// <param name="position">The sort position of the property.</param>
    public EntitySort<TEntity> Add<TProperty>(Expression<Func<TEntity, TProperty?>> property, SortDirection? direction = SortDirection.Ascending, int? position = null)
    {
        AddInternal(property, direction, position);
        return this;
    }

    /// <summary>
    /// Adds the sort order for the given property.
    /// </summary>
    /// <param name="syntax">The sort order syntax.</param>
    /// <param name="position">The sort position of the property.</param>
    public EntitySort<TEntity> Add(string syntax, int? position = null)
    {
        AddInternal(syntax, position);
        return this;
    }

    /// <summary>
    /// Replaces the nested sort order for the given property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    /// <param name="property">The property to order.</param>
    /// <param name="nestedSort">The nested class sort order.</param>
    public EntitySort<TEntity> AddNested<TProperty>(Expression<Func<TEntity, TProperty?>> property, EntitySort<TProperty> nestedSort)
    {
        AddNestedInternal(property, nestedSort);
        return this;
    }

    /// <summary>
    /// Remove the sort order for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to remove sorting from.</typeparam>
    /// <param name="property">The property to remove the sort order for.</param>
    public EntitySort<TEntity> Clear<TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        ClearInternal(property);
        return this;
    }

    /// <summary>
    /// Removes the sort order for all properties.
    /// </summary>
    public EntitySort<TEntity> Clear()
    {
        ClearInternal();
        return this;
    }

    /// <summary>
    /// Remove sort orders for given property including all navigation properties.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="property"></param>
    public EntitySort<TEntity> ClearNested<TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        ClearNestedInternal(property);
        return this;
    }

    /// <summary>
    /// Creates a deep clone of this filter.
    /// </summary>
    public EntitySort<TEntity> Clone()
        => JsonSerializer.Deserialize<EntitySort<TEntity>>(JsonSerializer.Serialize(this))!;

    /// <summary>
    /// Casts this filter to a different entity type (by creating a deep clone).
    /// Filtered properties are matched by type (check if assignable) and name (case-sensitive).
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination entity to filter.</typeparam>
    public EntitySort<TDestination> Cast<TDestination>()
        => (EntitySort<TDestination>)CastInternal(this, typeof(TEntity), typeof(TDestination));
}

/// <inheritdoc cref="EntitySort{TEntity}" />
[JsonConverter(typeof(EntitySortConverter))]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class EntitySort
{
    internal List<PropertySort> _propertySorts;

    /// <summary>
    /// Gets or sets the default configuration. Can be used to set a system-wide configuration.
    /// </summary>
    public static SortConfiguration DefaultConfiguration { get; set; } = new();

    /// <summary>
    /// Gets or sets the default interceptor. Can be used to set a system-wide interceptor.
    /// </summary>
    public static IPropertySortQueryableInterceptor? DefaultInterceptor { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySort"/> class.
    /// </summary>
    public EntitySort()
        => _propertySorts = [];

    /// <summary>
    /// Indicates whether this entity sort is empty.
    /// </summary>
    public bool IsEmpty() => !_propertySorts.Any();

    /// <inheritdoc />
    public override string ToString()
    {
        var sortStrings = _propertySorts
            .OrderBy(x => x.Position)
            .Select(sort => sort.ToString())
            .ToList();

        return string.Join(", ", sortStrings);
    }

    /// <summary>
    /// Gets the sort order syntax for the given <paramref name="property"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    /// <param name="property">The property to get the sort order for.</param>
    protected string? GetPropertySortSyntaxInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyPath = property.GetPropertyPath();
        var propertySort = _propertySorts
            .OrderBy(x => x.Position)
            .LastOrDefault(sort => sort.PropertyPath == propertyPath);

        return propertySort?.ToString();
    }

    /// <summary>
    /// Get the sort direction applied to the given property.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
    /// <typeparam name="TProperty">The type of the property sorted by.</typeparam>
    /// <param name="property">The property to get the sort order for.</param>
    protected SortDirection? GetPropertySortDirectionInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyPath = property.GetPropertyPath();
        return _propertySorts
            .OrderBy(x => x.Position)
            .LastOrDefault(sort => sort.PropertyPath == propertyPath)?.Direction;
    }

    /// <summary>
    /// Gets the <see cref="EntitySort{TProperty}"/> for the given nested class <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
    /// <typeparam name="TProperty">The type of the property sorted by.</typeparam>
    /// <param name="property">The property to get the sort order for.</param>
    protected EntitySort<TProperty> GetNestedInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyPath = property.GetPropertyPath();
        return (EntitySort<TProperty>)GetNestedInternal(typeof(TProperty), propertyPath);
    }

    /// <summary>
    /// Replaces the sort order for the given property.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    /// <param name="property">The property to order.</param>
    /// <param name="direction">The sort order to use.</param>
    /// <param name="position">The sort position of the property.</param>
    protected void AddInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property, SortDirection? direction, int? position)
    {
        var propertyPath = property.GetPropertyPath();
        position = GetPosition(position);
        var propertySort = PropertySort.Create(propertyPath, direction ?? SortDirection.Ascending, position);
        _propertySorts.Add(propertySort);
    }

    /// <summary>
    /// Adds the sort order for the given property.
    /// </summary>
    /// <param name="syntax">The sort order syntax.</param>
    /// <param name="position">The sort position of the property.</param>
    protected void AddInternal(string syntax, int? position)
    {
        position = GetPosition(position);
        var propertySort = PropertySort.Create(syntax, position);
        _propertySorts.Add(propertySort);
    }

    /// <summary>
    /// Adds the nested sort order for the given property
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    /// <typeparam name="TNested">The nested entity type to be sorted.</typeparam>
    /// <param name="property">The property to set sort for.</param>
    /// <param name="nestedSort">The nested class sort order.</param>
    protected void AddNestedInternal<TEntity, TProperty, TNested>(Expression<Func<TEntity, TProperty?>> property, EntitySort<TNested> nestedSort)
    {
        var propertyPath = property.GetPropertyPath();
        AddNestedInternal(propertyPath, nestedSort);
    }

    /// <summary>
    /// Remove the sort order for the specified property.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be sorted.</typeparam>
    /// <typeparam name="TProperty">The type of the property to remove sorting from.</typeparam>
    /// <param name="property">The property to remove the sort order for.</param>
    protected void ClearInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyPath = property.GetPropertyPath();
        _propertySorts.RemoveAll(x => x.PropertyPath == propertyPath);
    }

    /// <inheritdoc cref="EntitySort{TEntity}.Clear" />
    protected void ClearInternal()
        => _propertySorts.Clear();

    /// <inheritdoc cref="EntitySort{TEntity}.ClearNested{TProperty}(Expression{Func{TEntity, TProperty}})" />/>
    protected void ClearNestedInternal<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> property)
    {
        var propertyPath = property.GetPropertyPath();
        _propertySorts.RemoveAll(x => x.BelongsTo(propertyPath));
    }

    /// <summary>
    /// Casts the given <paramref name="sourceSort"/> to the given <paramref name="destinationType"/>.
    /// </summary>
    /// <param name="sourceSort">Source to cast.</param>
    /// <param name="sourceType">Type of <paramref name="sourceSort"/>.</param>
    /// <param name="destinationType">Type to cast to.</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected static EntitySort CastInternal(EntitySort sourceSort, Type sourceType, Type destinationType)
    {
        if (sourceSort == null)
            throw new ArgumentNullException(nameof(sourceSort));

        var destinationSourceOrderType = typeof(EntitySort<>).MakeGenericType(destinationType);
        var destinationSort = (EntitySort)JsonSerializer.Deserialize(JsonSerializer.Serialize(sourceSort), destinationSourceOrderType)!;

        var sourceProperties = sourceType.GetProperties();
        var destinationProperties = destinationType.GetProperties().ToList();

        foreach (var sourceProperty in sourceProperties)
        {
            var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);
            if (destinationProperty == null)
            {
                RemoveRelatedProperties(destinationSort, sourceProperty.Name);
                continue;
            }

            var destinationIsAssignableFromSource = destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType);
            if (destinationIsAssignableFromSource)
                continue;

            var nestedSourceSort = destinationSort.GetNestedInternal(sourceProperty.PropertyType, sourceProperty.Name);

            var removedPropertyCount = RemoveRelatedProperties(destinationSort, sourceProperty.Name);
            if (removedPropertyCount == 0)
                continue;

            var nestedDestinationSort = CastInternal(nestedSourceSort, sourceProperty.PropertyType, destinationProperty.PropertyType);
            destinationSort.AddNestedInternal(destinationProperty.Name, nestedDestinationSort);
        }

        return destinationSort;
    }

    /// <summary>
    /// Gets the <see cref="EntitySort{TProperty}"/> for the given nested class <paramref name="propertyType"/>.
    /// </summary>
    /// <param name="propertyType">The type of the property returned by <paramref name="propertyPath"/>.</param>
    /// <param name="propertyPath">Path to the nested property to set sort for.</param>
    /// <returns></returns>
    private EntitySort GetNestedInternal(Type propertyType, string propertyPath)
    {
        var relatedProperties = _propertySorts
            .Where(sort => sort.BelongsTo(propertyPath))
            .ToList();

        var nestedSorts = relatedProperties
            .Select(sort =>
            {
                var nestedPropertyPath = propertyPath == sort.PropertyPath
                    ? PropertySort.PATH_TO_SELF
                    : sort.PropertyPath[(propertyPath.Length + 1)..];
                return PropertySort.Create(nestedPropertyPath, sort.Direction, sort.Position);
            })
            .ToList();

        var genericSort = typeof(EntitySort<>).MakeGenericType(propertyType);
        var nestedSort = (EntitySort)Activator.CreateInstance(genericSort);
        nestedSort._propertySorts.AddRange(nestedSorts);

        return nestedSort;
    }

    /// <summary>
    /// Adds the nested sort order for the given property.
    /// </summary>
    /// <param name="propertyPath">Path to the nested property to set sort for.</param>
    /// <param name="nestedSort">The nested <see cref="EntitySort"/>.</param>
    /// <exception cref="ArgumentNullException"></exception>
    private void AddNestedInternal(string propertyPath, EntitySort nestedSort)
    {
        if (nestedSort == null)
            throw new ArgumentNullException(nameof(nestedSort));

        foreach (var propertySort in nestedSort._propertySorts)
        {
            var path = propertyPath;
            if (propertySort.PropertyPath != PropertySort.PATH_TO_SELF)
                path += "." + propertySort.PropertyPath;

            _propertySorts.Add(PropertySort.Create(path, propertySort.Direction, propertySort.Position));
        }
    }

    private int GetPosition(int? position)
        => position ?? (_propertySorts.Any() ? _propertySorts.Max(x => x.Position) + 1 : 0);

    private static int RemoveRelatedProperties(EntitySort sort, string propertyName)
        => sort._propertySorts.RemoveAll(propertySort => propertySort.BelongsTo(propertyName));

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            if (_propertySorts.Count == 0)
                return "<EMPTY>";

            var sortStrings = _propertySorts
                .OrderBy(x => x.Position)
                .Select(sort => $"{sort.Position}: {sort}")
                .ToList();

            return string.Join(", ", sortStrings);
        }
    }
}
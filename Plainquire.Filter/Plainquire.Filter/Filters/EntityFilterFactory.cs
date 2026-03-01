using Plainquire.Filter.Abstractions;
using System;

namespace Plainquire.Filter;

/// <summary>
/// Factory for creating instances of <see cref="EntityFilter{TEntity}"/>.
/// </summary>
public static class EntityFilterFactory
{
    /// <summary>
    /// Creates an instance of <see cref="EntityFilter{TEntity}"/>.
    /// </summary>
    /// <param name="entityType">The type of entity to filter</param>
    /// <param name="configuration">The configuration to assign to the created filter</param>
    public static EntityFilter Create(Type? entityType, FilterConfiguration? configuration)
    {
        if (entityType == null)
            return new EntityFilter();

        var entityFilterType = typeof(EntityFilter<>).MakeGenericType(entityType);
        var entityFilterInstance = Activator.CreateInstance(entityFilterType)
            ?? throw new InvalidOperationException($"Unable to create instance of type {entityFilterType.Name}");

        var entityFilter = (EntityFilter)entityFilterInstance;
        entityFilter.Configuration = configuration;

        return entityFilter;
    }
}

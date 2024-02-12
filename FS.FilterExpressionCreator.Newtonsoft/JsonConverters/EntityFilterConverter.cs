using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Newtonsoft.JsonConverters;

/// <summary>
/// <see cref="EntityFilter{TEntity}"/> specific JSON converter for Newtonsoft JSON.
/// Implements <see cref="JsonConverter" />
/// </summary>
/// <seealso cref="JsonConverter" />
[Obsolete("Use 'Plainquire.Filter.Newtonsoft.JsonConverters.EntityFilterConverter' instead.")]
public class EntityFilterConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
        => objectType.IsGenericEntityFilter() || objectType == typeof(EntityFilter);

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, [NotNull] object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var entityFilter = (EntityFilter)value;
        var entityFilterData = new EntityFilterData { PropertyFilters = entityFilter.PropertyFilters, NestedFilters = entityFilter.NestedFilters };
        serializer.Serialize(writer, entityFilterData);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var entityFilter = (EntityFilter)Activator.CreateInstance(objectType);
        var entityFilterData = serializer.Deserialize<EntityFilterData>(reader) ?? new EntityFilterData();
        entityFilter.PropertyFilters = entityFilterData.PropertyFilters ?? [];
        entityFilter.NestedFilters = entityFilterData.NestedFilters ?? [];
        return entityFilter;
    }

    private class EntityFilterData
    {
        public List<PropertyFilter>? PropertyFilters { get; set; } = [];

        public List<NestedFilter>? NestedFilters { get; set; } = [];
    }
}
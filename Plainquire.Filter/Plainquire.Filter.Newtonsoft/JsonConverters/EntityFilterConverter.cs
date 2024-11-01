using Newtonsoft.Json;
using Plainquire.Filter.JsonConverters;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Newtonsoft.JsonConverters;

/// <summary>
/// <see cref="EntityFilter{TEntity}"/> specific JSON converter for Newtonsoft JSON.
/// Implements <see cref="JsonConverter" />
/// </summary>
/// <seealso cref="JsonConverter" />
public class EntityFilterConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
        => objectType.IsGenericEntityFilter() || objectType == typeof(EntityFilter);

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var entityFilterInstance = Activator.CreateInstance(objectType)
            ?? throw new InvalidOperationException($"Unable to create instance of type {objectType.Name}");

        var entityFilter = (EntityFilter)(entityFilterInstance);
        var entityFilterData = serializer.Deserialize<EntityFilterConverterData>(reader) ?? new EntityFilterConverterData();
        var propertyFilters = Filter.JsonConverters.EntityFilterConverter.GetPropertyFilters(entityFilterData);

        entityFilter.PropertyFilters = propertyFilters ?? [];
        entityFilter.NestedFilters = entityFilterData.NestedFilters ?? [];
        entityFilter.Configuration = entityFilterData.Configuration;
        return entityFilter;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, [NotNull] object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var entityFilter = (EntityFilter)value;
        var propertyFiltersData = Filter.JsonConverters.EntityFilterConverter.GetPropertyFilterData(entityFilter);

        var entityFilterData = new EntityFilterConverterData
        {
            PropertyFilters = propertyFiltersData,
            NestedFilters = entityFilter.NestedFilters,
            Configuration = entityFilter.Configuration
        };

        serializer.Serialize(writer, entityFilterData);
    }
}
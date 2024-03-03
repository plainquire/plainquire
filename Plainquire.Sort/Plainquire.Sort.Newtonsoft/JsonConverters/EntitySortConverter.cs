using Newtonsoft.Json;
using Plainquire.Sort.JsonConverters;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Newtonsoft.JsonConverters;

/// <summary>
/// <see cref="EntitySort"/> specific JSON converter for Newtonsoft JSON.
/// Implements <see cref="JsonConverter" />
/// </summary>
/// <seealso cref="JsonConverter" />
public class EntitySortConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
        => objectType.IsGenericEntitySort() || objectType == typeof(EntitySort);

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var entitySort = (EntitySort)Activator.CreateInstance(objectType);
        var entitySortData = serializer.Deserialize<EntitySortConverterData>(reader) ?? new EntitySortConverterData();
        var propertySorts = Sort.JsonConverters.EntitySortConverter.GetPropertySorts(entitySortData);

        entitySort.PropertySorts = propertySorts ?? [];
        entitySort.Configuration = entitySortData.Configuration;
        return entitySort;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, [NotNull] object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var entitySort = (EntitySort)value;
        var propertySortsData = Sort.JsonConverters.EntitySortConverter.GetPropertySortData(entitySort);

        var entitySortData = new EntitySortConverterData
        {
            PropertySorts = propertySortsData,
            Configuration = entitySort.Configuration
        };

        serializer.Serialize(writer, entitySortData);
    }
}
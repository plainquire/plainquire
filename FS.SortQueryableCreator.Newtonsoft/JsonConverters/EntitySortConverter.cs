using FS.SortQueryableCreator.Extensions;
using FS.SortQueryableCreator.Sorts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FS.SortQueryableCreator.Newtonsoft.JsonConverters;

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
    public override void WriteJson(JsonWriter writer, [NotNull] object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var entitySort = (EntitySort)value;
        var entitySortData = new EntitySortData { PropertySorts = entitySort._propertySorts };
        serializer.Serialize(writer, entitySortData);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var entitySort = (EntitySort)Activator.CreateInstance(objectType);
        var entitySortData = serializer.Deserialize<EntitySortData>(reader) ?? new EntitySortData();
        entitySort._propertySorts = entitySortData.PropertySorts ?? [];
        return entitySort;
    }

    private class EntitySortData
    {
        public List<PropertySort>? PropertySorts { get; set; } = [];
    }
}
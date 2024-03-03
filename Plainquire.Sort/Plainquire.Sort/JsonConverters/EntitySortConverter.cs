using Plainquire.Sort.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plainquire.Sort.JsonConverters;

/// <summary>
/// <see cref="EntitySort{TEntity}"/> specific JSON converter factory for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverterFactory" />
/// </summary>
/// <seealso cref="JsonConverterFactory" />
public class EntitySortConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericEntitySort();

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var entityType = typeToConvert.GetGenericArguments()[0];
        var entitySortConverterType = typeof(EntitySortConverter<>).MakeGenericType(entityType);
        return (JsonConverter)Activator.CreateInstance(entitySortConverterType);
    }
}

/// <summary>
/// <see cref="EntitySort{TEntity}"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <typeparam name="TEntity">The type of the entity be sorted.</typeparam>
/// <seealso cref="JsonConverter{T}" />
public class EntitySortConverter<TEntity> : JsonConverter<EntitySort<TEntity>>
{
    /// <inheritdoc />
    public override EntitySort<TEntity> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => EntitySortConverter.Read<EntitySort<TEntity>>(ref reader, options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EntitySort<TEntity> value, JsonSerializerOptions options)
        => EntitySortConverter.Write(writer, value, options);
}

/// <summary>
/// <see cref="EntitySort"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <seealso cref="JsonConverter{T}" />
public class EntitySortConverter : JsonConverter<EntitySort>
{
    /// <inheritdoc />
    public override EntitySort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read<EntitySort>(ref reader, options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EntitySort value, JsonSerializerOptions options)
        => Write(writer, value, options);

    internal static TEntitySort Read<TEntitySort>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        where TEntitySort : EntitySort, new()
    {
        var entitySortData = JsonSerializer.Deserialize<EntitySortConverterData>(ref reader, options) ?? new EntitySortConverterData();
        var propertySorts = GetPropertySorts(entitySortData);

        return new TEntitySort
        {
            PropertySorts = propertySorts ?? [],
            Configuration = entitySortData.Configuration
        };
    }

    internal static void Write<TEntitySort>(Utf8JsonWriter writer, TEntitySort value, JsonSerializerOptions options)
        where TEntitySort : EntitySort
    {
        var propertySortData = GetPropertySortData(value);

        var entitySortData = new EntitySortConverterData
        {
            PropertySorts = propertySortData,
            Configuration = value.Configuration
        };

        JsonSerializer.Serialize(writer, entitySortData, options);
    }

    internal static List<PropertySort>? GetPropertySorts(EntitySortConverterData entitySortData)
        => entitySortData
            .PropertySorts?
            .Select(filter => new PropertySort(
                propertyPath: filter.PropertyPath,
                direction: filter.Direction,
                position: filter.Position,
                configuration: entitySortData.Configuration
            ))
            .ToList();

    internal static List<PropertySortConverterData> GetPropertySortData<TEntitySort>(TEntitySort entitySort) where TEntitySort : EntitySort
        => entitySort.PropertySorts
            .Select(filter => new PropertySortConverterData
            (
                propertyPath: filter.PropertyPath,
                direction: filter.Direction,
                position: filter.Position
            ))
            .ToList();
}

internal class EntitySortConverterData
{
    public List<PropertySortConverterData>? PropertySorts { get; set; } = [];
    public SortConfiguration? Configuration { get; set; }
}

internal class PropertySortConverterData
{
    public string PropertyPath { get; set; }
    public SortDirection Direction { get; set; }
    public int Position { get; set; }

    public PropertySortConverterData(string propertyPath, SortDirection direction, int position)
    {
        PropertyPath = propertyPath;
        Direction = direction;
        Position = position;
    }
}
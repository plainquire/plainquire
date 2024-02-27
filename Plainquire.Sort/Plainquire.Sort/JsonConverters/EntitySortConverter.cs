using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plainquire.Sort.Extensions;
using Plainquire.Sort.Sorts;

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
        var entitySortData = JsonSerializer.Deserialize<EntitySortData>(ref reader, options) ?? new EntitySortData();
        return new TEntitySort
        {
            _propertySorts = entitySortData.PropertySorts ?? []
        };
    }

    internal static void Write<TEntitySort>(Utf8JsonWriter writer, TEntitySort value, JsonSerializerOptions options)
        where TEntitySort : EntitySort
    {
        var entitySortData = new EntitySortData { PropertySorts = value._propertySorts };
        JsonSerializer.Serialize(writer, entitySortData, options);
    }

    private class EntitySortData
    {
        public List<PropertySort>? PropertySorts { get; set; } = [];
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plainquire.Filter.JsonConverters;

/// <summary>
/// <see cref="EntityFilter{TEntity}"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <typeparam name="TEntity">The type of the entity to filter for.</typeparam>
/// <seealso cref="JsonConverter{T}" />
public class EntityFilterConverter<TEntity> : JsonConverter<EntityFilter<TEntity>>
{
    /// <inheritdoc />
    public override EntityFilter<TEntity> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => EntityFilterConverter.Read<EntityFilter<TEntity>>(ref reader, options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EntityFilter<TEntity> value, JsonSerializerOptions options)
        => EntityFilterConverter.Write(writer, value, options);
}

/// <summary>
/// <see cref="EntityFilter"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <seealso cref="JsonConverter{T}" />
public class EntityFilterConverter : JsonConverter<EntityFilter>
{
    /// <inheritdoc />
    public override EntityFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read<EntityFilter>(ref reader, options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EntityFilter value, JsonSerializerOptions options)
        => Write(writer, value, options);

    internal static TEntityFilter Read<TEntityFilter>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        where TEntityFilter : EntityFilter, new()
    {
        var entityFilterData = JsonSerializer.Deserialize<EntityFilterConverterData>(ref reader, options) ?? new EntityFilterConverterData();
        var propertyFilters = GetPropertyFilters(entityFilterData);

        return new TEntityFilter
        {
            PropertyFilters = propertyFilters ?? [],
            NestedFilters = entityFilterData.NestedFilters ?? [],
            Configuration = entityFilterData.Configuration
        };
    }

    internal static void Write<TEntityFilter>(Utf8JsonWriter writer, TEntityFilter value, JsonSerializerOptions options)
        where TEntityFilter : EntityFilter
    {
        var propertyFiltersData = GetPropertyFilterData(value);

        var entityFilterData = new EntityFilterConverterData
        {
            PropertyFilters = propertyFiltersData,
            NestedFilters = value.NestedFilters,
            Configuration = value.Configuration
        };

        JsonSerializer.Serialize(writer, entityFilterData, options);
    }

    internal static List<PropertyFilter>? GetPropertyFilters(EntityFilterConverterData entityFilterData)
        => entityFilterData
            .PropertyFilters?
            .Select(filter => new PropertyFilter(
                propertyName: filter.PropertyName,
                valueFilters: filter.ValueFilters
                    .Select(valueFilter => ValueFilter.Create(
                        valueFilter.Operator,
                        valueFilter.Value,
                        entityFilterData.Configuration
                    ))
                    .ToArray()
            ))
            .ToList();

    internal static List<PropertyFilterConverterData> GetPropertyFilterData<TEntityFilter>(TEntityFilter entityFilter) where TEntityFilter : EntityFilter
        => entityFilter.PropertyFilters
            .Select(filter => new PropertyFilterConverterData
            (
                propertyName: filter.PropertyName,
                valueFilters: filter.ValueFilters
                    .Select(valueFilter => new ValueFilterConverterData { Operator = valueFilter.Operator, Value = valueFilter.Value })
                    .ToList()
            ))
            .ToList();

    /// <summary>
    /// <see cref="EntityFilter{TEntity}"/> specific JSON converter factory for Microsoft (System.Text.Json) JSON.
    /// Implements <see cref="JsonConverterFactory" />
    /// </summary>
    /// <seealso cref="JsonConverterFactory" />
    public class Factory : JsonConverterFactory
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericEntityFilter();

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var entityType = typeToConvert.GetGenericArguments()[0];
            var entityFilterConverterType = typeof(EntityFilterConverter<>).MakeGenericType(entityType);
            return (JsonConverter)Activator.CreateInstance(entityFilterConverterType);
        }
    }
}
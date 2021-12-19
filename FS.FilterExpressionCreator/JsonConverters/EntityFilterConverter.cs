using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FS.FilterExpressionCreator.JsonConverters
{
    /// <summary>
    /// <see cref="EntityFilter{TEntity}"/> specific JSON converter factory for Microsoft (System.Text.Json) JSON.
    /// Implements <see cref="JsonConverterFactory" />
    /// </summary>
    /// <seealso cref="JsonConverterFactory" />
    public class EntityFilterConverterFactory : JsonConverterFactory
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
            var entityFilterData = JsonSerializer.Deserialize<EntityFilterData>(ref reader, options) ?? new EntityFilterData();
            return new TEntityFilter()
            {
                PropertyFilters = entityFilterData.PropertyFilters ?? new List<PropertyFilter>(),
                NestedFilters = entityFilterData.NestedFilters ?? new List<NestedFilter>()
            };
        }

        internal static void Write<TEntityFilter>(Utf8JsonWriter writer, TEntityFilter value, JsonSerializerOptions options)
            where TEntityFilter : EntityFilter
        {
            var entityFilterData = new EntityFilterData { PropertyFilters = value.PropertyFilters, NestedFilters = value.NestedFilters };
            JsonSerializer.Serialize(writer, entityFilterData, options);
        }

        private class EntityFilterData
        {
            public List<PropertyFilter> PropertyFilters { get; set; } = new List<PropertyFilter>();

            public List<NestedFilter> NestedFilters { get; set; } = new List<NestedFilter>();
        }
    }
}

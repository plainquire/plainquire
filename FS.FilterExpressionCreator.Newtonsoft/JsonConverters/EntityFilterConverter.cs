using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FS.FilterExpressionCreator.Newtonsoft.JsonConverters
{
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
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var entityFilter = (EntityFilter)value;
            var entityFilterData = new EntityFilterData { PropertyFilters = entityFilter.PropertyFilters, NestedFilters = entityFilter.NestedFilters };
            serializer.Serialize(writer, entityFilterData);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var entityFilter = (EntityFilter)Activator.CreateInstance(objectType);
            var entityFilterData = serializer.Deserialize<EntityFilterData>(reader) ?? new EntityFilterData();
            entityFilter.PropertyFilters = entityFilterData.PropertyFilters ?? new List<EntityFilter.PropertyFilter>();
            entityFilter.NestedFilters = entityFilterData.NestedFilters ?? new List<EntityFilter.NestedFilter>();
            return entityFilter;
        }

        private class EntityFilterData
        {
            public List<EntityFilter.PropertyFilter> PropertyFilters { get; set; } = new List<EntityFilter.PropertyFilter>();

            public List<EntityFilter.NestedFilter> NestedFilters { get; set; } = new List<EntityFilter.NestedFilter>();
        }
    }
}

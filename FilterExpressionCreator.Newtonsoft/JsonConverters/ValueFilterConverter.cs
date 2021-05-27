using FilterExpressionCreator.Models;
using Newtonsoft.Json;
using System;

namespace FilterExpressionCreator.Newtonsoft.JsonConverters
{
    /// <summary>
    /// <see cref="ValueFilter"/> specific JSON converter for Newtonsoft JSON.
    /// Implements <see cref="JsonConverter{T}" />
    /// </summary>
    /// <seealso cref="JsonConverter{T}" />
    public class ValueFilterConverter : JsonConverter<ValueFilter>
    {
        /// <inheritdoc />
        public override ValueFilter ReadJson(JsonReader reader, Type objectType, ValueFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var filterSyntax = reader.Value switch
            {
                DateTime dtValue => dtValue.ToString("o"),
                _ => (string)reader.Value
            };

            return ValueFilter.Create(filterSyntax);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, ValueFilter value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString());
    }
}

using FS.FilterExpressionCreator.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using FS.FilterExpressionCreator.Filters;

namespace FS.FilterExpressionCreator.JsonConverters
{
    /// <summary>
    /// <see cref="ValueFilter"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
    /// Implements <see cref="JsonConverter{T}" />
    /// </summary>
    /// <seealso cref="JsonConverter{T}" />
    public class ValueFilterConverter : JsonConverter<ValueFilter>
    {
        /// <inheritdoc />
        public override ValueFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => ValueFilter.Create(reader.GetString());

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ValueFilter value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}

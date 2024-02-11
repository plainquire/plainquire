using Schick.Plainquire.Sort.Enums;
using Schick.Plainquire.Sort.Sorts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Schick.Plainquire.Sort.Newtonsoft.JsonConverters;

/// <summary>
/// <see cref="PropertySort"/> specific JSON converter for Newtonsoft JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <seealso cref="JsonConverter{T}" />
public class PropertySortConverter : JsonConverter<PropertySort>
{
    /// <inheritdoc />
    public override PropertySort? ReadJson(JsonReader reader, Type objectType, PropertySort? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jToken = JToken.ReadFrom(reader);
        return jToken.Type switch
        {
            JTokenType.Null => null,
            JTokenType.Object => CreatePropertySort(jToken),
            _ => throw new InvalidOperationException("Unable to read object from reader.")
        };
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, PropertySort? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            JObject.FromObject(value).WriteTo(writer);
    }

    private static PropertySort CreatePropertySort(JToken jToken)
    {
        var propertyPath = jToken.Value<string>(nameof(PropertySort.PropertyPath));
        var direction = (SortDirection)jToken.Value<int>(nameof(PropertySort.Direction));
        var position = jToken.Value<int>(nameof(PropertySort.Position));
        return PropertySort.Create(propertyPath!, direction, position);
    }
}
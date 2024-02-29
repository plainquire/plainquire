using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Newtonsoft.JsonConverters;

/// <summary>
/// <see cref="EntityPage"/> specific JSON converter for Newtonsoft JSON.
/// Implements <see cref="JsonConverter" />
/// </summary>
/// <seealso cref="JsonConverter" />
public class EntityPageConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
        => objectType.IsEntityPage() || objectType == typeof(EntityPage);

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, [NotNull] object? value, JsonSerializer serializer)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var entityPage = (EntityPage)value;
        var entityPageData = new EntityPageData { PageNumber = entityPage.PageNumberValue, PageSize = entityPage.PageSizeValue };
        serializer.Serialize(writer, entityPageData);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var entityPage = (EntityPage)Activator.CreateInstance(objectType);
        var entityPageData = serializer.Deserialize<EntityPageData>(reader) ?? new EntityPageData();
        entityPage.PageNumberValue = entityPageData.PageNumber ?? string.Empty;
        entityPage.PageSizeValue = entityPageData.PageSize ?? string.Empty;
        return entityPage;
    }

    private class EntityPageData
    {
        public string? PageNumber { get; set; } = string.Empty;
        public string? PageSize { get; set; } = string.Empty;
    }
}
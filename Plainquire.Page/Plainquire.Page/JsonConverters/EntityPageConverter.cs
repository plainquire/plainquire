using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plainquire.Page.Extensions;
using Plainquire.Page.Pages;

namespace Plainquire.Page.JsonConverters;

/// <summary>
/// <see cref="EntityPage{TEntity}"/> specific JSON converter factory for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverterFactory" />
/// </summary>
/// <seealso cref="JsonConverterFactory" />
public class EntityPageConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsEntityPage();

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var entityType = typeToConvert.GetGenericArguments()[0];
        var entityPageConverterType = typeof(EntityPageConverter<>).MakeGenericType(entityType);
        return (JsonConverter)Activator.CreateInstance(entityPageConverterType);
    }
}

/// <summary>
/// <see cref="EntityPage{TEntity}"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <typeparam name="TEntity">The type of the entity be paged.</typeparam>
/// <seealso cref="JsonConverter{T}" />
public class EntityPageConverter<TEntity> : JsonConverter<EntityPage<TEntity>>
{
    /// <inheritdoc />
    public override EntityPage<TEntity> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => EntityPageConverter.Read<EntityPage<TEntity>>(ref reader, options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EntityPage<TEntity> value, JsonSerializerOptions options)
        => EntityPageConverter.Write(writer, value, options);
}

/// <summary>
/// <see cref="EntityPage"/> specific JSON converter for Microsoft (System.Text.Json) JSON.
/// Implements <see cref="JsonConverter{T}" />
/// </summary>
/// <seealso cref="JsonConverter{T}" />
public class EntityPageConverter : JsonConverter<EntityPage>
{
    /// <inheritdoc />
    public override EntityPage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read<EntityPage>(ref reader, options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EntityPage value, JsonSerializerOptions options)
        => Write(writer, value, options);

    internal static TEntityPage Read<TEntityPage>(ref Utf8JsonReader reader, JsonSerializerOptions options)
        where TEntityPage : EntityPage, new()
    {
        var entityPageData = JsonSerializer.Deserialize<EntityPageData>(ref reader, options) ?? new EntityPageData();
        return new TEntityPage
        {
            PageNumberValue = entityPageData.PageNumber ?? string.Empty,
            PageSizeValue = entityPageData.PageSize ?? string.Empty,
        };
    }

    internal static void Write<TEntityPage>(Utf8JsonWriter writer, TEntityPage value, JsonSerializerOptions options)
        where TEntityPage : EntityPage
    {
        var entityPageData = new EntityPageData { PageNumber = value.PageNumberValue, PageSize = value.PageSizeValue };
        JsonSerializer.Serialize(writer, entityPageData, options);
    }

    private class EntityPageData
    {
        public string? PageNumber { get; set; } = string.Empty;
        public string? PageSize { get; set; } = string.Empty;
    }
}
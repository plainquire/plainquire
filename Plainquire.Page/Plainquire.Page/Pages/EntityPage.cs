﻿using Plainquire.Page.Abstractions;
using Plainquire.Page.JsonConverters;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plainquire.Page;

/// <summary>
/// Hub to create pagination for <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type to be paged.</typeparam>
[JsonConverter(typeof(EntityPageConverter.Factory))]
public class EntityPage<TEntity> : EntityPage
{
    /// <inheritdoc />
    public EntityPage() { }

    /// <inheritdoc />
    public EntityPage(PageConfiguration configuration)
        : base(configuration) { }

    /// <inheritdoc />
    public EntityPage(int? pageNumber, int? pageSize)
        : base(pageNumber, pageSize) { }

    /// <inheritdoc />
    public EntityPage(int? pageNumber, int? pageSize, PageConfiguration configuration)
        : base(pageNumber, pageSize, configuration) { }

    /// <summary>
    /// Creates a deep clone of this page.
    /// </summary>
    public new EntityPage<TEntity> Clone()
        => JsonSerializer.Deserialize<EntityPage<TEntity>>(JsonSerializer.Serialize(this))!;

    /// <summary>
    /// Casts this page to a different entity type (by creating a deep clone).
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination entity to page.</typeparam>
    public EntityPage<TDestination> Cast<TDestination>()
        => JsonSerializer.Deserialize<EntityPage<TDestination>>(JsonSerializer.Serialize(this))!;
}

/// <inheritdoc cref="EntityPage{TEntity}" />
[JsonConverter(typeof(EntityPageConverter))]
public class EntityPage : ICloneable
{
    internal string PageNumberValue = string.Empty;
    internal string PageSizeValue = string.Empty;

    /// <summary>
    /// Gets or sets the default configuration. Can be used to set a system-wide configuration.
    /// </summary>
    public PageConfiguration? Configuration { get; internal set; }

    /// <summary>
    /// The page number to get.
    /// </summary>
    public int? PageNumber
    {
        get => int.TryParse(PageNumberValue, NumberStyles.None, CultureInfo.InvariantCulture, out var page) ? page : null;
        set => PageNumberValue = value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    /// <summary>
    /// The page size to use.
    /// </summary>
    public int? PageSize
    {
        get => int.TryParse(PageSizeValue, NumberStyles.None, CultureInfo.InvariantCulture, out var pageSize) ? pageSize : null;
        set => PageSizeValue = value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    /// <summary>
    /// Creates a new instance of <see cref="EntityPage"/>.
    /// </summary>
    public EntityPage() { }

    /// <summary>
    /// Creates a new instance of <see cref="EntityPage"/>.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    public EntityPage(PageConfiguration configuration)
        : this()
        => Configuration = configuration;

    /// <summary>
    /// Creates a new instance of <see cref="EntityPage"/>.
    /// </summary>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    public EntityPage(int? pageNumber, int? pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Creates a new instance of <see cref="EntityPage"/>.
    /// </summary>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="configuration">The configuration to use.</param>
    public EntityPage(int? pageNumber, int? pageSize, PageConfiguration configuration)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Configuration = configuration;
    }

    /// <inheritdoc />
    public object Clone()
        => JsonSerializer.Deserialize<EntityPage>(JsonSerializer.Serialize(this))!;
}
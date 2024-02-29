using System;

namespace Plainquire.Filter.Abstractions;

/// <summary>
/// Filter expression related settings.
/// Implements <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property)]
public class FilterAttribute : Attribute
{
    /// <summary>
    /// The name this property is bind to when used via <see><cref>EntityFilter{TEntity}</cref></see>
    /// from MVC controllers.
    /// Default is the name of the property.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Set to <c>false</c> to disable filtering for this property.
    /// </summary>
    public bool Filterable { get; set; } = true;

    /// <summary>
    /// Set to <c>false</c> to disable sorting for this property.
    /// </summary>
    public bool Sortable { get; set; } = true;
}
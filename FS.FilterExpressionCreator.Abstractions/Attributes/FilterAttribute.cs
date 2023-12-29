using System;

namespace FS.FilterExpressionCreator.Abstractions.Attributes;

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
    /// Set to <c>false</c> to hide this property from binding done via <see><cref>EntityFilter{TEntity}</cref></see> from MVC controllers
    /// </summary>
    public bool Visible { get; set; } = true;
}
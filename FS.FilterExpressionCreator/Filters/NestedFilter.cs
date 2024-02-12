using System;

namespace FS.FilterExpressionCreator.Filters;

internal class NestedFilter
{
    public string PropertyName { get; }

    [Obsolete("Use 'Plainquire.Filter.NestedFilter.EntityFilter' instead.")]
    public EntityFilter EntityFilter { get; }

    [Obsolete("Use 'Plainquire.Filter.NestedFilter' instead.")]
    public NestedFilter(string propertyName, EntityFilter? entityFilter)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        EntityFilter = entityFilter ?? new EntityFilter();
    }
}
using System;

namespace Plainquire.Filter.Filters;

internal class NestedFilter
{
    public string PropertyName { get; }

    public EntityFilter EntityFilter { get; }

    public NestedFilter(string propertyName, EntityFilter? entityFilter)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        EntityFilter = entityFilter ?? new EntityFilter();
    }
}
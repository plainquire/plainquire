using Plainquire.Filter.Abstractions;
using System.Collections.Generic;

namespace Plainquire.Filter.JsonConverters;

internal class EntityFilterConverterData
{
    public List<PropertyFilterConverterData>? PropertyFilters { get; set; } = [];

    public List<NestedFilter>? NestedFilters { get; set; } = [];

    public FilterConfiguration? Configuration { get; set; }
}
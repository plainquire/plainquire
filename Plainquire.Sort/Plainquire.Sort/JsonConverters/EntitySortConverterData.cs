using Plainquire.Sort.Abstractions;
using System.Collections.Generic;

namespace Plainquire.Sort.JsonConverters;

internal class EntitySortConverterData
{
    public List<PropertySortConverterData>? PropertySorts { get; set; } = [];
    public SortConfiguration? Configuration { get; set; }
}

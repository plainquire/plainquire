using System.Collections.Generic;

namespace Plainquire.Filter.JsonConverters;

internal class PropertyFilterConverterData
{
    public PropertyFilterConverterData(string propertyName, List<ValueFilterConverterData> valueFilters)
    {
        PropertyName = propertyName;
        ValueFilters = valueFilters;
    }

    public string PropertyName { get; }

    public List<ValueFilterConverterData> ValueFilters { get; }
}
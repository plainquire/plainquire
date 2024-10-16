using Plainquire.Filter.Abstractions;

namespace Plainquire.Filter.JsonConverters;

internal class ValueFilterConverterData
{
    public FilterOperator Operator { get; set; }

    public string? Value { get; set; }
}
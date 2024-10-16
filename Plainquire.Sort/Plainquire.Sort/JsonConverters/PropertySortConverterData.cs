using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.JsonConverters;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Required by serializers")]
internal class PropertySortConverterData
{
    public string PropertyPath { get; set; }
    public SortDirection Direction { get; set; }
    public int Position { get; set; }

    public PropertySortConverterData(string propertyPath, SortDirection direction, int position)
    {
        PropertyPath = propertyPath;
        Direction = direction;
        Position = position;
    }
}
using System;

namespace FS.FilterExpressionCreator.Filters
{
    internal class PropertyFilter
    {
        public string PropertyName { get; }

        public ValueFilter[] ValueFilters { get; }

        public PropertyFilter(string propertyName, ValueFilter[]? valueFilters)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            ValueFilters = valueFilters ?? Array.Empty<ValueFilter>();
        }
    }
}

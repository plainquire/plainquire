using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.SortQueryableCreator.Tests.Models;

[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TestModel<TValue> : IComparable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public TValue? Value { get; set; }

    public TValue? Value2 { get; set; }

    public TestModelNested<TValue>? NestedObject { get; set; }

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            var values = new List<string>();
            if (Value != null && !Value.Equals(default(TValue)))
                values.Add($"ValueA: {Value}");
            if (Value2 != null && !Value2.Equals(default(TValue)))
                values.Add($"ValueB: {Value2}");
            if (NestedObject != null && NestedObject.Value != null && !NestedObject.Value.Equals(default(TValue)))
                values.Add($"Nested: {NestedObject.Value}");
            return string.Join(", ", values);
        }
    }

    public int CompareTo(object? obj)
    {
        if (Value is IComparable comparable && obj is TestModel<TValue> other)
            return comparable.CompareTo(other.Value);

        return 0;
    }
}
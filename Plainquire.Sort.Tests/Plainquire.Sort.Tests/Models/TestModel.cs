using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TestModel<TValue> : IComparable
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public TValue? Value { get; init; }

    public TValue? Value2 { get; init; }

    public TestModelNested<TValue>? NestedObject { get; init; }

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
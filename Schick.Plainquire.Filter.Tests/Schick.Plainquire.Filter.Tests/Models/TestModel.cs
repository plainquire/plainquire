using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Models;

[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TestModel<TValue>
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public TValue? ValueA { get; init; }

    public TValue? ValueB { get; init; }

    public TestModelNested? NestedObject { get; init; }

    public List<TestModelNested>? NestedList { get; init; }

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            var values = new List<string>();
            if (ValueA != null && !ValueA.Equals(default(TValue)))
                values.Add($"ValueA: {ValueA}");
            if (ValueB != null && !ValueB.Equals(default(TValue)))
                values.Add($"ValueB: {ValueB}");
            return string.Join(", ", values);
        }
    }
}
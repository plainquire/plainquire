using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Models;

[ExcludeFromCodeCoverage]
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public class TestModel<TValue>
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public TValue? ValueA { get; set; }

    public TValue? ValueB { get; set; }

    public TestModelNested? NestedObject { get; set; }

    public List<TestModelNested>? NestedList { get; set; }

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
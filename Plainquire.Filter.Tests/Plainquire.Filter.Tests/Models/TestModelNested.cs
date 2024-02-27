using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by EF")]
public class TestModelNested
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ParentId { get; init; }

    public required string Value { get; init; }
}
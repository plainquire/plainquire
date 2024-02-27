using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Required by EF")]
public class TestModelNested<TValue>
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required TValue? Value { get; init; }
}
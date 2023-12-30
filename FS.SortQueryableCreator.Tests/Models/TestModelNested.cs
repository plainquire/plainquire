using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.SortQueryableCreator.Tests.Models;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
[ExcludeFromCodeCoverage]
public class TestModelNested<TValue>
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ParentId { get; set; }

    public required TValue? Value { get; set; }
}
using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Required by model binders")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by model binders")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by EF")]
[EntityFilter(Prefix = "")]
public class TestPerson
{
    [Filter(Name = "FullName")]
    public string? Name { get; set; }

    public DateTime? Birthday { get; set; }

    public TestAddress? Address { get; set; }
}
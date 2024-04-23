using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[EntityFilter(Prefix = "")]
public class TestPerson
{
    [Filter(Name = "FullName")]
    public string? Name { get; set; }

    public DateTime? Birthday { get; set; }

    public TestAddress? Address { get; set; }
}
using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[EntityFilter(Prefix = "Address")]
public class TestAddress
{
    public string? Street { get; set; }

    public string? Country { get; set; }

    [Filter(Sortable = false)]
    public CultureInfo? Culture { get; set; }
}
using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[EntitySortSet]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by model binders")]
public class TestEntitySortSet
{
    public EntitySort<TestPerson> Person { get; set; } = new();

    public EntitySort<TestAddress> Address { get; set; } = new();
}
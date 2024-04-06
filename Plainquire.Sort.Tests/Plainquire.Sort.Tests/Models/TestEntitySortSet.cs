using Plainquire.Filter.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
[EntitySortSet]
public class TestEntitySortSet
{
    public EntitySort<TestPerson> Person { get; set; } = new();

    public EntitySort<TestAddress> Address { get; set; } = new();
}
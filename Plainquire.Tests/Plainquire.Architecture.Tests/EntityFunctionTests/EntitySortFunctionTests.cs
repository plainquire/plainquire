using FluentAssertions;
using NUnit.Framework;
using Plainquire.Sort.Tests.Services;
using Plainquire.TestSupport.Services;

namespace Plainquire.Architecture.Tests.EntityFunctionTests;

[TestFixture]
public class EntitySortFunctionTests : TestContainer
{
    [Test]
    public void AllEntitySortFunctions_ShouldBeActive()
    {
        var filterFUnctions = EntitySortFunctions.GetEntitySortFunctions<object>();
        filterFUnctions.Should().HaveCount(6);
    }
}
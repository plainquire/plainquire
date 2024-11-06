using FluentAssertions;
using NUnit.Framework;
using Plainquire.Filter.Tests.Services;
using Plainquire.TestSupport.Services;

namespace Plainquire.Architecture.Tests.EntityFunctionTests;

[TestFixture]
public class EntityFilterFunctionTests : TestContainer
{
    [Test]
    public void AllEntityFilterFunctions_ShouldBeActive()
    {
        var filterFUnctions = EntityFilterFunctions.GetEntityFilterFunctions<object>();
        filterFUnctions.Should().HaveCount(6);
    }
}
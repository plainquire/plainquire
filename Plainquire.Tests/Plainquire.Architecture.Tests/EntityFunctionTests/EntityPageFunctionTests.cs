using FluentAssertions;
using NUnit.Framework;
using Plainquire.Page.Tests.Services;
using Plainquire.TestSupport.Services;

namespace Plainquire.Architecture.Tests.EntityFunctionTests;

[TestFixture]
public class EntityPageFunctionTests : TestContainer
{
    [Test]
    public void AllEntityPageFunctions_ShouldBeActive()
    {
        var filterFUnctions = EntityPageFunctions.GetEntityPageFunctions<object>();
        filterFUnctions.Should().HaveCount(6);
    }
}
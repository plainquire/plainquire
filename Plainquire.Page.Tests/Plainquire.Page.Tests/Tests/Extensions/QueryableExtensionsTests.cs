using FluentAssertions;
using NUnit.Framework;
using Plainquire.Page.Tests.Models;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.Extensions;

[TestFixture]
public class QueryableExtensionsTests
{
    [Test]
    public void WhenEnumerableIsPagedDirectly_ExpectedPageIsReturned()
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var pagedItems = testItems.Page(2, 2);
        pagedItems.Should().Equal(testItems[2], testItems[3]);
    }

    [Test]
    public void WhenQueryableIsPagedDirectly_ExpectedPageIsReturned()
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var pagedItems = testItems.AsQueryable().Page(2, 2);
        pagedItems.Should().Equal(testItems[2], testItems[3]);
    }
}
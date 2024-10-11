using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Tests.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.Extensions;

[TestClass]
public class QueryableExtensionsTests
{
    [TestMethod]
    public void WhenEnumerableIsPagedDirectly_ExpectedPageIsReturned()
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var pagedItems = testItems.Page(2, 2);
        pagedItems.Should().Equal(testItems[2], testItems[3]);
    }

    [TestMethod]
    public void WhenQueryableIsPagedDirectly_ExpectedPageIsReturned()
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var pagedItems = testItems.AsQueryable().Page(2, 2);
        pagedItems.Should().Equal(testItems[2], testItems[3]);
    }
}
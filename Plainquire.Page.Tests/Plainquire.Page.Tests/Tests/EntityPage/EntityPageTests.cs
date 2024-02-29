using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Tests.Models;
using Plainquire.Page.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.EntityPage;

[TestClass, ExcludeFromCodeCoverage]
public class EntityPageTests
{
    private static readonly PageTestcase<TestModel<string>>[] _validPages =
    [
        PageTestcase<TestModel<string>>.Create(null!, "1"),
        PageTestcase<TestModel<string>>.Create("", "1"),
        PageTestcase<TestModel<string>>.Create("1", "1"),
        PageTestcase<TestModel<string>>.Create("2", "1"),
        PageTestcase<TestModel<string>>.Create("3", "1"),
        PageTestcase<TestModel<string>>.Create("4", "1"),
        PageTestcase<TestModel<string>>.Create("5", "1"),

        PageTestcase<TestModel<string>>.Create(null!, "2"),
        PageTestcase<TestModel<string>>.Create("", "2"),
        PageTestcase<TestModel<string>>.Create("1", "2"),
        PageTestcase<TestModel<string>>.Create("2", "2"),
        PageTestcase<TestModel<string>>.Create("3", "2"),

        PageTestcase<TestModel<string>>.Create(null!, "3"),
        PageTestcase<TestModel<string>>.Create("", "3"),
        PageTestcase<TestModel<string>>.Create("1", "3"),
        PageTestcase<TestModel<string>>.Create("2", "3"),
        PageTestcase<TestModel<string>>.Create("3", "3"),

        PageTestcase<TestModel<string>>.Create(null!, "4"),
        PageTestcase<TestModel<string>>.Create("", "4"),
        PageTestcase<TestModel<string>>.Create("1", "4"),
        PageTestcase<TestModel<string>>.Create("2", "4"),
    ];

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_validPages))]
    public void WhenValidPageIsGiven_ItemsMatchingRequestedPageAreReturned(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        if (testCase.Page.PageSize == null)
            throw new InvalidOperationException("PageSize must be set");

        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var pagedItems = pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        var expectedItems = testItems
            .Skip(((testCase.Page.PageNumber ?? 1) - 1) * testCase.Page.PageSize.Value)
            .Take(testCase.Page.PageSize.Value);

        pagedItems.Should().Equal(expectedItems);
    }

    private static readonly PageTestcase<TestModel<string>>[] _outOfRangePages =
    [
        PageTestcase<TestModel<string>>.Create("-1", "1"),
        PageTestcase<TestModel<string>>.Create("0", "1"),
        PageTestcase<TestModel<string>>.Create("1", "-1"),
        PageTestcase<TestModel<string>>.Create("1", "0"),
    ];

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_outOfRangePages))]
    public void WhenOutOfRangePageParameterIsGiven_ArgumentOutOfRangeExceptionIsThrown(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var page = () => pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        page.Should().Throw<ArgumentOutOfRangeException>();
    }

    private static readonly PageTestcase<TestModel<string>>[] _notParseablePages =
    [
        PageTestcase<TestModel<string>>.Create("a", "1"),
        PageTestcase<TestModel<string>>.Create("1", "b"),
        PageTestcase<TestModel<string>>.Create("a", "b"),
    ];

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_notParseablePages))]
    public void WhenNotParseablePageParameterIsGiven_FormatExceptionIsThrown(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var page = () => pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        page.Should().Throw<FormatException>();
    }

    private static readonly PageTestcase<TestModel<string>>[] _noPageSizePages =
    [
        PageTestcase<TestModel<string>>.Create("1", null!),
        PageTestcase<TestModel<string>>.Create("1", ""),
    ];

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_noPageSizePages))]
    public void WhenPageWithoutPageSizeIsGiven_ArgumentExceptionIsThrown(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };
        var page = () => pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        page.Should().Throw<ArgumentException>();
    }
}
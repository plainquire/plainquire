using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Abstractions;
using Plainquire.Page.Tests.Models;
using Plainquire.Page.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.EntityPage;

[TestClass, ExcludeFromCodeCoverage]
public class PageConfigurationTests
{
    private static readonly PageTestcase<TestModel<string>>[] _invalidPageNumber =
    [
        PageTestcase<TestModel<string>>.Create(null!, "1"),
        PageTestcase<TestModel<string>>.Create("", "1"),
        PageTestcase<TestModel<string>>.Create("-1", "1"),
        PageTestcase<TestModel<string>>.Create("0", "1"),
        PageTestcase<TestModel<string>>.Create("a", "1"),
    ];

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_invalidPageNumber))]
    public void WhenInvalidPageNumberIsGivenAndParseExceptionsAreIgnored_FirstPageIsReturned(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        if (testCase.Page.PageSize == null)
            throw new InvalidOperationException("PageSize must be set");

        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var configuration = new PageConfiguration { IgnoreParseExceptions = true };
        var pagedItems = pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page, configuration);

        var expectedItems = testItems
            .Take(testCase.Page.PageSize.Value);

        pagedItems.Should().Equal(expectedItems);
    }

    private static readonly PageTestcase<TestModel<string>>[] _invalidPageSize =
    [
        PageTestcase<TestModel<string>>.Create("1", null!),
        PageTestcase<TestModel<string>>.Create("1", ""),
        PageTestcase<TestModel<string>>.Create("1", "-1"),
        PageTestcase<TestModel<string>>.Create("1", "0"),
        PageTestcase<TestModel<string>>.Create("1", "a"),
    ];

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_invalidPageSize))]
    public void WhenInvalidPageSizeIsGivenAndParseExceptionsAreIgnored_AllItemsReturned(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var configuration = new PageConfiguration { IgnoreParseExceptions = true };
        var pagedItems = pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page, configuration);

        var expectedItems = testItems;

        pagedItems.Should().Equal(expectedItems);
    }

    [DataTestMethod]
    [PageTestCaseDataSource(nameof(_invalidPageSize))]
    public void WhenParseExceptionsAreIgnoredByDefaultConfiguration_AllItemsReturned(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        Page.EntityPage.DefaultConfiguration = new PageConfiguration { IgnoreParseExceptions = true };
        var pagedItems = pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        var expectedItems = testItems;

        pagedItems.Should().Equal(expectedItems);

        // Cleanup
        Page.EntityPage.DefaultConfiguration = new PageConfiguration();
    }
}
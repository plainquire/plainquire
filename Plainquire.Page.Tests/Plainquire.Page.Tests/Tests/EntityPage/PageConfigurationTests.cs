﻿using FluentAssertions;
using NUnit.Framework;
using Plainquire.Page.Abstractions;
using Plainquire.Page.Tests.Models;
using Plainquire.Page.Tests.Services;
using Plainquire.TestSupport.Services;
using System;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.EntityPage;

[TestFixture]
public class PageConfigurationTests : TestContainer
{
    private static readonly PageConfiguration _ignoreParseExceptionConfiguration = new() { IgnoreParseExceptions = true };

    private static readonly PageTestcase<TestModel<string>>[] _invalidPageNumber =
    [
        PageTestcase<TestModel<string>>.Create(null!, "1", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("", "1", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("-1", "1", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("0", "1", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("a", "1", _ignoreParseExceptionConfiguration),
    ];

    [PageTestDataSource(nameof(_invalidPageNumber))]
    public void WhenInvalidPageNumberIsGivenAndParseExceptionsAreIgnored_FirstPageIsReturned(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        if (testCase.Page.PageSize == null)
            throw new InvalidOperationException("PageSize must be set");

        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var pagedItems = pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        var expectedItems = testItems
            .Take(testCase.Page.PageSize.Value);

        pagedItems.Should().Equal(expectedItems);
    }

    private static readonly PageTestcase<TestModel<string>>[] _invalidPageSize =
    [
        PageTestcase<TestModel<string>>.Create("1", null!, _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("1", "", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("1", "-1", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("1", "0", _ignoreParseExceptionConfiguration),
        PageTestcase<TestModel<string>>.Create("1", "a", _ignoreParseExceptionConfiguration),
    ];

    [PageTestDataSource(nameof(_invalidPageSize))]
    public void WhenInvalidPageSizeIsGivenAndParseExceptionsAreIgnored_AllItemsReturned(PageTestcase<TestModel<string>> testCase, EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var pagedItems = pageFunc(testItems, (EntityPage<TestModel<string>>)testCase.Page);

        var expectedItems = testItems;

        pagedItems.Should().Equal(expectedItems);
    }
}
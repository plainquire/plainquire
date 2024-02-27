using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Enums;
using Plainquire.Filter.Exceptions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForGuidByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForGuidByValue_WorksAsExpected(FilterTestCase<Guid, Guid> testCase, EntityFilterFunc<TestModel<Guid>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<Guid>[] _testItems =
    [
        new() { ValueA = Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab") },
        new() { ValueA = Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2") },
        new() { ValueA = Guid.Parse("6cda682c-c0cc-4d4d-bea3-7058777e98d1") }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly FilterTestCase<Guid, Guid>[] _testCases =
    [
        FilterTestCase.Create(1100, FilterOperator.Default, [Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")], (Guid x) => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create(1200, FilterOperator.Contains, [Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab")], (Guid x) => x == Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab")),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, [Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")], (Guid x) => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, [Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")], (Guid x) => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create(1500, FilterOperator.NotEqual, [Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")], (Guid x) => x != Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create(1600, FilterOperator.LessThan, [Guid.Empty], new FilterExpressionException("Filter operator 'LessThan' not allowed for property type 'System.Guid'")),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, [Guid.Empty], new FilterExpressionException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Guid'")),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, [Guid.Empty], new FilterExpressionException("Filter operator 'GreaterThan' not allowed for property type 'System.Guid'")),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, [Guid.Empty], new FilterExpressionException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Guid'")),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new Guid[] { default }, new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Guid'")),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new Guid[] { default }, new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Guid'"))
    ];
    // ReSharper restore RedundantExplicitArrayCreation
}
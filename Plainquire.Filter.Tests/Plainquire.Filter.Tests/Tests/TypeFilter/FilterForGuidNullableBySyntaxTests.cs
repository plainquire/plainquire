using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForGuidNullableBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForGuidNullableBySyntax_WorksAsExpected(FilterTestCase<Guid?, Guid?> testCase, EntityFilterFunc<TestModel<Guid?>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<Guid?>[] _testItems =
    [
        new() { ValueA = null },
        new() { ValueA = Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab") },
        new() { ValueA = Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2") },
        new() { ValueA = Guid.Parse("6cda682c-c0cc-4d4d-bea3-7058777e98d1") }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly FilterTestCase<Guid?, Guid?>[] _testCases =
    [
        FilterTestCase.Create<Guid?>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1102, "6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
        FilterTestCase.Create<Guid?>(1103, "6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create<Guid?>(1200, "~686c", x => x == Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab")),
        FilterTestCase.Create<Guid?>(1201, "~6cda682c", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2") || x == Guid.Parse("6cda682c-c0cc-4d4d-bea3-7058777e98d1")),

        FilterTestCase.Create<Guid?>(1300, "^df72ce74", x => x == Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab")),

        FilterTestCase.Create<Guid?>(1400, "$50a213ab", x => x == Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab")),

        FilterTestCase.Create<Guid?>(1500, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1501, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1502, "=6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
        FilterTestCase.Create<Guid?>(1503, "=6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create<Guid?>(1600, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1601, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1602, "==6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
        FilterTestCase.Create<Guid?>(1603, "==6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create<Guid?>(1700, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1701, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<Guid?>(1702, "!6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x != Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
        FilterTestCase.Create<Guid?>(1703, "!6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x != Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

        FilterTestCase.Create<Guid?>(1800, "<", new FilterExpressionException("Filter operator 'LessThan' not allowed for property type 'System.Nullable`1[System.Guid]'")),

        FilterTestCase.Create<Guid?>(1900, "<=", new FilterExpressionException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Nullable`1[System.Guid]'")),

        FilterTestCase.Create<Guid?>(2000, ">", new FilterExpressionException("Filter operator 'GreaterThan' not allowed for property type 'System.Nullable`1[System.Guid]'")),

        FilterTestCase.Create<Guid?>(2100, ">=", new FilterExpressionException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Nullable`1[System.Guid]'")),

        FilterTestCase.Create<Guid?>(2200, "ISNULL", x => x == null),

        FilterTestCase.Create<Guid?>(2300, "NOTNULL", x => x != null)
    ];
    // ReSharper restore RedundantExplicitArrayCreation
}
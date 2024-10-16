using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass]
public class FilterForShortNullableByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForShortNullableByValue_WorksAsExpected(object testCase, EntityFilterFunc<TestModel<short?>> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<short?, short?> shortTestCase:
                shortTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<float?, short?> floatTestCase:
                floatTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new UnreachableException("Unsupported test case");
        }
    }

    private static readonly TestModel<short?>[] _testItems =
    [
        new() { ValueA = -19 },
        new() { ValueA = -05 },
        new() { ValueA = -00 },
        new() { ValueA = +05 },
        new() { ValueA = +19 },
        new() { ValueA = null }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    // ReSharper disable CompareOfFloatsByEqualityOperator
    private static readonly object[] _testCases =
    [
        FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, new short?[] { -9 }, (short? x) => x == -9f),

        FilterTestCase.Create(1100, FilterOperator.Default, new short?[] { -5 }, (short? x) => x == -5),
        FilterTestCase.Create(1101, FilterOperator.Default, new short?[] { -20 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1102, FilterOperator.Default, new short?[] { +5 }, (short? x) => x == +5),

        FilterTestCase.Create(1200, FilterOperator.Contains, new short?[] { +5 }, (short? x) => x is +5 or -5),
        FilterTestCase.Create(1201, FilterOperator.Contains, new short?[] { -5 }, (short? x) => x == -5),
        FilterTestCase.Create(1202, FilterOperator.Contains, new short?[] { +3 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1203, FilterOperator.Contains, new short?[] { +0 }, (short? x) => x == 0),

        FilterTestCase.Create(1300, FilterOperator.StartsWith, new short?[] { +0 }, (short? x) => x == 0),
        FilterTestCase.Create(1301, FilterOperator.StartsWith, new short?[] { -0 }, (short? x) => x == 0),
        FilterTestCase.Create(1302, FilterOperator.StartsWith, new short?[] { +1 }, (short? x) => x == 19),
        FilterTestCase.Create(1303, FilterOperator.StartsWith, new short?[] { -1 }, (short? x) => x == -19),
        FilterTestCase.Create(1304, FilterOperator.StartsWith, new short?[] { +2 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1305, FilterOperator.StartsWith, new short?[] { -2 }, (short? _) => TestItems.NONE),

        FilterTestCase.Create(1400, FilterOperator.EndsWith, new short?[] { 0 }, (short? x) => x == 0),
        FilterTestCase.Create(1401, FilterOperator.EndsWith, new short?[] { 4 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1403, FilterOperator.EndsWith, new short?[] { 9 }, (short? x) => x is 19 or -19),

        FilterTestCase.Create(1500, FilterOperator.EqualCaseInsensitive, new short?[] { -5 }, (short? x) => x == -5),
        FilterTestCase.Create(1501, FilterOperator.EqualCaseInsensitive, new short?[] { -20 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1502, FilterOperator.EqualCaseInsensitive, new short?[] { +5 }, (short? x) => x == +5),

        FilterTestCase.Create(1600, FilterOperator.EqualCaseSensitive, new short?[] { -5 }, (short? x) => x == -5),
        FilterTestCase.Create(1601, FilterOperator.EqualCaseSensitive, new short?[] { -20 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1602, FilterOperator.EqualCaseSensitive, new short?[] { +5 }, (short? x) => x == +5),

        FilterTestCase.Create(1700, FilterOperator.NotEqual, new short?[] { -5 }, (short? x) => x != -5),
        FilterTestCase.Create(1701, FilterOperator.NotEqual, new short?[] { -20 }, (short? _) => TestItems.ALL),
        FilterTestCase.Create(1702, FilterOperator.NotEqual, new short?[] { +5 }, (short? x) => x != +5),

        FilterTestCase.Create(1800, FilterOperator.LessThan, new short?[] { -5 }, (short? x) => x < -5),
        FilterTestCase.Create(1801, FilterOperator.LessThan, new short?[] { -20 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1802, FilterOperator.LessThan, new short?[] { +5 }, (short? x) => x < +5),

        FilterTestCase.Create(1900, FilterOperator.LessThanOrEqual, new short?[] { -5 }, (short? x) => x <= -5),
        FilterTestCase.Create(1901, FilterOperator.LessThanOrEqual, new short?[] { -20 }, (short? _) => TestItems.NONE),
        FilterTestCase.Create(1902, FilterOperator.LessThanOrEqual, new short?[] { +5 }, (short? x) => x <= +5),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, new short?[] { -5 }, (short? x) => x > -5),
        FilterTestCase.Create(1801, FilterOperator.GreaterThan, new short?[] { -20 }, (short? x) => x >= -20),
        FilterTestCase.Create(1802, FilterOperator.GreaterThan, new short?[] { +5 }, (short? x) => x > +5),

        FilterTestCase.Create(2000, FilterOperator.GreaterThanOrEqual, new short?[] { -5 }, (short? x) => x >= -5),
        FilterTestCase.Create(2001, FilterOperator.GreaterThanOrEqual, new short?[] { -20 }, (short? x) => x >= -20),
        FilterTestCase.Create(2002, FilterOperator.GreaterThanOrEqual, new short?[] { +5 }, (short? x) => x >= +5),

        FilterTestCase.Create(2100, FilterOperator.IsNull, new short?[] { default }, (short? x) => x == null),

        FilterTestCase.Create(2200, FilterOperator.NotNull, new short?[] { default }, (short? x) => x != null)
    ];
    // ReSharper restore CompareOfFloatsByEqualityOperator
    // ReSharper restore RedundantExplicitArrayCreation
}
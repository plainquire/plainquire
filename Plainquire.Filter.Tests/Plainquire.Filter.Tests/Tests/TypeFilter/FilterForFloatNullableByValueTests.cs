using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass]
public class FilterForFloatNullableByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForFloatNullableByValue_WorksAsExpected(object testCase, EntityFilterFunc<TestModel<float?>> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<float?, float?> floatTestCase:
                floatTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<int?, float?> intTestCase:
                intTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new InvalidOperationException("Unsupported test case");
        }
    }

    private static readonly TestModel<float?>[] _testItems =
        new TestModel<float?>[]
        {
            new() { ValueA = -19.0f },
            new() { ValueA = -05.5f },
            new() { ValueA = -00.0f },
            new() { ValueA = +05.5f },
            new() { ValueA = +19.0f },
            new() { ValueA = null }
        };

    // ReSharper disable RedundantExplicitArrayCreation
    // ReSharper disable CompareOfFloatsByEqualityOperator
    private static readonly object[] _testCases =
        new object[]
        {
            FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, new int?[] { -9 }, (float? x) => x == -9f),

            FilterTestCase.Create(1100, FilterOperator.Default, new float?[] { -5.5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1101, FilterOperator.Default, new float?[] { -20f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1102, FilterOperator.Default, new float?[] { +5.5f }, (float? x) => x == +5.5f),

            FilterTestCase.Create(1200, FilterOperator.Contains, new float?[] { +5f }, (float? x) => x is +5.5f or -5.5f),
            FilterTestCase.Create(1201, FilterOperator.Contains, new float?[] { -5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1202, FilterOperator.Contains, new float?[] { +3f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1203, FilterOperator.Contains, new float?[] { +0f }, (float? x) => x == -0),

            FilterTestCase.Create(1300, FilterOperator.StartsWith, new float?[] { +0f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1301, FilterOperator.StartsWith, new float?[] { -0f }, (float? x) => x == -0),
            FilterTestCase.Create(1302, FilterOperator.StartsWith, new float?[] { +1f }, (float? x) => x == 19),
            FilterTestCase.Create(1303, FilterOperator.StartsWith, new float?[] { -1f }, (float? x) => x == -19),
            FilterTestCase.Create(1304, FilterOperator.StartsWith, new float?[] { +2f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1305, FilterOperator.StartsWith, new float?[] { -2f }, (float? _) => TestItems.NONE),

            FilterTestCase.Create(1400, FilterOperator.EndsWith, new float?[] { 0f }, (float? x) => x == -0),
            FilterTestCase.Create(1401, FilterOperator.EndsWith, new float?[] { 4f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1403, FilterOperator.EndsWith, new float?[] { 9f }, (float? x) => x is 19 or -19),

            FilterTestCase.Create(1500, FilterOperator.EqualCaseInsensitive, new float?[] { -5.5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1501, FilterOperator.EqualCaseInsensitive, new float?[] { -20f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1502, FilterOperator.EqualCaseInsensitive, new float?[] { +5.5f }, (float? x) => x == +5.5f),

            FilterTestCase.Create(1600, FilterOperator.EqualCaseSensitive, new float?[] { -5.5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1601, FilterOperator.EqualCaseSensitive, new float?[] { -20f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1602, FilterOperator.EqualCaseSensitive, new float?[] { +5.5f }, (float? x) => x == +5.5f),

            FilterTestCase.Create(1700, FilterOperator.NotEqual, new float?[] { -5.5f }, (float? x) => x != -5.5f),
            FilterTestCase.Create(1701, FilterOperator.NotEqual, new float?[] { -20f }, (float? _) => TestItems.ALL),
            FilterTestCase.Create(1702, FilterOperator.NotEqual, new float?[] { +5.5f }, (float? x) => x != +5.5f),

            FilterTestCase.Create(1800, FilterOperator.LessThan, new float?[] { -5.5f }, (float? x) => x < -5.5f),
            FilterTestCase.Create(1801, FilterOperator.LessThan, new float?[] { -20f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1802, FilterOperator.LessThan, new float?[] { +5.5f }, (float? x) => x < +5.5f),

            FilterTestCase.Create(1900, FilterOperator.LessThanOrEqual, new float?[] { -5.5f }, (float? x) => x <= -5.5f),
            FilterTestCase.Create(1901, FilterOperator.LessThanOrEqual, new float?[] { -20f }, (float? _) => TestItems.NONE),
            FilterTestCase.Create(1902, FilterOperator.LessThanOrEqual, new float?[] { +5.5f }, (float? x) => x <= +5.5f),

            FilterTestCase.Create(2000, FilterOperator.GreaterThan, new float?[] { -5.5f }, (float? x) => x > -5.5f),
            FilterTestCase.Create(2001, FilterOperator.GreaterThan, new float?[] { -20f }, (float? x) => x >= -20f),
            FilterTestCase.Create(2002, FilterOperator.GreaterThan, new float?[] { +5.5f }, (float? x) => x > +5.5f),

            FilterTestCase.Create(2100, FilterOperator.GreaterThanOrEqual, new float?[] { -5.5f }, (float? x) => x >= -5.5f),
            FilterTestCase.Create(2101, FilterOperator.GreaterThanOrEqual, new float?[] { -20f }, (float? x) => x >= -20f),
            FilterTestCase.Create(2102, FilterOperator.GreaterThanOrEqual, new float?[] { +5.5f }, (float? x) => x >= +5.5f),

            FilterTestCase.Create(2200, FilterOperator.IsNull, new float?[] { default }, (float? x) => x == null),

            FilterTestCase.Create(2300, FilterOperator.NotNull, new float?[] { default }, (float? x) => x != null)
        };
    // ReSharper restore CompareOfFloatsByEqualityOperator
    // ReSharper restore RedundantExplicitArrayCreation
}
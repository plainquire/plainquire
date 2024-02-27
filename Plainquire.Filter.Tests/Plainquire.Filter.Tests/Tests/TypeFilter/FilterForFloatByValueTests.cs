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
public class FilterForFloatByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForFloatByValue_WorksAsExpected(object testCase, EntityFilterFunc<TestModel<float>> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<float, float> floatTestCase:
                floatTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<int, float> intTestCase:
                intTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new InvalidOperationException("Unsupported test case");
        }
    }

    private static readonly TestModel<float>[] _testItems =
    [
        new() { ValueA = -9f },
        new() { ValueA = -5.5f },
        new() { ValueA = -0f },
        new() { ValueA = +5.5f },
        new() { ValueA = +9f }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    // ReSharper disable CompareOfFloatsByEqualityOperator
    private static readonly object[] _testCases =
    [
        FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, [-9], (float x) => x == -9f),

        FilterTestCase.Create(1102, FilterOperator.Default, [-5.5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1103, FilterOperator.Default, new float[] { -10 }, (float _) => TestItems.NONE),
        FilterTestCase.Create(1104, FilterOperator.Default, [+5.5f], (float x) => x == +5.5f),

        FilterTestCase.Create(1200, FilterOperator.Contains, [+5f], (float x) => x is +5.5f or -5.5f),
        FilterTestCase.Create(1201, FilterOperator.Contains, [-5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1202, FilterOperator.Contains, [+3f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1203, FilterOperator.Contains, [+0f], (float x) => x == 0),

        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, [-5.5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, new float[] { -10 }, (float _) => TestItems.NONE),
        FilterTestCase.Create(1304, FilterOperator.EqualCaseInsensitive, [+5.5f], (float x) => x == +5.5f),

        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, [-5.5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, new float[] { -10 }, (float _) => TestItems.NONE),
        FilterTestCase.Create(1404, FilterOperator.EqualCaseSensitive, [+5.5f], (float x) => x == +5.5f),

        FilterTestCase.Create(1502, FilterOperator.NotEqual, [-5.5f], (float x) => x != -5.5f),
        FilterTestCase.Create(1503, FilterOperator.NotEqual, new float[] { -10 }, (float _) => TestItems.ALL),
        FilterTestCase.Create(1504, FilterOperator.NotEqual, [+5.5f], (float x) => x != +5.5f),

        FilterTestCase.Create(1602, FilterOperator.LessThan, [-5.5f], (float x) => x < -5.5f),
        FilterTestCase.Create(1603, FilterOperator.LessThan, new float[] { -10 }, (float _) => TestItems.NONE),
        FilterTestCase.Create(1604, FilterOperator.LessThan, [+5.5f], (float x) => x < +5.5f),

        FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, [-5.5f], (float x) => x <= -5.5f),
        FilterTestCase.Create(1703, FilterOperator.LessThanOrEqual, new float[] { -10 }, (float _) => TestItems.NONE),
        FilterTestCase.Create(1704, FilterOperator.LessThanOrEqual, [+5.5f], (float x) => x <= +5.5f),

        FilterTestCase.Create(1802, FilterOperator.GreaterThan, [-5.5f], (float x) => x > -5.5f),
        FilterTestCase.Create(1803, FilterOperator.GreaterThan, new float[] { -10 }, (float _) => TestItems.ALL),
        FilterTestCase.Create(1804, FilterOperator.GreaterThan, [+5.5f], (float x) => x > +5.5f),

        FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, [-5.5f], (float x) => x >= -5.5f),
        FilterTestCase.Create(1903, FilterOperator.GreaterThanOrEqual, new float[] { -10 }, (float _) => TestItems.ALL),
        FilterTestCase.Create(1904, FilterOperator.GreaterThanOrEqual, [+5.5f], (float x) => x >= +5.5f),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new float[] { default }, new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Single'")),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new float[] { default }, new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Single'"))
    ];
    // ReSharper restore CompareOfFloatsByEqualityOperator
    // ReSharper restore RedundantExplicitArrayCreation
}
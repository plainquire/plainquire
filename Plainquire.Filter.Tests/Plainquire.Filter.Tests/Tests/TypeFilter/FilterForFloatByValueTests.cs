using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using Plainquire.TestSupport.Services;
using System.Diagnostics;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestFixture]
public class FilterForFloatByValueTests : TestContainer
{
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
                throw new UnreachableException("Unsupported test case");
        }
    }

    private static readonly TestModel<float>[] _testItems =
    [
        new() { ValueA = -19.0f },
        new() { ValueA = -05.5f },
        new() { ValueA = -00.0f },
        new() { ValueA = +05.5f },
        new() { ValueA = +19.0f }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    // ReSharper disable CompareOfFloatsByEqualityOperator
    private static readonly object[] _testCases =
    [
        FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, [-9], (float x) => x == -9f),

        FilterTestCase.Create(1102, FilterOperator.Default, [-5.5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1103, FilterOperator.Default, [-20f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1104, FilterOperator.Default, [+5.5f], (float x) => x == +5.5f),

        FilterTestCase.Create(1200, FilterOperator.Contains, [+5f], (float x) => x is +5.5f or -5.5f),
        FilterTestCase.Create(1201, FilterOperator.Contains, [-5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1202, FilterOperator.Contains, [+3f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1203, FilterOperator.Contains, [+0f], (float x) => x == -0),

        FilterTestCase.Create(1300, FilterOperator.StartsWith, [+0f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1301, FilterOperator.StartsWith, [-0f], (float x) => x == -0),
        FilterTestCase.Create(1302, FilterOperator.StartsWith, [+1f], (float x) => x == 19),
        FilterTestCase.Create(1303, FilterOperator.StartsWith, [-1f], (float x) => x == -19),
        FilterTestCase.Create(1304, FilterOperator.StartsWith, [+2f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1305, FilterOperator.StartsWith, [-2f], (float _) => TestItems.NONE),

        FilterTestCase.Create(1400, FilterOperator.EndsWith, [0f], (float x) => x == -0),
        FilterTestCase.Create(1401, FilterOperator.EndsWith, [4f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1403, FilterOperator.EndsWith, [9f], (float x) => x is 19 or -19),

        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, [-5.5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, [-20f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1304, FilterOperator.EqualCaseInsensitive, [+5.5f], (float x) => x == +5.5f),

        FilterTestCase.Create(1602, FilterOperator.EqualCaseSensitive, [-5.5f], (float x) => x == -5.5f),
        FilterTestCase.Create(1603, FilterOperator.EqualCaseSensitive, [-20f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1604, FilterOperator.EqualCaseSensitive, [+5.5f], (float x) => x == +5.5f),

        FilterTestCase.Create(1702, FilterOperator.NotEqual, [-5.5f], (float x) => x != -5.5f),
        FilterTestCase.Create(1703, FilterOperator.NotEqual, [-20f], (float _) => TestItems.ALL),
        FilterTestCase.Create(1704, FilterOperator.NotEqual, [+5.5f], (float x) => x != +5.5f),

        FilterTestCase.Create(1802, FilterOperator.LessThan, [-5.5f], (float x) => x < -5.5f),
        FilterTestCase.Create(1803, FilterOperator.LessThan, [-20f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1804, FilterOperator.LessThan, [+5.5f], (float x) => x < +5.5f),

        FilterTestCase.Create(1902, FilterOperator.LessThanOrEqual, [-5.5f], (float x) => x <= -5.5f),
        FilterTestCase.Create(1903, FilterOperator.LessThanOrEqual, [-20f], (float _) => TestItems.NONE),
        FilterTestCase.Create(1904, FilterOperator.LessThanOrEqual, [+5.5f], (float x) => x <= +5.5f),

        FilterTestCase.Create(2002, FilterOperator.GreaterThan, [-5.5f], (float x) => x > -5.5f),
        FilterTestCase.Create(2003, FilterOperator.GreaterThan, [-20f], (float _) => TestItems.ALL),
        FilterTestCase.Create(2004, FilterOperator.GreaterThan, [+5.5f], (float x) => x > +5.5f),

        FilterTestCase.Create(2102, FilterOperator.GreaterThanOrEqual, [-5.5f], (float x) => x >= -5.5f),
        FilterTestCase.Create(2103, FilterOperator.GreaterThanOrEqual, [-20f], (float _) => TestItems.ALL),
        FilterTestCase.Create(2104, FilterOperator.GreaterThanOrEqual, [+5.5f], (float x) => x >= +5.5f),

        FilterTestCase.Create(2200, FilterOperator.IsNull, new float[] { default }, new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Single'")),

        FilterTestCase.Create(2300, FilterOperator.NotNull, new float[] { default }, new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Single'"))
    ];
    // ReSharper restore CompareOfFloatsByEqualityOperator
    // ReSharper restore RedundantExplicitArrayCreation
}
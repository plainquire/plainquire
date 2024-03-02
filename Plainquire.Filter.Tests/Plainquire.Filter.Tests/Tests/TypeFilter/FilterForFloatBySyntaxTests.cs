using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForFloatBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForFloatBySyntax_WorksAsExpected(FilterTestCase<float, float> testCase, EntityFilterFunc<TestModel<float>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<float>[] _testItems =
    [
        new() { ValueA = -9f },
        new() { ValueA = -5.5f },
        new() { ValueA = -0f },
        new() { ValueA = +5.5f },
        new() { ValueA = +9f }
    ];

    // ReSharper disable CompareOfFloatsByEqualityOperator
    private static readonly FilterTestCase<float, float>[] _testCases =
    [
        FilterTestCase.Create<float>(1000, "-5\\,5", x => x == -5.5f, TestConfig.FilterCultureDeDe),
        FilterTestCase.Create<float>(1001, "-5\\,5,+5\\,5", x => x is -5.5f or 5.5f, TestConfig.FilterCultureDeDe),
        FilterTestCase.Create<float>(1002, "-5,5,+5,5", _ => TestItems.NONE, TestConfig.FilterCultureDeDe),

        FilterTestCase.Create<float>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1102, "-5.5", x => x == -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1103, "-10", _ => TestItems.NONE),
        FilterTestCase.Create<float>(1104, "+5.5", x => x == +5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1105, "-5.5,+5.5", x => x is -5.5f or +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1200, "~5", x => x is +5.5f or -5.5f),
        FilterTestCase.Create<float>(1201, "~-5", x => x == -5.5f),
        FilterTestCase.Create<float>(1202, "~3", _ => TestItems.NONE),
        FilterTestCase.Create<float>(1203, "~0", x => x == 0),

        FilterTestCase.Create<float>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1302, "=-5.5", x => x == -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1303, "=-10", _ => TestItems.NONE),
        FilterTestCase.Create<float>(1304, "=+5.5", x => x == +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1402, "==-5.5", x => x == -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1403, "==-10", _ => TestItems.NONE),
        FilterTestCase.Create<float>(1404, "==+5.5", x => x == +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1502, "!-5.5", x => x != -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1503, "!-10", _ => TestItems.ALL),
        FilterTestCase.Create<float>(1504, "!+5.5", x => x != +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1600, "<null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1601, "<", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1602, "<-5.5", x => x < -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1603, "<-10", _ => TestItems.NONE),
        FilterTestCase.Create<float>(1604, "<+5.5", x => x < +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1700, "<=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1701, "<=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1702, "<=-5.5", x => x <= -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1703, "<=-10", _ => TestItems.NONE),
        FilterTestCase.Create<float>(1704, "<=+5.5", x => x <= +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1800, ">null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1801, ">", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1802, ">-5.5", x => x > -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1803, ">-10", _ => TestItems.ALL),
        FilterTestCase.Create<float>(1804, ">+5.5", x => x > +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(1900, ">=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1901, ">=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<float>(1902, ">=-5.5", x => x >= -5.5f, TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<float>(1903, ">=-10", _ => TestItems.ALL),
        FilterTestCase.Create<float>(1904, ">=+5.5", x => x >= +5.5f, TestConfig.FilterCultureEnUs),

        FilterTestCase.Create<float>(2000, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Single'")),

        FilterTestCase.Create<float>(2100, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Single'"))
    ];
    // ReSharper restore CompareOfFloatsByEqualityOperator
}
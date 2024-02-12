using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Filter.Exceptions;
using Schick.Plainquire.Filter.Tests.Extensions;
using Schick.Plainquire.Filter.Tests.Models;
using Schick.Plainquire.Filter.Tests.Services;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForShortBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForShortBySyntax_WorksAsExpected(FilterTestCase<short, short> testCase, EntityFilterFunc<TestModel<short>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<short>[] _testItems =
    [
        new() { ValueA = -9 },
        new() { ValueA = -5 },
        new() { ValueA = -0 },
        new() { ValueA = +5 },
        new() { ValueA = +9 }
    ];

    private static readonly FilterTestCase<short, short>[] _testCases =
    [
        FilterTestCase.Create<short>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1102, "-5", x => x == -5),
        FilterTestCase.Create<short>(1103, "-10", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1104, "+5", x => x == +5),

        FilterTestCase.Create<short>(1200, "~5", x => x is +5 or -5),
        FilterTestCase.Create<short>(1201, "~-5", x => x == -5),
        FilterTestCase.Create<short>(1202, "~3", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1203, "~0", x => x == 0),

        FilterTestCase.Create<short>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1302, "=-5", x => x == -5),
        FilterTestCase.Create<short>(1303, "=-10", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1304, "=+5", x => x == +5),

        FilterTestCase.Create<short>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1402, "==-5", x => x == -5),
        FilterTestCase.Create<short>(1403, "==-10", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1404, "==+5", x => x == +5),

        FilterTestCase.Create<short>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1502, "!-5", x => x != -5),
        FilterTestCase.Create<short>(1503, "!-10", _ => TestItems.ALL),
        FilterTestCase.Create<short>(1504, "!+5", x => x != +5),

        FilterTestCase.Create<short>(1600, "<null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1601, "<", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1602, "<-5", x => x < -5),
        FilterTestCase.Create<short>(1603, "<-10", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1604, "<+5", x => x < +5),

        FilterTestCase.Create<short>(1700, "<=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1701, "<=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1702, "<=-5", x => x <= -5),
        FilterTestCase.Create<short>(1703, "<=-10", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1704, "<=+5", x => x <= +5),

        FilterTestCase.Create<short>(1800, ">null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1801, ">", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1802, ">-5", x => x > -5),
        FilterTestCase.Create<short>(1803, ">-10", _ => TestItems.ALL),
        FilterTestCase.Create<short>(1804, ">+5", x => x > +5),

        FilterTestCase.Create<short>(1900, ">=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1901, ">=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1902, ">=-5", x => x >= -5),
        FilterTestCase.Create<short>(1903, ">=-10", _ => TestItems.ALL),
        FilterTestCase.Create<short>(1904, ">=+5", x => x >= +5),

        FilterTestCase.Create<short>(2000, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Int16'")),

        FilterTestCase.Create<short>(2100, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Int16'"))
    ];
}
using NUnit.Framework;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using Plainquire.TestSupport.Services;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestFixture]
public class FilterForShortBySyntaxTests : TestContainer
{
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForShortBySyntax_WorksAsExpected(FilterTestCase<short, short> testCase, EntityFilterFunc<TestModel<short>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<short>[] _testItems =
    [
        new() { ValueA = -19 },
        new() { ValueA = -05 },
        new() { ValueA = -00 },
        new() { ValueA = +05 },
        new() { ValueA = +19 }
    ];

    private static readonly FilterTestCase<short, short>[] _testCases =
    [
        FilterTestCase.Create<short>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1102, "-5", x => x == -5),
        FilterTestCase.Create<short>(1103, "-20", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1104, "+5", x => x == +5),

        FilterTestCase.Create<short>(1200, "~5", x => x is +5 or -5),
        FilterTestCase.Create<short>(1201, "~-5", x => x == -5),
        FilterTestCase.Create<short>(1202, "~3", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1203, "~0", x => x == 0),

        FilterTestCase.Create<short>(1300, "^", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1301, "^0", x => x == 0),
        FilterTestCase.Create<short>(1302, "^-0", x => x == 0),
        FilterTestCase.Create<short>(1303, "^+0", x => x == 0),
        FilterTestCase.Create<short>(1304, "^1", x => x == 19),
        FilterTestCase.Create<short>(1305, "^+1", x => x is 19),
        FilterTestCase.Create<short>(1306, "^-1", x => x == -19),
        FilterTestCase.Create<short>(1307, "^2", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1308, "^+2", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1309, "^-2", _ => TestItems.NONE),

        FilterTestCase.Create<short>(1400, "$", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1400, "$0", x => x == 0),
        FilterTestCase.Create<short>(1401, "$4", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1402, "$9", x => x is 19 or -19),

        FilterTestCase.Create<short>(1500, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1501, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1502, "=-5", x => x == -5),
        FilterTestCase.Create<short>(1503, "=-20", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1504, "=+5", x => x == +5),

        FilterTestCase.Create<short>(1600, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1601, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1602, "==-5", x => x == -5),
        FilterTestCase.Create<short>(1603, "==-20", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1604, "==+5", x => x == +5),

        FilterTestCase.Create<short>(1700, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1701, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1702, "!-5", x => x != -5),
        FilterTestCase.Create<short>(1703, "!-20", _ => TestItems.ALL),
        FilterTestCase.Create<short>(1704, "!+5", x => x != +5),

        FilterTestCase.Create<short>(1800, "<null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1801, "<", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1802, "<-5", x => x < -5),
        FilterTestCase.Create<short>(1803, "<-20", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1804, "<+5", x => x < +5),

        FilterTestCase.Create<short>(1900, "<=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1901, "<=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(1902, "<=-5", x => x <= -5),
        FilterTestCase.Create<short>(1903, "<=-20", _ => TestItems.NONE),
        FilterTestCase.Create<short>(1904, "<=+5", x => x <= +5),

        FilterTestCase.Create<short>(2000, ">null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(2001, ">", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(2002, ">-5", x => x > -5),
        FilterTestCase.Create<short>(2003, ">-20", _ => TestItems.ALL),
        FilterTestCase.Create<short>(2004, ">+5", x => x > +5),

        FilterTestCase.Create<short>(2100, ">=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(2101, ">=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<short>(2102, ">=-5", x => x >= -5),
        FilterTestCase.Create<short>(2103, ">=-20", _ => TestItems.ALL),
        FilterTestCase.Create<short>(2104, ">=+5", x => x >= +5),

        FilterTestCase.Create<short>(2200, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Int16'")),

        FilterTestCase.Create<short>(2300, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Int16'"))
    ];
}
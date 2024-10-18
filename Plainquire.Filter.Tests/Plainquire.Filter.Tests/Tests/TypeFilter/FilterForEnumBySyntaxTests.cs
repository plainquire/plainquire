using NUnit.Framework;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestFixture]
public class FilterForEnumBySyntaxTests
{
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForEnumBySyntax_WorksAsExpected(FilterTestCase<TestEnum, TestEnum> testCase, EntityFilterFunc<TestModel<TestEnum>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<TestEnum>[] _testItems =
    [
        new() { ValueA = TestEnum.Negative },
        new() { ValueA = TestEnum.Neutral },
        new() { ValueA = TestEnum.Positive },
        new() { ValueA = TestEnum.Positive2 },
        new() { ValueA = TestEnum.Positive4 }
    ];

    private static readonly FilterTestCase<TestEnum, TestEnum>[] _testCases =
    [
        FilterTestCase.Create<TestEnum>(1100, "null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1101, "", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1102, "Negative", x => x == TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1103, "-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1104, "1", x => x == TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1105, "Positive", x => x == TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1106, "positive", x => x == TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(1200, "~null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1201, "~", _ => TestItems.ALL),
        FilterTestCase.Create<TestEnum>(1202, "~Negative", x => x == TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1203, "~-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1204, "~1", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1205, "~Positive", x => x is TestEnum.Positive or TestEnum.Positive2 or TestEnum.Positive4),
        FilterTestCase.Create<TestEnum>(1206, "~positive", x => x is TestEnum.Positive or TestEnum.Positive2 or TestEnum.Positive4),

        FilterTestCase.Create<TestEnum>(1300, "^null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1301, "^", _ => TestItems.ALL),
        FilterTestCase.Create<TestEnum>(1302, "^Negative", x => x == TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1303, "^-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1304, "^1", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1305, "^Positive", x => x is TestEnum.Positive or TestEnum.Positive2 or TestEnum.Positive4),
        FilterTestCase.Create<TestEnum>(1306, "^positive", x => x is TestEnum.Positive or TestEnum.Positive2 or TestEnum.Positive4),

        FilterTestCase.Create<TestEnum>(1400, "$null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1401, "$", _ => TestItems.ALL),
        FilterTestCase.Create<TestEnum>(1402, "$Negative", x => x == TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1403, "$-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1404, "$1", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1405, "$Tive", x => x is TestEnum.Negative or TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1406, "$tive", x => x is TestEnum.Negative or TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(1500, "=null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1501, "=", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1502, "=Negative", x => x == TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1503, "=-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1504, "=1", x => x == TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1505, "=Positive", x => x == TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1506, "=positive", x => x == TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(1600, "==null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1601, "==", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1602, "==Negative", x => x == TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1603, "==-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1604, "==1", x => x == TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1605, "==Positive", x => x == TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1606, "==positive", _ => TestItems.NONE),

        FilterTestCase.Create<TestEnum>(1700, "!null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1701, "!", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1702, "!Negative", x => x != TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1703, "!-10", _ => TestItems.ALL),
        FilterTestCase.Create<TestEnum>(1704, "!1", x => x != TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1705, "!Positive", x => x != TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1706, "!positive", x => x != TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(1800, "<null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1801, "<", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1802, "<Negative", x => x < TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1803, "<-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1804, "<1", x => x < TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1805, "<Positive", x => x < TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1806, "<positive", x => x < TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(1900, "<=null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1901, "<=", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1902, "<=Negative", x => x <= TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(1903, "<=-10", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(1904, "<=1", x => x <= TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1905, "<=Positive", x => x <= TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(1906, "<=positive", x => x <= TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(2000, ">null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(2001, ">", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(2002, ">Negative", x => x > TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(2003, ">-10", _ => TestItems.ALL),
        FilterTestCase.Create<TestEnum>(2004, ">1", x => x > TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(2005, ">Positive", x => x > TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(2006, ">positive", x => x > TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(2100, ">=null", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(2101, ">=", _ => TestItems.NONE),
        FilterTestCase.Create<TestEnum>(2102, ">=Negative", x => x >= TestEnum.Negative),
        FilterTestCase.Create<TestEnum>(2103, ">=-10", _ => TestItems.ALL),
        FilterTestCase.Create<TestEnum>(2104, ">=1", x => x >= TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(2105, ">=Positive", x => x >= TestEnum.Positive),
        FilterTestCase.Create<TestEnum>(2106, ">=positive", x => x >= TestEnum.Positive),

        FilterTestCase.Create<TestEnum>(2200, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'Plainquire.Filter.Tests.Models.TestEnum'")),

        FilterTestCase.Create<TestEnum>(2300, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'Plainquire.Filter.Tests.Models.TestEnum'"))
    ];
}
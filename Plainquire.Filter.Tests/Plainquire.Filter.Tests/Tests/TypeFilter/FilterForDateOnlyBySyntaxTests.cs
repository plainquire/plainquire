using NUnit.Framework;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using Plainquire.TestSupport.Services;
using System;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestFixture]
public class FilterForDateOnlyBySyntaxTests : TestContainer
{
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForDateOnlyBySyntax_WorksAsExpected(FilterTestCase<DateOnly, DateOnly> testCase, EntityFilterFunc<TestModel<DateOnly>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<DateOnly>[] _testItems =
    [
        new() { ValueA = new DateOnly(1900, 01, 01) },
        new() { ValueA = new DateOnly(2000, 01, 01) },
        new() { ValueA = new DateOnly(2010, 01, 01) },
        new() { ValueA = new DateOnly(2010, 06, 01) },
        new() { ValueA = new DateOnly(2010, 06, 15) },
        new() { ValueA = new DateOnly(2010, 06, 16) },
        new() { ValueA = new DateOnly(2010, 07, 01) },
        new() { ValueA = new DateOnly(2011, 01, 01) },
        new() { ValueA = new DateOnly(2020, 01, 01) }
    ];

    private static readonly FilterTestCase<DateOnly, DateOnly>[] _testCases =
    [
        FilterTestCase.Create<DateOnly>(1000, "null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<DateOnly>(1001, "=null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<DateOnly>(1002, "~2010", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2011, 01, 01)),
        FilterTestCase.Create<DateOnly>(1003, "~2010-06", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 07, 01)),
        FilterTestCase.Create<DateOnly>(1004, "~2010-06-15", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 16)),
        FilterTestCase.Create<DateOnly>(1005, "~2010-06-15T12", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1006, "~2010-06-15T12:30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1007, "~2010 06 15 12 30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1008, "~2010/06/15/12/30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1009, "~01.06.2010", x => x >= new DateOnly(2010, 01, 06) && x < new DateOnly(2010, 01, 07), TestConfig.FilterCultureEnUs),
        FilterTestCase.Create<DateOnly>(1010, "~01.06.2010", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 07, 01), TestConfig.FilterCultureDeDe),
        FilterTestCase.Create<DateOnly>(1011, "one-month-ago", x => x >= new DateOnly(2010, 05, 16) && x < new DateOnly(2010, 06, 16), null, new DateInterceptor { Now = () => new DateTime(2010, 06, 16) }),
        FilterTestCase.Create<DateOnly>(1012, "june 1st_june-16th", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 16), null, new DateInterceptor { Now = () => new DateTime(2010, 06, 16) }),
        FilterTestCase.Create<DateOnly>(1013, "2010_2020", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2021, 01, 01)),
        FilterTestCase.Create<DateOnly>(1014, "2010-06_2010-07", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 08, 01)),
        FilterTestCase.Create<DateOnly>(1015, "2010-06-01_2010-06-01", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 02)),
        FilterTestCase.Create<DateOnly>(1016, "2010-06-01_2010-06-15T12:31:00", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1017, "2010-13-40", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1018, "2010-13-40_2020-01-01", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1019, "INVALID_june-16th", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1020, "june 1st_INVALID", new FilterExpressionException("Unable to parse given filter value")),

        FilterTestCase.Create<DateOnly>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1102, "2010", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2011, 01, 01)),
        FilterTestCase.Create<DateOnly>(1103, "2010-01", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2010, 02, 01)),
        FilterTestCase.Create<DateOnly>(1104, "2010-01-01", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2010, 01, 02)),
        FilterTestCase.Create<DateOnly>(1105, "2010-06-15", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 16)),
        FilterTestCase.Create<DateOnly>(1106, "2010-06-15T12", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1107, "2010-06-15T12:30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1108, "2010-06-15T12:30:30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1109, "2100-01-01", _ => TestItems.NONE),
        FilterTestCase.Create<DateOnly>(1110, "2010-06-01_2010-06-15T12:31:00", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 15)),

        FilterTestCase.Create<DateOnly>(1200, "~null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1201, "~", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1202, "~2010", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2011, 01, 01)),
        FilterTestCase.Create<DateOnly>(1203, "~2010-01", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2010, 02, 01)),
        FilterTestCase.Create<DateOnly>(1204, "~2010-01-01", x => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2010, 01, 02)),
        FilterTestCase.Create<DateOnly>(1205, "~2010-06-15", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 16)),
        FilterTestCase.Create<DateOnly>(1206, "~2010-06-15T12", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1207, "~2010-06-15T12:30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1208, "~2010-06-15T12:30:30", x => x >= new DateOnly(2010, 06, 15) && x < new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1209, "~2100-01-01", _ => TestItems.NONE),
        FilterTestCase.Create<DateOnly>(1210, "~2010-06-01_2010-06-15T12:31:00", x => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 15)),

        FilterTestCase.Create<DateOnly>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1302, "=2010-01-01", x => x == new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1303, "=2010-06-15T12:30:30", x => x == new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1304, "=2010-01-01_2020-01-01", x => x == new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1402, "==2010-01-01", x => x == new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1403, "==2010-06-15T12:30:30", x => x == new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1404, "==2010-01-01_2020-01-01", x => x == new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1502, "!2010-01-01", x => x != new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1503, "!2010-06-15T12:30:30", x => x != new DateOnly(2010, 06, 15)),
        FilterTestCase.Create<DateOnly>(1504, "!2010-01-01_2020-01-01", x => x != new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(1600, "<null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1601, "<", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1602, "<2010-01-01", x => x < new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1603, "<2010-01-01_2020-01-01", x => x < new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(1700, "<=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1701, "<=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1702, "<=2010-01-01", x => x <= new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1703, "<=2010-01-01_2020-01-01", x => x <= new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(1800, ">null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1801, ">", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1802, ">2010-01-01", x => x > new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1803, ">2010-01-01_2020-01-01", x => x > new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(1900, ">=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1901, ">=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateOnly>(1902, ">=2010-01-01", x => x >= new DateOnly(2010, 01, 01)),
        FilterTestCase.Create<DateOnly>(1903, ">=2010-01-01_2020-01-01", x => x >= new DateOnly(2010, 01, 01)),

        FilterTestCase.Create<DateOnly>(2000, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.DateOnly'")),

        FilterTestCase.Create<DateOnly>(2100, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.DateOnly'"))
    ];
}
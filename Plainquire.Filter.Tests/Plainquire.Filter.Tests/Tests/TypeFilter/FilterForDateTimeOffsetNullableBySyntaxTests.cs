using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForDateTimeOffsetNullableBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForDateTimeNullableBySyntax_WorksAsExpected(FilterTestCase<DateTimeOffset?, DateTimeOffset?> testCase, EntityFilterFunc<TestModel<DateTimeOffset?>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<DateTimeOffset?>[] _testItems =
    [
        new() { ValueA = null },
        new() { ValueA = new DateTimeOffset(1900, 01, 01, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) },
        new() { ValueA = new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) },
        new() { ValueA = new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 12, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 12, 30, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 12, 30, 31, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 15, 13, 00, 00, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2010, 07, 01, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero) },
        new() { ValueA = new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero) }
    ];

    private static readonly FilterTestCase<DateTimeOffset?, DateTimeOffset?>[] _testCases =
    [
        FilterTestCase.Create<DateTimeOffset?>(1000, "null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<DateTimeOffset?>(1001, "=null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<DateTimeOffset?>(1002, "~2010", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1003, "~2010+02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) && x < new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1004, "~2010-02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) && x < new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1005, "~2010-06", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 07, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1006, "~2010-06-15", x => x >= new DateTimeOffset(2010, 06, 15, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1007, "~2010-06-15T12", x => x >= new DateTimeOffset(2010, 06, 15, 12, 00, 00, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 13, 00, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1008, "~2010-06-15T12:30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 00, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1009, "~2010 06 15 12 30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 00, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1010, "~2010/06/15/12/30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 00, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1011, "~01.06.2010", x => x >= new DateTimeOffset(2010, 01, 06, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 07, 0, 0, 0, TimeSpan.Zero), TestConfig.CultureEnUs),
        FilterTestCase.Create<DateTimeOffset?>(1012, "~01.06.2010", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 07, 01, 0, 0, 0, TimeSpan.Zero), TestConfig.CultureDeDe),
        FilterTestCase.Create<DateTimeOffset?>(1013, "one-month-ago", x => x >= new DateTimeOffset(2010, 05, 16, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero), new FilterConfiguration { Now = () => new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero) }),
        FilterTestCase.Create<DateTimeOffset?>(1014, "june 1st_june-16th", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero), new FilterConfiguration { Now = () => new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero) }),
        FilterTestCase.Create<DateTimeOffset?>(1015, "2010_2020", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2021, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1016, "2010-06_2010-07", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 08, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1017, "2010-06-01_2010-06-01", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 02, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1018, "2010-06-01_2010-06-15T12:31:00", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 32, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1019, "2010-13-40", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1020, "2010-13-40_2020-01-01", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1021, "INVALID_june-16th", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1022, "june 1st_INVALID", new FilterExpressionException("Unable to parse given filter value")),

        FilterTestCase.Create<DateTimeOffset?>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1102, "2010-01-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1103, "2010-01-01+02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1104, "2010-01-01-02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1105, "2010-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 02, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1106, "2010-01-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1107, "2010-06-15", x => x >= new DateTimeOffset(2010, 06, 15, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1108, "2010-06-15T12", x => x >= new DateTimeOffset(2010, 06, 15, 12, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 13, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1109, "2010-06-15T12:30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1110, "2010-06-15T12:30:30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 30, 31, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1111, "2100-01-01", _ => TestItems.NONE),
        FilterTestCase.Create<DateTimeOffset?>(1112, "2010-06-01_2010-06-15T12:31:00", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 32, 00, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1200, "~null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1201, "~", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1202, "~2010-01-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1203, "~2010-01-01+02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1204, "~2010-01-01-02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1205, "~2010", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1206, "~2010-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 02, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1207, "~2010-01-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 02, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1208, "~2010-06-15", x => x >= new DateTimeOffset(2010, 06, 15, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 16, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1209, "~2010-06-15T12", x => x >= new DateTimeOffset(2010, 06, 15, 12, 00, 00, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 13, 00, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1210, "~2010-06-15T12:30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 00, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1211, "~2010-06-15T12:30:30", x => x >= new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 30, 31, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1212, "~2100-01-01", _ => TestItems.NONE),
        FilterTestCase.Create<DateTimeOffset?>(1213, "~2010-06-01_2010-06-15T12:31:00", x => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 32, 00, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1302, "=2010-01-01+02:00", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1303, "=2010-01-01", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1304, "=2010-01-01-02:00", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1305, "=2010-06-15T12:30:30", x => x == new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1306, "=2010-01-01_2020-01-01", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1402, "==2010-01-01+02:00", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1403, "==2010-01-01", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1404, "==2010-01-01-02:00", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1405, "==2010-06-15T12:30:30", x => x == new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1406, "==2010-01-01_2020-01-01", x => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1502, "!2010-01-01+02:00", x => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1503, "!2010-01-01", x => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1504, "!2010-01-01-02:00", x => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1505, "!2010-06-15T12:30:30", x => x != new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1506, "!2010-01-01_2020-01-01", x => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1600, "<null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1601, "<", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1602, "<2010-01-01+02:00", x => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1603, "<2010-01-01", x => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1604, "<2010-01-01-02:00", x => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1605, "<2010-01-01_2020-01-01", x => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1700, "<=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1701, "<=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1702, "<=2010-01-01+02:00", x => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1703, "<=2010-01-01", x => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1704, "<=2010-01-01-02:00", x => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1705, "<=2010-01-01_2020-01-01", x => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1800, ">null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1801, ">", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1802, ">2010-01-01+02:00", x => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1803, ">2010-01-01", x => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1804, ">2010-01-01-02:00", x => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1805, ">2010-01-01_2020-01-01", x => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(1900, ">=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1901, ">=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTimeOffset?>(1902, ">=2010-01-01+02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create<DateTimeOffset?>(1903, ">=2010-01-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create<DateTimeOffset?>(1904, ">=2010-01-01-02:00", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create<DateTimeOffset?>(1905, ">=2010-01-01_2020-01-01", x => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create<DateTimeOffset?>(2000, "ISNULL", x => x == null),

        FilterTestCase.Create<DateTimeOffset?>(2100, "NOTNULL", x => x != null)
    ];
}
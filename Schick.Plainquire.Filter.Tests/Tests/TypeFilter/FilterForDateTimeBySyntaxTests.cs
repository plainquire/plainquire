using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Filter.Abstractions.Configurations;
using Schick.Plainquire.Filter.Exceptions;
using Schick.Plainquire.Filter.Tests.Extensions;
using Schick.Plainquire.Filter.Tests.Models;
using Schick.Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Tests.TypeFilter;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestClass, ExcludeFromCodeCoverage]
public class FilterForDateTimeBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForDateTimeBySyntax_WorksAsExpected(FilterTestCase<DateTime, DateTime> testCase, EntityFilterFunc<TestModel<DateTime>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<DateTime>[] _testItems =
    [
        new() { ValueA = new DateTime(1900, 01, 01) },
        new() { ValueA = new DateTime(2000, 01, 01) },
        new() { ValueA = new DateTime(2010, 01, 01) },
        new() { ValueA = new DateTime(2010, 06, 01) },
        new() { ValueA = new DateTime(2010, 06, 15) },
        new() { ValueA = new DateTime(2010, 06, 15, 12, 0, 0) },
        new() { ValueA = new DateTime(2010, 06, 15, 12, 30, 0) },
        new() { ValueA = new DateTime(2010, 06, 15, 12, 30, 30) },
        new() { ValueA = new DateTime(2010, 06, 15, 12, 30, 31) },
        new() { ValueA = new DateTime(2010, 06, 15, 12, 31, 00) },
        new() { ValueA = new DateTime(2010, 06, 15, 13, 00, 00) },
        new() { ValueA = new DateTime(2010, 06, 16) },
        new() { ValueA = new DateTime(2010, 07, 01) },
        new() { ValueA = new DateTime(2011, 01, 01) },
        new() { ValueA = new DateTime(2020, 01, 01) }
    ];

    private static readonly FilterTestCase<DateTime, DateTime>[] _testCases =
    [
        FilterTestCase.Create<DateTime>(1000, "null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<DateTime>(1001, "=null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<DateTime>(1002, "~2010", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2011, 01, 01)),
        FilterTestCase.Create<DateTime>(1003, "~2010-06", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 07, 01)),
        FilterTestCase.Create<DateTime>(1004, "~2010-06-15", x => x >= new DateTime(2010, 06, 15) && x < new DateTime(2010, 06, 16)),
        FilterTestCase.Create<DateTime>(1005, "~2010-06-15T12", x => x >= new DateTime(2010, 06, 15, 12, 00, 00) && x < new DateTime(2010, 06, 15, 13, 00, 00)),
        FilterTestCase.Create<DateTime>(1006, "~2010-06-15T12:30", x => x >= new DateTime(2010, 06, 15, 12, 30, 00) && x < new DateTime(2010, 06, 15, 12, 31, 00)),
        FilterTestCase.Create<DateTime>(1007, "~2010 06 15 12 30", x => x >= new DateTime(2010, 06, 15, 12, 30, 00) && x < new DateTime(2010, 06, 15, 12, 31, 00)),
        FilterTestCase.Create<DateTime>(1008, "~2010/06/15/12/30", x => x >= new DateTime(2010, 06, 15, 12, 30, 00) && x < new DateTime(2010, 06, 15, 12, 31, 00)),
        FilterTestCase.Create<DateTime>(1009, "~01.06.2010", x => x >= new DateTime(2010, 01, 06) && x < new DateTime(2010, 01, 07), TestConfig.CultureEnUs),
        FilterTestCase.Create<DateTime>(1010, "~01.06.2010", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 07, 01), TestConfig.CultureDeDe),
        FilterTestCase.Create<DateTime>(1011, "one-month-ago", x => x >= new DateTime(2010, 05, 16) && x < new DateTime(2010, 06, 16), new FilterConfiguration { Now = () => new DateTime(2010, 06, 16) }),
        FilterTestCase.Create<DateTime>(1012, "june 1st_june-16th", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 06, 16), new FilterConfiguration { Now = () => new DateTime(2010, 06, 16) }),
        FilterTestCase.Create<DateTime>(1013, "2010_2020", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2021, 01, 01)),
        FilterTestCase.Create<DateTime>(1014, "2010-06_2010-07", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 08, 01)),
        FilterTestCase.Create<DateTime>(1015, "2010-06-01_2010-06-01", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 06, 02)),
        FilterTestCase.Create<DateTime>(1016, "2010-06-01_2010-06-15T12:31:00", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 06, 15, 12, 32, 00)),
        FilterTestCase.Create<DateTime>(1017, "2010-13-40", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1018, "2010-13-40_2020-01-01", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1019, "INVALID_june-16th", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1020, "june 1st_INVALID", new FilterExpressionException("Unable to parse given filter value")),

        FilterTestCase.Create<DateTime>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1102, "2010", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2011, 01, 01)),
        FilterTestCase.Create<DateTime>(1103, "2010-01", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2010, 02, 01)),
        FilterTestCase.Create<DateTime>(1104, "2010-01-01", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2010, 01, 02)),
        FilterTestCase.Create<DateTime>(1105, "2010-06-15", x => x >= new DateTime(2010, 06, 15) && x < new DateTime(2010, 06, 16)),
        FilterTestCase.Create<DateTime>(1106, "2010-06-15T12", x => x >= new DateTime(2010, 06, 15, 12, 00, 00) && x < new DateTime(2010, 06, 15, 13, 00, 00)),
        FilterTestCase.Create<DateTime>(1107, "2010-06-15T12:30", x => x >= new DateTime(2010, 06, 15, 12, 30, 00) && x < new DateTime(2010, 06, 15, 12, 31, 00)),
        FilterTestCase.Create<DateTime>(1108, "2010-06-15T12:30:30", x => x >= new DateTime(2010, 06, 15, 12, 30, 30) && x < new DateTime(2010, 06, 15, 12, 30, 31)),
        FilterTestCase.Create<DateTime>(1109, "2100-01-01", _ => TestItems.NONE),
        FilterTestCase.Create<DateTime>(1110, "2010-06-01_2010-06-15T12:31:00", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 06, 15, 12, 32, 00)),

        FilterTestCase.Create<DateTime>(1200, "~null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1201, "~", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1202, "~2010", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2011, 01, 01)),
        FilterTestCase.Create<DateTime>(1203, "~2010-01", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2010, 02, 01)),
        FilterTestCase.Create<DateTime>(1204, "~2010-01-01", x => x >= new DateTime(2010, 01, 01) && x < new DateTime(2010, 01, 02)),
        FilterTestCase.Create<DateTime>(1205, "~2010-06-15", x => x >= new DateTime(2010, 06, 15) && x < new DateTime(2010, 06, 16)),
        FilterTestCase.Create<DateTime>(1206, "~2010-06-15T12", x => x >= new DateTime(2010, 06, 15, 12, 00, 00) && x < new DateTime(2010, 06, 15, 13, 00, 00)),
        FilterTestCase.Create<DateTime>(1207, "~2010-06-15T12:30", x => x >= new DateTime(2010, 06, 15, 12, 30, 00) && x < new DateTime(2010, 06, 15, 12, 31, 00)),
        FilterTestCase.Create<DateTime>(1208, "~2010-06-15T12:30:30", x => x >= new DateTime(2010, 06, 15, 12, 30, 30) && x < new DateTime(2010, 06, 15, 12, 30, 31)),
        FilterTestCase.Create<DateTime>(1209, "~2100-01-01", _ => TestItems.NONE),
        FilterTestCase.Create<DateTime>(1210, "~2010-06-01_2010-06-15T12:31:00", x => x >= new DateTime(2010, 06, 01) && x < new DateTime(2010, 06, 15, 12, 32, 00)),

        FilterTestCase.Create<DateTime>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1302, "=2010-01-01", x => x == new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1303, "=2010-06-15T12:30:30", x => x == new DateTime(2010, 06, 15, 12, 30, 30)),
        FilterTestCase.Create<DateTime>(1304, "=2010-01-01_2020-01-01", x => x == new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1402, "==2010-01-01", x => x == new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1403, "==2010-06-15T12:30:30", x => x == new DateTime(2010, 06, 15, 12, 30, 30)),
        FilterTestCase.Create<DateTime>(1404, "==2010-01-01_2020-01-01", x => x == new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1502, "!2010-01-01", x => x != new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1503, "!2010-06-15T12:30:30", x => x != new DateTime(2010, 06, 15, 12, 30, 30)),
        FilterTestCase.Create<DateTime>(1504, "!2010-01-01_2020-01-01", x => x != new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(1600, "<null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1601, "<", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1602, "<2010-01-01", x => x < new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1603, "<2010-01-01_2020-01-01", x => x < new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(1700, "<=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1701, "<=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1702, "<=2010-01-01", x => x <= new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1703, "<=2010-01-01_2020-01-01", x => x <= new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(1800, ">null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1801, ">", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1802, ">2010-01-01", x => x > new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1803, ">2010-01-01_2020-01-01", x => x > new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(1900, ">=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1901, ">=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<DateTime>(1902, ">=2010-01-01", x => x >= new DateTime(2010, 01, 01)),
        FilterTestCase.Create<DateTime>(1903, ">=2010-01-01_2020-01-01", x => x >= new DateTime(2010, 01, 01)),

        FilterTestCase.Create<DateTime>(2000, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.DateTime'")),

        FilterTestCase.Create<DateTime>(2100, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.DateTime'"))
    ];
}
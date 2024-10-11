using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass]
public class FilterForDateTimeOffsetByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForDateTimeByValue_WorksAsExpected(object testCase, EntityFilterFunc<TestModel<DateTimeOffset>> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<DateTimeOffset, DateTimeOffset> dateTimeTestCase:
                dateTimeTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<Range<DateTimeOffset>, DateTimeOffset> dateTimeSpanTestCase:
                dateTimeSpanTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new InvalidOperationException("Unsupported test case");
        }
    }

    private static readonly TestModel<DateTimeOffset>[] _testItems =
    [
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

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly object[] _testCases =
    [
        FilterTestCase.Create(1100, FilterOperator.Default, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 01, 0, 0, 1, TimeSpan.Zero)),
        FilterTestCase.Create(1101, FilterOperator.Default, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) && x < new DateTimeOffset(2010, 01, 01, 0, 0, 1, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1102, FilterOperator.Default, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) && x < new DateTimeOffset(2010, 01, 01, 0, 0, 1, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1103, FilterOperator.Default, new DateTimeOffset[] { new(2100, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset _) => TestItems.NONE),
        FilterTestCase.Create(1104, FilterOperator.Default, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 01, TimeSpan.Zero)),

        FilterTestCase.Create(1201, FilterOperator.Contains, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 01, 01, 0, 0, 1, TimeSpan.Zero)),
        FilterTestCase.Create(1202, FilterOperator.Contains, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) && x < new DateTimeOffset(2010, 01, 01, 0, 0, 1, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1203, FilterOperator.Contains, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) && x < new DateTimeOffset(2010, 01, 01, 0, 0, 1, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1204, FilterOperator.Contains, new DateTimeOffset[] { new(2100, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset _) => TestItems.NONE),
        FilterTestCase.Create(1205, FilterOperator.Contains, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 06, 15, 12, 31, 00, TimeSpan.Zero)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 06, 01, 0, 0, 0, TimeSpan.Zero) && x < new DateTimeOffset(2010, 06, 15, 12, 31, 01, TimeSpan.Zero)),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, new DateTimeOffset[] { new(2010, 06, 15, 12, 30, 30, TimeSpan.Zero) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero)),
        FilterTestCase.Create(1304, FilterOperator.EqualCaseInsensitive, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, new DateTimeOffset[] { new(2010, 06, 15, 12, 30, 30, TimeSpan.Zero) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero)),
        FilterTestCase.Create(1404, FilterOperator.EqualCaseSensitive, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x == new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(1500, FilterOperator.NotEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1501, FilterOperator.NotEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1502, FilterOperator.NotEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1503, FilterOperator.NotEqual, new DateTimeOffset[] { new(2010, 06, 15, 12, 30, 30, TimeSpan.Zero) }, (DateTimeOffset x) => x != new DateTimeOffset(2010, 06, 15, 12, 30, 30, TimeSpan.Zero)),
        FilterTestCase.Create(1504, FilterOperator.NotEqual, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x != new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(1600, FilterOperator.LessThan, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1601, FilterOperator.LessThan, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1602, FilterOperator.LessThan, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1603, FilterOperator.LessThan, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x < new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1701, FilterOperator.LessThanOrEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1703, FilterOperator.LessThanOrEqual, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x <= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1801, FilterOperator.GreaterThan, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1802, FilterOperator.GreaterThan, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1803, FilterOperator.GreaterThan, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x > new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(2))),
        FilterTestCase.Create(1901, FilterOperator.GreaterThanOrEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.Zero) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),
        FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, new DateTimeOffset[] { new(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.FromHours(-2))),
        FilterTestCase.Create(1903, FilterOperator.GreaterThanOrEqual, new Range<DateTimeOffset>[] { new(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 01, 01, 0, 0, 0, TimeSpan.Zero)) }, (DateTimeOffset x) => x >= new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero)),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new DateTimeOffset[] { default }, new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.DateTimeOffset'")),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new DateTimeOffset[] { default }, new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.DateTimeOffset'"))
    ];
    // ReSharper restore RedundantExplicitArrayCreation
}
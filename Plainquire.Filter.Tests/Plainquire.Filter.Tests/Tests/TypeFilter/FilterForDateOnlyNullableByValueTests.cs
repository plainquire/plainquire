using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using Plainquire.TestSupport.Services;
using System;
using System.Diagnostics;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestFixture]
public class FilterForDateOnlyNullableByValueTests : TestContainer
{
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForDateOnlyNullableByValue_WorksAsExpected(object testCase, EntityFilterFunc<TestModel<DateOnly?>> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<DateOnly?, DateOnly?> dateTimeTestCase:
                dateTimeTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<Range<DateTimeOffset>, DateOnly?> dateTimeSpanTestCase:
                dateTimeSpanTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new UnreachableException("Unsupported test case");
        }
    }

    private static readonly TestModel<DateOnly?>[] _testItems =
    [
        new() { ValueA = null },
        new() { ValueA = new DateOnly(1900, 01, 01) },
        new() { ValueA = new DateOnly(2000, 01, 01) },
        new() { ValueA = new DateOnly(2010, 01, 01) },
        new() { ValueA = new DateOnly(2010, 06, 01) },
        new() { ValueA = new DateOnly(2010, 06, 15) },
        new() { ValueA = new DateOnly(2010, 06, 15) },
        new() { ValueA = new DateOnly(2010, 06, 16) },
        new() { ValueA = new DateOnly(2010, 07, 01) },
        new() { ValueA = new DateOnly(2011, 01, 01) },
        new() { ValueA = new DateOnly(2020, 01, 01) }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly object[] _testCases =
    [
        FilterTestCase.Create(1100, FilterOperator.Default, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2010, 01, 02)),
        FilterTestCase.Create(1101, FilterOperator.Default, new DateOnly?[] { new(2100, 01, 01) }, (DateOnly? _) => TestItems.NONE),
        FilterTestCase.Create(1102, FilterOperator.Default, new Range<DateTimeOffset>[] { new(new DateTime(2010, 06, 01), new DateTime(2010, 06, 15)) }, (DateOnly? x) => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 15)),

        FilterTestCase.Create(1200, FilterOperator.Contains, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x >= new DateOnly(2010, 01, 01) && x < new DateOnly(2010, 01, 02)),
        FilterTestCase.Create(1201, FilterOperator.Contains, new DateOnly?[] { new(2100, 01, 01) }, (DateOnly? _) => TestItems.NONE),
        FilterTestCase.Create(1202, FilterOperator.Contains, new Range<DateTimeOffset>[] { new(new DateTime(2010, 06, 01), new DateTime(2010, 06, 15)) }, (DateOnly? x) => x >= new DateOnly(2010, 06, 01) && x < new DateOnly(2010, 06, 15)),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x == new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new DateOnly?[] { new(2010, 06, 15) }, (DateOnly? x) => x == new DateOnly(2010, 06, 15)),
        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x == new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x == new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new DateOnly?[] { new(2010, 06, 15) }, (DateOnly? x) => x == new DateOnly(2010, 06, 15)),
        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x == new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(1500, FilterOperator.NotEqual, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x != new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1501, FilterOperator.NotEqual, new DateOnly?[] { new(2010, 06, 15) }, (DateOnly? x) => x != new DateOnly(2010, 06, 15)),
        FilterTestCase.Create(1502, FilterOperator.NotEqual, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x != new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(1600, FilterOperator.LessThan, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x < new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1601, FilterOperator.LessThan, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x < new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x <= new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1701, FilterOperator.LessThanOrEqual, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x <= new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x > new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1801, FilterOperator.GreaterThan, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x > new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new DateOnly?[] { new(2010, 01, 01) }, (DateOnly? x) => x >= new DateOnly(2010, 01, 01)),
        FilterTestCase.Create(1901, FilterOperator.GreaterThanOrEqual, new Range<DateTimeOffset>[] { new(new DateTime(2010, 01, 01), new DateTime(2020, 01, 01)) }, (DateOnly? x) => x >= new DateOnly(2010, 01, 01)),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new DateOnly?[] { default }, (DateOnly? x) => x == null),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new DateOnly?[] { default }, (DateOnly? x) => x != null)
    ];
    // ReSharper restore RedundantExplicitArrayCreation
}
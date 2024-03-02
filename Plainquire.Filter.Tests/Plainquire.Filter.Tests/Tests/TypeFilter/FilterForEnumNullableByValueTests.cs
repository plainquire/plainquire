using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForEnumNullableByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForEnumNullableByValue_WorksAsExpected(object testCase, EntityFilterFunc<TestModel<TestEnum?>> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<TestEnum?, TestEnum?> enumTestCase:
                enumTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<short, TestEnum?> shortTestCase:
                shortTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<int, TestEnum?> intTestCase:
                intTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<string, TestEnum?> stringTestCase:
                stringTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new InvalidOperationException("Unsupported test case");
        }
    }

    private static readonly TestModel<TestEnum?>[] _testItems =
    [
        new() { ValueA = null },
        new() { ValueA = TestEnum.Negative },
        new() { ValueA = TestEnum.Neutral },
        new() { ValueA = TestEnum.Positive },
        new() { ValueA = TestEnum.Positive2 },
        new() { ValueA = TestEnum.Positive4 }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly object[] _testCases =
    [
        FilterTestCase.Create(1100, FilterOperator.Default, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x == TestEnum.Negative),
        FilterTestCase.Create(1401, FilterOperator.Default, [-10], (TestEnum? _) => TestItems.NONE),
        FilterTestCase.Create(1102, FilterOperator.Default, [(int)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1103, FilterOperator.Default, [(short)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1104, FilterOperator.Default, ["Positive"], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1105, FilterOperator.Default, ["positive"], (TestEnum? x) => x == TestEnum.Positive),

        FilterTestCase.Create(1200, FilterOperator.Contains, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x == TestEnum.Negative),
        FilterTestCase.Create(1201, FilterOperator.Contains, [-10], (TestEnum? _) => TestItems.NONE),
        FilterTestCase.Create(1202, FilterOperator.Contains, [(int)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1203, FilterOperator.Contains, [(short)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1204, FilterOperator.Contains, ["Positive"], (TestEnum? x) => x is TestEnum.Positive or TestEnum.Positive2 or TestEnum.Positive4),
        FilterTestCase.Create(1205, FilterOperator.Contains, ["positive"], (TestEnum? x) => x is TestEnum.Positive or TestEnum.Positive2 or TestEnum.Positive4),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x == TestEnum.Negative),
        FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, [-10], (TestEnum? _) => TestItems.NONE),
        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, [(int)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, [(short)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1304, FilterOperator.EqualCaseInsensitive, ["Positive"], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1305, FilterOperator.EqualCaseInsensitive, ["positive"], (TestEnum? x) => x == TestEnum.Positive),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x == TestEnum.Negative),
        FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, [-10], (TestEnum? _) => TestItems.NONE),
        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, [(int)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, [(short)TestEnum.Positive], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1404, FilterOperator.EqualCaseSensitive, ["Positive"], (TestEnum? x) => x == TestEnum.Positive),
        FilterTestCase.Create(1405, FilterOperator.EqualCaseSensitive, ["positive"], (TestEnum? _) => TestItems.NONE),

        FilterTestCase.Create(1500, FilterOperator.NotEqual, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x != TestEnum.Negative),
        FilterTestCase.Create(1501, FilterOperator.NotEqual, [-10], (TestEnum? _) => TestItems.ALL),
        FilterTestCase.Create(1502, FilterOperator.NotEqual, [(int)TestEnum.Positive], (TestEnum? x) => x != TestEnum.Positive),
        FilterTestCase.Create(1503, FilterOperator.NotEqual, [(short)TestEnum.Positive], (TestEnum? x) => x != TestEnum.Positive),
        FilterTestCase.Create(1504, FilterOperator.NotEqual, ["Positive"], (TestEnum? x) => x != TestEnum.Positive),
        FilterTestCase.Create(1505, FilterOperator.NotEqual, ["positive"], (TestEnum? x) => x != TestEnum.Positive),

        FilterTestCase.Create(1600, FilterOperator.LessThan, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x < TestEnum.Negative),
        FilterTestCase.Create(1601, FilterOperator.LessThan, [-10], (TestEnum? _) => TestItems.NONE),
        FilterTestCase.Create(1602, FilterOperator.LessThan, [(int)TestEnum.Positive], (TestEnum? x) => x < TestEnum.Positive),
        FilterTestCase.Create(1603, FilterOperator.LessThan, [(short)TestEnum.Positive], (TestEnum? x) => x < TestEnum.Positive),
        FilterTestCase.Create(1604, FilterOperator.LessThan, ["Positive"], (TestEnum? x) => x < TestEnum.Positive),
        FilterTestCase.Create(1605, FilterOperator.LessThan, ["positive"], (TestEnum? x) => x < TestEnum.Positive),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x <= TestEnum.Negative),
        FilterTestCase.Create(1701, FilterOperator.LessThanOrEqual, [-10], (TestEnum? _) => TestItems.NONE),
        FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, [(int)TestEnum.Positive], (TestEnum? x) => x <= TestEnum.Positive),
        FilterTestCase.Create(1703, FilterOperator.LessThanOrEqual, [(short)TestEnum.Positive], (TestEnum? x) => x <= TestEnum.Positive),
        FilterTestCase.Create(1704, FilterOperator.LessThanOrEqual, ["Positive"], (TestEnum? x) => x <= TestEnum.Positive),
        FilterTestCase.Create(1705, FilterOperator.LessThanOrEqual, ["positive"], (TestEnum? x) => x <= TestEnum.Positive),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x > TestEnum.Negative),
        FilterTestCase.Create(1801, FilterOperator.GreaterThan, [-10], (TestEnum? x) => x != null),
        FilterTestCase.Create(1802, FilterOperator.GreaterThan, [(int)TestEnum.Positive], (TestEnum? x) => x > TestEnum.Positive),
        FilterTestCase.Create(1803, FilterOperator.GreaterThan, [(short)TestEnum.Positive], (TestEnum? x) => x > TestEnum.Positive),
        FilterTestCase.Create(1804, FilterOperator.GreaterThan, ["Positive"], (TestEnum? x) => x > TestEnum.Positive),
        FilterTestCase.Create(1805, FilterOperator.GreaterThan, ["positive"], (TestEnum? x) => x > TestEnum.Positive),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new TestEnum?[] { TestEnum.Negative }, (TestEnum? x) => x >= TestEnum.Negative),
        FilterTestCase.Create(1901, FilterOperator.GreaterThanOrEqual, [-10], (TestEnum? x) => x != null),
        FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, [(int)TestEnum.Positive], (TestEnum? x) => x >= TestEnum.Positive),
        FilterTestCase.Create(1903, FilterOperator.GreaterThanOrEqual, [(short)TestEnum.Positive], (TestEnum? x) => x >= TestEnum.Positive),
        FilterTestCase.Create(1904, FilterOperator.GreaterThanOrEqual, ["Positive"], (TestEnum? x) => x >= TestEnum.Positive),
        FilterTestCase.Create(1905, FilterOperator.GreaterThanOrEqual, ["positive"], (TestEnum? x) => x >= TestEnum.Positive),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new TestEnum?[] { default }, (TestEnum? x) => x == null),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new TestEnum?[] { default }, (TestEnum? x) => x != null)
    ];
    // ReSharper restore RedundantExplicitArrayCreation
}
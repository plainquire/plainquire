using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilter;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestClass, ExcludeFromCodeCoverage]
public class FilterForEnumByValueTests : TestBase<TestEnum>
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
    public void FilterForEnumByValue_WorksAsExpected(object testCase, TestModelFilterFunc<TestEnum> filterFunc)
    {
        switch (testCase)
        {
            case FilterTestCase<TestEnum, TestEnum> enumTestCase:
                enumTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<short, TestEnum> shortTestCase:
                shortTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<int, TestEnum> intTestCase:
                intTestCase.Run(_testItems, filterFunc);
                break;
            case FilterTestCase<string, TestEnum> stringTestCase:
                stringTestCase.Run(_testItems, filterFunc);
                break;
            default:
                throw new InvalidOperationException("Unsupported test case");
        }
    }

    private static readonly TestModel<TestEnum>[] _testItems = {
        new() { ValueA = TestEnum.Negative },
        new() { ValueA = TestEnum.Neutral },
        new() { ValueA = TestEnum.Positive },
        new() { ValueA = TestEnum.Positive2 },
        new() { ValueA = TestEnum.Positive4 },
    };

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly object[] _testCases = {
        FilterTestCase.Create(1100, FilterOperator.Default, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x == TestEnum.Negative),
        FilterTestCase.Create(1101, FilterOperator.Default, new int[] { -10 }, (TestEnum _) => NONE),
        FilterTestCase.Create(1102, FilterOperator.Default, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1103, FilterOperator.Default, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1104, FilterOperator.Default, new string[] { "Positive" }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1105, FilterOperator.Default, new string[] { "positive" }, (TestEnum x) => x == TestEnum.Positive),

        FilterTestCase.Create(1200, FilterOperator.Contains, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x == TestEnum.Negative),
        FilterTestCase.Create(1201, FilterOperator.Contains, new int[] { -10 }, (TestEnum _) => NONE),
        FilterTestCase.Create(1202, FilterOperator.Contains, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1203, FilterOperator.Contains, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1204, FilterOperator.Contains, new string[] { "Positive" }, (TestEnum x) => x == TestEnum.Positive || x == TestEnum.Positive2 || x == TestEnum.Positive4),
        FilterTestCase.Create(1205, FilterOperator.Contains, new string[] { "positive" }, (TestEnum x) => x == TestEnum.Positive || x == TestEnum.Positive2 || x == TestEnum.Positive4),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x == TestEnum.Negative),
        FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new int[] { -10 }, (TestEnum _) => NONE),
        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1304, FilterOperator.EqualCaseInsensitive, new string[] { "Positive" }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1305, FilterOperator.EqualCaseInsensitive, new string[] { "positive" }, (TestEnum x) => x == TestEnum.Positive),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x == TestEnum.Negative),
        FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new int[] { -10 }, (TestEnum _) => NONE),
        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1404, FilterOperator.EqualCaseSensitive, new string[] { "Positive" }, (TestEnum x) => x == TestEnum.Positive),
        FilterTestCase.Create(1405, FilterOperator.EqualCaseSensitive, new string[] { "positive" }, (TestEnum _) => NONE),

        FilterTestCase.Create(1500, FilterOperator.NotEqual, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x != TestEnum.Negative),
        FilterTestCase.Create(1501, FilterOperator.NotEqual, new int[] { -10 }, (TestEnum _) => ALL),
        FilterTestCase.Create(1502, FilterOperator.NotEqual, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x != TestEnum.Positive),
        FilterTestCase.Create(1503, FilterOperator.NotEqual, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x != TestEnum.Positive),
        FilterTestCase.Create(1504, FilterOperator.NotEqual, new string[] { "Positive" }, (TestEnum x) => x != TestEnum.Positive),
        FilterTestCase.Create(1505, FilterOperator.NotEqual, new string[] { "positive" }, (TestEnum x) => x != TestEnum.Positive),

        FilterTestCase.Create(1600, FilterOperator.LessThan, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x < TestEnum.Negative),
        FilterTestCase.Create(1601, FilterOperator.LessThan, new int[] { -10 }, (TestEnum _) => NONE),
        FilterTestCase.Create(1602, FilterOperator.LessThan, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x < TestEnum.Positive),
        FilterTestCase.Create(1603, FilterOperator.LessThan, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x < TestEnum.Positive),
        FilterTestCase.Create(1604, FilterOperator.LessThan, new string[] { "Positive" }, (TestEnum x) => x < TestEnum.Positive),
        FilterTestCase.Create(1605, FilterOperator.LessThan, new string[] { "positive" }, (TestEnum x) => x < TestEnum.Positive),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x <= TestEnum.Negative),
        FilterTestCase.Create(1701, FilterOperator.LessThanOrEqual, new int[] { -10 }, (TestEnum _) => NONE),
        FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x <= TestEnum.Positive),
        FilterTestCase.Create(1703, FilterOperator.LessThanOrEqual, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x <= TestEnum.Positive),
        FilterTestCase.Create(1704, FilterOperator.LessThanOrEqual, new string[] { "Positive" }, (TestEnum x) => x <= TestEnum.Positive),
        FilterTestCase.Create(1705, FilterOperator.LessThanOrEqual, new string[] { "positive" }, (TestEnum x) => x <= TestEnum.Positive),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x > TestEnum.Negative),
        FilterTestCase.Create(1801, FilterOperator.GreaterThan, new int[] { -10 }, (TestEnum _) => ALL),
        FilterTestCase.Create(1802, FilterOperator.GreaterThan, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x > TestEnum.Positive),
        FilterTestCase.Create(1803, FilterOperator.GreaterThan, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x > TestEnum.Positive),
        FilterTestCase.Create(1804, FilterOperator.GreaterThan, new string[] { "Positive" }, (TestEnum x) => x > TestEnum.Positive),
        FilterTestCase.Create(1805, FilterOperator.GreaterThan, new string[] { "positive" }, (TestEnum x) => x > TestEnum.Positive),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new TestEnum[] { TestEnum.Negative }, (TestEnum x) => x >= TestEnum.Negative),
        FilterTestCase.Create(1901, FilterOperator.GreaterThanOrEqual, new int[] { -10 }, (TestEnum _) => ALL),
        FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, new int[] { (int)TestEnum.Positive }, (TestEnum x) => x >= TestEnum.Positive),
        FilterTestCase.Create(1903, FilterOperator.GreaterThanOrEqual, new short[] { (short)TestEnum.Positive }, (TestEnum x) => x >= TestEnum.Positive),
        FilterTestCase.Create(1904, FilterOperator.GreaterThanOrEqual, new string[] { "Positive" }, (TestEnum x) => x >= TestEnum.Positive),
        FilterTestCase.Create(1905, FilterOperator.GreaterThanOrEqual, new string[] { "positive" }, (TestEnum x) => x >= TestEnum.Positive),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new TestEnum[] { default }, new FilterExpressionCreationException("Filter operator 'IsNull' not allowed for property type 'FS.FilterExpressionCreator.Tests.Models.TestEnum'")),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new TestEnum[] { default }, new FilterExpressionCreationException("Filter operator 'NotNull' not allowed for property type 'FS.FilterExpressionCreator.Tests.Models.TestEnum'")),
    };
    // ReSharper restore RedundantExplicitArrayCreation
}
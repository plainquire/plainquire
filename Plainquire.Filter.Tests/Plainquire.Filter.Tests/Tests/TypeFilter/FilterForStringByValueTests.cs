using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForStringByValueTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForStringByValue_WorksAsExpected(FilterTestCase<string, string> testCase, EntityFilterFunc<TestModel<string>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<string>[] _testItems =
    [
        new() { ValueA = null },
        new() { ValueA = "" },
        new() { ValueA = "Hello" },
        new() { ValueA = "HelloWorld" }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    // ReSharper disable ReplaceWithStringIsNullOrEmpty
    private static readonly FilterTestCase<string?, string?>[] _testCases =
    [
        FilterTestCase.Create(1100, FilterOperator.Default, [""], (string? x) => x is "" or "Hello" or "HelloWorld"),
        FilterTestCase.Create(1101, FilterOperator.Default, ["Hello"], (string? x) => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create(1102, FilterOperator.Default, ["hello"], (string? x) => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create(1103, FilterOperator.Default, ["HelloWorld"], (string? x) => x == "HelloWorld"),

        FilterTestCase.Create(1200, FilterOperator.Contains, [""], (string? x) => x is "" or "Hello" or "HelloWorld"),
        FilterTestCase.Create(1201, FilterOperator.Contains, ["Hello"], (string? x) => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create(1202, FilterOperator.Contains, ["hello"], (string? x) => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create(1203, FilterOperator.Contains, ["HelloWorld"], (string? x) => x == "HelloWorld"),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, [""], (string? x) => x == ""),
        FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, ["Hello"], (string? x) => x == "Hello"),
        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, ["hello"], (string? x) => x == "Hello"),
        FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, ["HelloWorld"], (string? x) => x == "HelloWorld"),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, [""], (string? x) => x == ""),
        FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, ["Hello"], (string? x) => x == "Hello"),
        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, ["hello"], (string? _) => TestItems.NONE),
        FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, ["HelloWorld"], (string? x) => x == "HelloWorld"),

        FilterTestCase.Create(1500, FilterOperator.NotEqual, [""], (string? x) => x == null),
        FilterTestCase.Create(1501, FilterOperator.NotEqual, ["Hello"], (string? x) => x is null or ""),
        FilterTestCase.Create(1502, FilterOperator.NotEqual, ["hello"], (string? x) => x is null or ""),
        FilterTestCase.Create(1503, FilterOperator.NotEqual, ["HelloWorld"], (string? x) => x != "HelloWorld"),

        FilterTestCase.Create(1600, FilterOperator.LessThan, [string.Empty], new FilterExpressionException("Filter operator 'LessThan' not allowed for property type 'System.String'")),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, [string.Empty], new FilterExpressionException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.String'")),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, [string.Empty], new FilterExpressionException("Filter operator 'GreaterThan' not allowed for property type 'System.String'")),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, [string.Empty], new FilterExpressionException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.String'")),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new string?[] { default }, (string? x) => x == null),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new string?[] { default }, (string? x) => x != null)
    ];
    // ReSharper restore ReplaceWithStringIsNullOrEmpty
    // ReSharper restore RedundantExplicitArrayCreation
}
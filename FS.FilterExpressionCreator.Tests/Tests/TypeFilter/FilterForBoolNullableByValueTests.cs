using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForBoolNullableByValueTests : TestBase<bool?>
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
    public void FilterForBoolNullableByValue_WorksAsExpected(FilterTestCase<bool?, bool?> testCase, TestModelFilterFunc<bool?> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<bool?>[] _testItems =
    [
        new() { ValueA = true },
        new() { ValueA = false },
        new() { ValueA = null }
    ];

    // ReSharper disable RedundantExplicitArrayCreation
    private static readonly FilterTestCase<bool?, bool?>[] _testCases =
    [
        FilterTestCase.Create(1100, FilterOperator.Default, new bool?[] { true }, (bool? x) => x == true),
        FilterTestCase.Create(1101, FilterOperator.Default, new bool?[] { false }, (bool? x) => x == false),
        FilterTestCase.Create(1102, FilterOperator.Default, new bool?[] { true, false }, (bool? x) => x is true or false),

        FilterTestCase.Create(1200, FilterOperator.Contains, new bool?[] { false }, new FilterExpressionCreationException("Filter operator 'Contains' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new bool?[] { true }, (bool? x) => x == true),
        FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new bool?[] { false }, (bool? x) => x == false),
        FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new bool?[] { true, false }, (bool? x) => x is true or false),

        FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new bool?[] { true }, (bool? x) => x == true),
        FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new bool?[] { false }, (bool? x) => x == false),
        FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new bool?[] { true, false }, (bool? x) => x is true or false),

        FilterTestCase.Create(1502, FilterOperator.NotEqual, new bool?[] { true }, (bool? x) => x != true),
        FilterTestCase.Create(1503, FilterOperator.NotEqual, new bool?[] { false }, (bool? x) => x != false),
        FilterTestCase.Create(1504, FilterOperator.NotEqual, new bool?[] { true, false }, (bool? _) => true),

        FilterTestCase.Create(1600, FilterOperator.LessThan, new bool?[] { false }, new FilterExpressionCreationException("Filter operator 'LessThan' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new bool?[] { false }, new FilterExpressionCreationException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create(1800, FilterOperator.GreaterThan, new bool?[] { false }, new FilterExpressionCreationException("Filter operator 'GreaterThan' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new bool?[] { false }, new FilterExpressionCreationException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create(2000, FilterOperator.IsNull, new bool?[] { default }, (bool? x) => x == null),

        FilterTestCase.Create(2100, FilterOperator.NotNull, new bool?[] { default }, (bool? x) => x != null)
    ];
    // ReSharper restore RedundantExplicitArrayCreation
}
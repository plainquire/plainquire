using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilterTests
{
    [TestClass]
    public class FilterForShortNullableByValueTests : TestBase<short?>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForShortNullableByValue_WorksAsExpected(object testCase, TestModelFilterFunc<short?> filterFunc)
        {
            switch (testCase)
            {
                case FilterTestCase<short?, short?> shortTestCase:
                    shortTestCase.Run(_testItems, filterFunc);
                    break;
                case FilterTestCase<float?, short?> floatTestCase:
                    floatTestCase.Run(_testItems, filterFunc);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported test case");
            }
        }

        private static readonly TestModel<short?>[] _testItems = {
            new() { ValueA = -9 },
            new() { ValueA = -5 },
            new() { ValueA = -0 },
            new() { ValueA = +5 },
            new() { ValueA = +9 },
        };

        // ReSharper disable RedundantExplicitArrayCreation
        // ReSharper disable CompareOfFloatsByEqualityOperator
        private static readonly object[] _testCases = {
            FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, new short?[] { -9 }, (short? x) => x == -9f),

            FilterTestCase.Create(1100, FilterOperator.Default, new short?[] { -5 }, (short? x) => x == -5),
            FilterTestCase.Create(1101, FilterOperator.Default, new short?[] { -10 }, (short? _) => NONE),
            FilterTestCase.Create(1102, FilterOperator.Default, new short?[] { +5 }, (short? x) => x == +5),

            FilterTestCase.Create(1200, FilterOperator.Contains, new short?[] { 0 }, new FilterExpressionCreationException("Filter operator 'Contains' not allowed for property type 'System.Nullable`1[System.Int16]'")),

            FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new short?[] { -5 }, (short? x) => x == -5),
            FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new short?[] { -10 }, (short? _) => NONE),
            FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new short?[] { +5 }, (short? x) => x == +5),

            FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new short?[] { -5 }, (short? x) => x == -5),
            FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new short?[] { -10 }, (short? _) => NONE),
            FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new short?[] { +5 }, (short? x) => x == +5),

            FilterTestCase.Create(1500, FilterOperator.NotEqual, new short?[] { -5 }, (short? x) => x != -5),
            FilterTestCase.Create(1501, FilterOperator.NotEqual, new short?[] { -10 }, (short? _) => ALL),
            FilterTestCase.Create(1502, FilterOperator.NotEqual, new short?[] { +5 }, (short? x) => x != +5),

            FilterTestCase.Create(1600, FilterOperator.LessThan, new short?[] { -5 }, (short? x) => x < -5),
            FilterTestCase.Create(1601, FilterOperator.LessThan, new short?[] { -10 }, (short? _) => NONE),
            FilterTestCase.Create(1602, FilterOperator.LessThan, new short?[] { +5 }, (short? x) => x < +5),

            FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new short?[] { -5 }, (short? x) => x <= -5),
            FilterTestCase.Create(1701, FilterOperator.LessThanOrEqual, new short?[] { -10 }, (short? _) => NONE),
            FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, new short?[] { +5 }, (short? x) => x <= +5),

            FilterTestCase.Create(1800, FilterOperator.GreaterThan, new short?[] { -5 }, (short? x) => x > -5),
            FilterTestCase.Create(1801, FilterOperator.GreaterThan, new short?[] { -10 }, (short? _) => ALL),
            FilterTestCase.Create(1802, FilterOperator.GreaterThan, new short?[] { +5 }, (short? x) => x > +5),

            FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new short?[] { -5 }, (short? x) => x >= -5),
            FilterTestCase.Create(1901, FilterOperator.GreaterThanOrEqual, new short?[] { -10 }, (short? _) => ALL),
            FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, new short?[] { +5 }, (short? x) => x >= +5),

            FilterTestCase.Create(2000, FilterOperator.IsNull, (short?[])null, (short? x) => x == null),

            FilterTestCase.Create(2100, FilterOperator.NotNull, (short?[])null, (short? x) => x != null),
        };
        // ReSharper restore CompareOfFloatsByEqualityOperator
        // ReSharper restore RedundantExplicitArrayCreation
    }
}

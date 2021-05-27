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
    public class FilterForFloatNullableByValueTests : TestBase<float?>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForFloatNullableByValue_WorksAsExpected(object testCase, TestModelFilterFunc<float?> filterFunc)
        {
            switch (testCase)
            {
                case FilterTestCase<float?, float?> floatTestCase:
                    floatTestCase.Run(_testItems, filterFunc);
                    break;
                case FilterTestCase<int?, float?> intTestCase:
                    intTestCase.Run(_testItems, filterFunc);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported test case");
            }
        }

        private static readonly TestModel<float?>[] _testItems = {
            new() { ValueA = -9f },
            new() { ValueA = -5.5f },
            new() { ValueA = -0f },
            new() { ValueA = +5.5f },
            new() { ValueA = +9f },
        };

        // ReSharper disable RedundantExplicitArrayCreation
        // ReSharper disable CompareOfFloatsByEqualityOperator
        private static readonly object[] _testCases = {
            FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, new int?[] { -9 }, (float? x) => x == -9f),

            FilterTestCase.Create(1100, FilterOperator.Default, new float?[] { -5.5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1101, FilterOperator.Default, new float?[] { -10 }, (float? _) => NONE),
            FilterTestCase.Create(1102, FilterOperator.Default, new float?[] { +5.5f }, (float? x) => x == +5.5f),

            FilterTestCase.Create(1200, FilterOperator.Contains, new float?[] { 0 }, new FilterExpressionCreationException("Filter operator 'Contains' not allowed for property type 'System.Nullable`1[System.Single]'")),

            FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new float?[] { -5.5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new float?[] { -10 }, (float? _) => NONE),
            FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new float?[] { +5.5f }, (float? x) => x == +5.5f),

            FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new float?[] { -5.5f }, (float? x) => x == -5.5f),
            FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new float?[] { -10 }, (float? _) => NONE),
            FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new float?[] { +5.5f }, (float? x) => x == +5.5f),

            FilterTestCase.Create(1500, FilterOperator.NotEqual, new float?[] { -5.5f }, (float? x) => x != -5.5f),
            FilterTestCase.Create(1501, FilterOperator.NotEqual, new float?[] { -10 }, (float? _) => ALL),
            FilterTestCase.Create(1502, FilterOperator.NotEqual, new float?[] { +5.5f }, (float? x) => x != +5.5f),

            FilterTestCase.Create(1600, FilterOperator.LessThan, new float?[] { -5.5f }, (float? x) => x < -5.5f),
            FilterTestCase.Create(1601, FilterOperator.LessThan, new float?[] { -10 }, (float? _) => NONE),
            FilterTestCase.Create(1602, FilterOperator.LessThan, new float?[] { +5.5f }, (float? x) => x < +5.5f),

            FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new float?[] { -5.5f }, (float? x) => x <= -5.5f),
            FilterTestCase.Create(1701, FilterOperator.LessThanOrEqual, new float?[] { -10 }, (float? _) => NONE),
            FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, new float?[] { +5.5f }, (float? x) => x <= +5.5f),

            FilterTestCase.Create(1800, FilterOperator.GreaterThan, new float?[] { -5.5f }, (float? x) => x > -5.5f),
            FilterTestCase.Create(1801, FilterOperator.GreaterThan, new float?[] { -10 }, (float? _) => ALL),
            FilterTestCase.Create(1802, FilterOperator.GreaterThan, new float?[] { +5.5f }, (float? x) => x > +5.5f),

            FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new float?[] { -5.5f }, (float? x) => x >= -5.5f),
            FilterTestCase.Create(1901, FilterOperator.GreaterThanOrEqual, new float?[] { -10 }, (float? _) => ALL),
            FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, new float?[] { +5.5f }, (float? x) => x >= +5.5f),

            FilterTestCase.Create(2000, FilterOperator.IsNull, (float?[])null, (float? x) => x == null),

            FilterTestCase.Create(2100, FilterOperator.NotNull, (float?[])null, (float? x) => x != null),
        };
        // ReSharper restore CompareOfFloatsByEqualityOperator
        // ReSharper restore RedundantExplicitArrayCreation
    }
}

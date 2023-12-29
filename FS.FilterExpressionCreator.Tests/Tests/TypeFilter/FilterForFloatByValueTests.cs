using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilter
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [TestClass, ExcludeFromCodeCoverage]
    public class FilterForFloatByValueTests : TestBase<float>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForFloatByValue_WorksAsExpected(object testCase, TestModelFilterFunc<float> filterFunc)
        {
            switch (testCase)
            {
                case FilterTestCase<float, float> floatTestCase:
                    floatTestCase.Run(_testItems, filterFunc);
                    break;
                case FilterTestCase<int, float> intTestCase:
                    intTestCase.Run(_testItems, filterFunc);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported test case");
            }
        }

        private static readonly TestModel<float>[] _testItems = {
            new() { ValueA = -9f },
            new() { ValueA = -5.5f },
            new() { ValueA = -0f },
            new() { ValueA = +5.5f },
            new() { ValueA = +9f },
        };

        // ReSharper disable RedundantExplicitArrayCreation
        // ReSharper disable CompareOfFloatsByEqualityOperator
        private static readonly object[] _testCases = {
            FilterTestCase.Create(1000, FilterOperator.EqualCaseInsensitive, new int[] { -9 }, (float x) => x == -9f),

            FilterTestCase.Create(1102, FilterOperator.Default, new float[] { -5.5f }, (float x) => x == -5.5f),
            FilterTestCase.Create(1103, FilterOperator.Default, new float[] { -10 }, (float _) => NONE),
            FilterTestCase.Create(1104, FilterOperator.Default, new float[] { +5.5f }, (float x) => x == +5.5f),

            FilterTestCase.Create(1200, FilterOperator.Contains, new float[] { +5f }, (float x) => x == +5.5f || x == -5.5f),
            FilterTestCase.Create(1201, FilterOperator.Contains, new float[] { -5f }, (float x) => x == -5.5f),
            FilterTestCase.Create(1202, FilterOperator.Contains, new float[] { +3f }, (float _) => NONE),
            FilterTestCase.Create(1203, FilterOperator.Contains, new float[] { +0f }, (float x) => x == 0),

            FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new float[] { -5.5f }, (float x) => x == -5.5f),
            FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, new float[] { -10 }, (float _) => NONE),
            FilterTestCase.Create(1304, FilterOperator.EqualCaseInsensitive, new float[] { +5.5f }, (float x) => x == +5.5f),

            FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new float[] { -5.5f }, (float x) => x == -5.5f),
            FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, new float[] { -10 }, (float _) => NONE),
            FilterTestCase.Create(1404, FilterOperator.EqualCaseSensitive, new float[] { +5.5f }, (float x) => x == +5.5f),

            FilterTestCase.Create(1502, FilterOperator.NotEqual, new float[] { -5.5f }, (float x) => x != -5.5f),
            FilterTestCase.Create(1503, FilterOperator.NotEqual, new float[] { -10 }, (float _) => ALL),
            FilterTestCase.Create(1504, FilterOperator.NotEqual, new float[] { +5.5f }, (float x) => x != +5.5f),

            FilterTestCase.Create(1602, FilterOperator.LessThan, new float[] { -5.5f }, (float x) => x < -5.5f),
            FilterTestCase.Create(1603, FilterOperator.LessThan, new float[] { -10 }, (float _) => NONE),
            FilterTestCase.Create(1604, FilterOperator.LessThan, new float[] { +5.5f }, (float x) => x < +5.5f),

            FilterTestCase.Create(1702, FilterOperator.LessThanOrEqual, new float[] { -5.5f }, (float x) => x <= -5.5f),
            FilterTestCase.Create(1703, FilterOperator.LessThanOrEqual, new float[] { -10 }, (float _) => NONE),
            FilterTestCase.Create(1704, FilterOperator.LessThanOrEqual, new float[] { +5.5f }, (float x) => x <= +5.5f),

            FilterTestCase.Create(1802, FilterOperator.GreaterThan, new float[] { -5.5f }, (float x) => x > -5.5f),
            FilterTestCase.Create(1803, FilterOperator.GreaterThan, new float[] { -10 }, (float _) => ALL),
            FilterTestCase.Create(1804, FilterOperator.GreaterThan, new float[] { +5.5f }, (float x) => x > +5.5f),

            FilterTestCase.Create(1902, FilterOperator.GreaterThanOrEqual, new float[] { -5.5f }, (float x) => x >= -5.5f),
            FilterTestCase.Create(1903, FilterOperator.GreaterThanOrEqual, new float[] { -10 }, (float _) => ALL),
            FilterTestCase.Create(1904, FilterOperator.GreaterThanOrEqual, new float[] { +5.5f }, (float x) => x >= +5.5f),

            FilterTestCase.Create(2000, FilterOperator.IsNull, new float[] { default }, new FilterExpressionCreationException("Filter operator 'IsNull' not allowed for property type 'System.Single'")),

            FilterTestCase.Create(2100, FilterOperator.NotNull, new float[] { default }, new FilterExpressionCreationException("Filter operator 'NotNull' not allowed for property type 'System.Single'")),
        };
        // ReSharper restore CompareOfFloatsByEqualityOperator
        // ReSharper restore RedundantExplicitArrayCreation
    }
}

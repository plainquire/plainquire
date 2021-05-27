using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilterTests
{
    [TestClass]
    public class FilterForStringByValueTests : TestBase<string>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForStringByValue_WorksAsExpected(FilterTestCase<string, string> testCase, TestModelFilterFunc<string> filterFunc)
            => testCase.Run(_testItems, filterFunc);

        private static readonly TestModel<string>[] _testItems = {
            new() { ValueA = null },
            new() { ValueA = "" },
            new() { ValueA = "Hello" },
            new() { ValueA = "HelloWorld" },
        };

        // ReSharper disable RedundantExplicitArrayCreation
        // ReSharper disable ReplaceWithStringIsNullOrEmpty
        private static readonly FilterTestCase<string, string>[] _testCases = {
            FilterTestCase.Create(1100, FilterOperator.Default, new string[] { "" }, (string x) => x == "" || x == "Hello" || x == "HelloWorld"),
            FilterTestCase.Create(1101, FilterOperator.Default, new string[] { "Hello" }, (string x) => x == "Hello" || x == "HelloWorld"),
            FilterTestCase.Create(1102, FilterOperator.Default, new string[] { "hello" }, (string x) => x == "Hello" || x == "HelloWorld"),
            FilterTestCase.Create(1103, FilterOperator.Default, new string[] { "HelloWorld" }, (string x) => x == "HelloWorld"),

            FilterTestCase.Create(1200, FilterOperator.Contains, new string[] { "" }, (string x) => x == "" || x == "Hello" || x == "HelloWorld"),
            FilterTestCase.Create(1201, FilterOperator.Contains, new string[] { "Hello" }, (string x) => x == "Hello" || x == "HelloWorld"),
            FilterTestCase.Create(1202, FilterOperator.Contains, new string[] { "hello" }, (string x) => x == "Hello" || x == "HelloWorld"),
            FilterTestCase.Create(1203, FilterOperator.Contains, new string[] { "HelloWorld" }, (string x) => x == "HelloWorld"),

            FilterTestCase.Create(1300, FilterOperator.EqualCaseInsensitive, new string[] { "" }, (string x) => x == ""),
            FilterTestCase.Create(1301, FilterOperator.EqualCaseInsensitive, new string[] { "Hello" }, (string x) => x == "Hello"),
            FilterTestCase.Create(1302, FilterOperator.EqualCaseInsensitive, new string[] { "hello" }, (string x) => x == "Hello"),
            FilterTestCase.Create(1303, FilterOperator.EqualCaseInsensitive, new string[] { "HelloWorld" }, (string x) => x == "HelloWorld"),

            FilterTestCase.Create(1400, FilterOperator.EqualCaseSensitive, new string[] { "" }, (string x) => x == ""),
            FilterTestCase.Create(1401, FilterOperator.EqualCaseSensitive, new string[] { "Hello" }, (string x) => x == "Hello"),
            FilterTestCase.Create(1402, FilterOperator.EqualCaseSensitive, new string[] { "hello" }, (string _) => NONE),
            FilterTestCase.Create(1403, FilterOperator.EqualCaseSensitive, new string[] { "HelloWorld" }, (string x) => x == "HelloWorld"),

            FilterTestCase.Create(1500, FilterOperator.NotEqual, new string[] { "" }, (string x) => x == null),
            FilterTestCase.Create(1501, FilterOperator.NotEqual, new string[] { "Hello" }, (string x) => x == null || x == ""),
            FilterTestCase.Create(1502, FilterOperator.NotEqual, new string[] { "hello" }, (string x) => x == null || x == ""),
            FilterTestCase.Create(1503, FilterOperator.NotEqual, new string[] { "HelloWorld" }, (string x) => x != "HelloWorld"),

            FilterTestCase.Create(1600, FilterOperator.LessThan, new string[] { string.Empty }, new FilterExpressionCreationException("Filter operator 'LessThan' not allowed for property type 'System.String'")),

            FilterTestCase.Create(1700, FilterOperator.LessThanOrEqual, new string[] { string.Empty }, new FilterExpressionCreationException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.String'")),

            FilterTestCase.Create(1800, FilterOperator.GreaterThan, new string[] { string.Empty }, new FilterExpressionCreationException("Filter operator 'GreaterThan' not allowed for property type 'System.String'")),

            FilterTestCase.Create(1900, FilterOperator.GreaterThanOrEqual, new string[] { string.Empty }, new FilterExpressionCreationException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.String'")),

            FilterTestCase.Create(2000, FilterOperator.IsNull, (string[])null, (string x) => x == null),

            FilterTestCase.Create(2100, FilterOperator.NotNull, (string[])null, (string x) => x != null),
        };
        // ReSharper restore ReplaceWithStringIsNullOrEmpty
        // ReSharper restore RedundantExplicitArrayCreation
    }
}

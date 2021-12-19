using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilterTests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class FilterForBoolBySyntaxTests : TestBase<bool>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForBoolBySyntax_WorksAsExpected(FilterTestCase<bool, bool> testCase, TestModelFilterFunc<bool> filterFunc)
            => testCase.Run(_testItems, filterFunc);

        private static readonly TestModel<bool>[] _testItems =
        {
            new() { ValueA = true },
            new() { ValueA = false },
        };

        private static readonly FilterTestCase<bool, bool>[] _testCases = {
            // ReSharper disable RedundantBoolCompare
            FilterTestCase.Create<bool>(1000, "null", _ => ALL, IgnoreParseExceptions),
            FilterTestCase.Create<bool>(1001, "=null", _ => ALL, IgnoreParseExceptions),
            FilterTestCase.Create<bool>(1002, "TRUE", x => x == true),
            FilterTestCase.Create<bool>(1003, "FALSE", x => x == false),
            FilterTestCase.Create<bool>(1004, "yes", x => x == true),
            FilterTestCase.Create<bool>(1005, "no", x => x == false),
            FilterTestCase.Create<bool>(1004, "ja", x => x == true, CultureDeDe),
            // ReSharper disable once StringLiteralTypo
            FilterTestCase.Create<bool>(1005, "nein", x => x == false, CultureDeDe),
            FilterTestCase.Create<bool>(1006, "YES", x => x == true),
            FilterTestCase.Create<bool>(1007, "NO", x => x == false),
            FilterTestCase.Create<bool>(1008, "1", x => x == true),
            FilterTestCase.Create<bool>(1009, "0", x => x == false),
            // ReSharper restore RedundantBoolCompare

            FilterTestCase.Create<bool>(1100, "null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1101, "", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1102, "true", x => x),
            FilterTestCase.Create<bool>(1103, "false", x => !x),
            FilterTestCase.Create<bool>(1104, "true, false", _ => ALL),

            FilterTestCase.Create<bool>(1200, "~null", new FilterExpressionCreationException("Filter operator 'Contains' not allowed for property type 'System.Boolean'")),

            FilterTestCase.Create<bool>(1300, "=null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1301, "=", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1302, "=true", x => x),
            FilterTestCase.Create<bool>(1303, "=false", x => !x),
            FilterTestCase.Create<bool>(1304, "=true, false", _ => ALL),

            FilterTestCase.Create<bool>(1400, "==null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1401, "==", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1402, "==true", x => x),
            FilterTestCase.Create<bool>(1403, "==false", x => !x),
            FilterTestCase.Create<bool>(1404, "==true, false", _ => ALL),

            FilterTestCase.Create<bool>(1500, "!null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1501, "!", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<bool>(1502, "!true", x => !x),
            FilterTestCase.Create<bool>(1503, "!false", x => x),
            FilterTestCase.Create<bool>(1504, "!true, !false", _ => ALL),

            FilterTestCase.Create<bool>(1600, "<null", new FilterExpressionCreationException("Filter operator 'LessThan' not allowed for property type 'System.Boolean'")),

            FilterTestCase.Create<bool>(1700, "<=null", new FilterExpressionCreationException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Boolean'")),

            FilterTestCase.Create<bool>(1800, ">null", new FilterExpressionCreationException("Filter operator 'GreaterThan' not allowed for property type 'System.Boolean'")),

            FilterTestCase.Create<bool>(1900, ">=null", new FilterExpressionCreationException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Boolean'")),

            FilterTestCase.Create<bool>(2000, "ISNULL", new FilterExpressionCreationException("Filter operator 'IsNull' not allowed for property type 'System.Boolean'")),

            FilterTestCase.Create<bool>(2100, "NOTNULL", new FilterExpressionCreationException("Filter operator 'NotNull' not allowed for property type 'System.Boolean'")),
        };
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Filter.Exceptions;
using Schick.Plainquire.Filter.Tests.Extensions;
using Schick.Plainquire.Filter.Tests.Models;
using Schick.Plainquire.Filter.Tests.Services;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForBoolNullableBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForBoolNullableBySyntax_WorksAsExpected(FilterTestCase<bool?, bool?> testCase, EntityFilterFunc<TestModel<bool?>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<bool?>[] _testItems =
    [
        new() { ValueA = true },
        new() { ValueA = false },
        new() { ValueA = null }
    ];

    private static readonly FilterTestCase<bool?, bool?>[] _testCases =
    [
        FilterTestCase.Create<bool?>(1000, "null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<bool?>(1001, "=null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<bool?>(1002, "TRUE", x => x == true),
        FilterTestCase.Create<bool?>(1003, "FALSE", x => x == false),
        FilterTestCase.Create<bool?>(1004, "yes", x => x == true),
        FilterTestCase.Create<bool?>(1005, "no", x => x == false),
        FilterTestCase.Create<bool?>(1006, "YES", x => x == true),
        FilterTestCase.Create<bool?>(1007, "NO", x => x == false),
        FilterTestCase.Create<bool?>(1008, "1", x => x == true),
        FilterTestCase.Create<bool?>(1009, "0", x => x == false),

        FilterTestCase.Create<bool?>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1102, "true", x => x == true),
        FilterTestCase.Create<bool?>(1103, "false", x => x == false),
        FilterTestCase.Create<bool?>(1104, "true, false", x => x is true or false),

        FilterTestCase.Create<bool?>(1200, "~", new FilterExpressionException("Filter operator 'Contains' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create<bool?>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1302, "=true", x => x == true),
        FilterTestCase.Create<bool?>(1303, "=false", x => x == false),
        FilterTestCase.Create<bool?>(1304, "=true, false", x => x is true or false),

        FilterTestCase.Create<bool?>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1402, "==true", x => x == true),
        FilterTestCase.Create<bool?>(1403, "==false", x => x == false),
        FilterTestCase.Create<bool?>(1404, "==true, false", x => x is true or false),

        FilterTestCase.Create<bool?>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool?>(1502, "!true", x => x != true),
        FilterTestCase.Create<bool?>(1503, "!false", x => x != false),
        FilterTestCase.Create<bool?>(1504, "!true, !false", _ => TestItems.ALL),

        FilterTestCase.Create<bool?>(1600, "<", new FilterExpressionException("Filter operator 'LessThan' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create<bool?>(1700, "<=", new FilterExpressionException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create<bool?>(1800, ">", new FilterExpressionException("Filter operator 'GreaterThan' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create<bool?>(1900, ">=", new FilterExpressionException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Nullable`1[System.Boolean]'")),

        FilterTestCase.Create<bool?>(2000, "ISNULL", x => x == null),

        FilterTestCase.Create<bool?>(2100, "NOTNULL", x => x != null)
    ];
}
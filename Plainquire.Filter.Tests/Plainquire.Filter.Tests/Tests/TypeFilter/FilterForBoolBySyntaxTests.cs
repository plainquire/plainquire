using NUnit.Framework;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using Plainquire.TestSupport.Services;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestFixture]
public class FilterForBoolBySyntaxTests : TestContainer
{
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForBoolBySyntax_WorksAsExpected(FilterTestCase<bool, bool> testCase, EntityFilterFunc<TestModel<bool>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<bool>[] _testItems =
    [
        new() { ValueA = true },
        new() { ValueA = false }
    ];

    private static readonly FilterTestCase<bool, bool>[] _testCases =
    [
        // ReSharper disable RedundantBoolCompare
        FilterTestCase.Create<bool>(1000, "null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<bool>(1001, "=null", _ => TestItems.ALL, TestConfig.IgnoreParseExceptions),
        FilterTestCase.Create<bool>(1002, "TRUE", x => x == true),
        FilterTestCase.Create<bool>(1003, "FALSE", x => x == false),
        FilterTestCase.Create<bool>(1004, "yes", x => x == true),
        FilterTestCase.Create<bool>(1005, "no", x => x == false),
        FilterTestCase.Create<bool>(1006, "ja", x => x == true, TestConfig.FilterCultureDeDe),
        // ReSharper disable once StringLiteralTypo
        FilterTestCase.Create<bool>(1007, "nein", x => x == false, TestConfig.FilterCultureDeDe),
        FilterTestCase.Create<bool>(1008, "YES", x => x == true),
        FilterTestCase.Create<bool>(1009, "NO", x => x == false),
        FilterTestCase.Create<bool>(1010, "1", x => x == true),
        FilterTestCase.Create<bool>(1011, "0", x => x == false),
        // ReSharper restore RedundantBoolCompare

        FilterTestCase.Create<bool>(1100, "null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1101, "", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1102, "true", x => x),
        FilterTestCase.Create<bool>(1103, "false", x => !x),
        FilterTestCase.Create<bool>(1104, "true, false", _ => TestItems.ALL),

        FilterTestCase.Create<bool>(1200, "~null", new FilterExpressionException("Filter operator 'Contains' not allowed for property type 'System.Boolean'")),

        FilterTestCase.Create<bool>(1300, "=null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1301, "=", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1302, "=true", x => x),
        FilterTestCase.Create<bool>(1303, "=false", x => !x),
        FilterTestCase.Create<bool>(1304, "=true, false", _ => TestItems.ALL),

        FilterTestCase.Create<bool>(1400, "==null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1401, "==", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1402, "==true", x => x),
        FilterTestCase.Create<bool>(1403, "==false", x => !x),
        FilterTestCase.Create<bool>(1404, "==true, false", _ => TestItems.ALL),

        FilterTestCase.Create<bool>(1500, "!null", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1501, "!", new FilterExpressionException("Unable to parse given filter value")),
        FilterTestCase.Create<bool>(1502, "!true", x => !x),
        FilterTestCase.Create<bool>(1503, "!false", x => x),
        FilterTestCase.Create<bool>(1504, "!true, !false", _ => TestItems.ALL),

        FilterTestCase.Create<bool>(1600, "<null", new FilterExpressionException("Filter operator 'LessThan' not allowed for property type 'System.Boolean'")),

        FilterTestCase.Create<bool>(1700, "<=null", new FilterExpressionException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Boolean'")),

        FilterTestCase.Create<bool>(1800, ">null", new FilterExpressionException("Filter operator 'GreaterThan' not allowed for property type 'System.Boolean'")),

        FilterTestCase.Create<bool>(1900, ">=null", new FilterExpressionException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Boolean'")),

        FilterTestCase.Create<bool>(2000, "ISNULL", new FilterExpressionException("Filter operator 'IsNull' not allowed for property type 'System.Boolean'")),

        FilterTestCase.Create<bool>(2100, "NOTNULL", new FilterExpressionException("Filter operator 'NotNull' not allowed for property type 'System.Boolean'"))
    ];
}
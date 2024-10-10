using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Tests.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Tests.TypeFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterForStringBySyntaxTests
{
    [DataTestMethod]
    [FilterTestDataSource(nameof(_testCases))]
    public void FilterForStringBySyntax_WorksAsExpected(FilterTestCase<string, string> testCase, EntityFilterFunc<TestModel<string>> filterFunc)
        => testCase.Run(_testItems, filterFunc);

    private static readonly TestModel<string>[] _testItems =
    [
        new() { ValueA = null },
        new() { ValueA = "" },
        new() { ValueA = "Hello" },
        new() { ValueA = "HelloWorld" }
    ];

    // ReSharper disable ReplaceWithStringIsNullOrEmpty
    private static readonly FilterTestCase<string?, string?>[] _testCases =
    [
        FilterTestCase.Create<string>(1101, "", x => x is "" or "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1102, "Hello", x => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1103, "hello", x => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1104, "HelloWorld", x => x == "HelloWorld"),

        FilterTestCase.Create<string>(1201, "~", x => x is "" or "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1202, "~Hello", x => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1203, "~hello", x => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1204, "~HelloWorld", x => x == "HelloWorld"),

        FilterTestCase.Create<string>(1301, "^", x => x != null),
        FilterTestCase.Create<string>(1302, "^Hello", x => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1303, "^hello", x => x is "Hello" or "HelloWorld"),
        FilterTestCase.Create<string>(1304, "^HelloWorld", x => x == "HelloWorld"),
        FilterTestCase.Create<string>(1305, "^World", _ => TestItems.NONE),

        FilterTestCase.Create<string>(1401, "$", x => x != null),
        FilterTestCase.Create<string>(1402, "$World", x => x == "HelloWorld"),
        FilterTestCase.Create<string>(1403, "$world", x => x == "HelloWorld"),
        FilterTestCase.Create<string>(1404, "$HelloWorld", x => x == "HelloWorld"),
        FilterTestCase.Create<string>(1405, "$NA", _ => TestItems.NONE),

        FilterTestCase.Create<string>(1501, "=", x => x == ""),
        FilterTestCase.Create<string>(1502, "=Hello", x => x == "Hello"),
        FilterTestCase.Create<string>(1503, "=hello", x => x == "Hello"),
        FilterTestCase.Create<string>(1504, "=HelloWorld", x => x == "HelloWorld"),

        FilterTestCase.Create<string>(1601, "==", x => x == ""),
        FilterTestCase.Create<string>(1602, "==Hello", x => x == "Hello"),
        FilterTestCase.Create<string>(1603, "==hello", _ => TestItems.NONE),
        FilterTestCase.Create<string>(1604, "==HelloWorld", x => x == "HelloWorld"),

        FilterTestCase.Create<string>(1701, "!", x => x == null),
        FilterTestCase.Create<string>(1702, "!Hello", x => x is null or ""),
        FilterTestCase.Create<string>(1703, "!hello", x => x is null or ""),
        FilterTestCase.Create<string>(1704, "!HelloWorld", x => x != "HelloWorld"),

        FilterTestCase.Create<string>(1800, "<", new FilterExpressionException("Filter operator 'LessThan' not allowed for property type 'System.String'")),

        FilterTestCase.Create<string>(1900, "<=", new FilterExpressionException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.String'")),

        FilterTestCase.Create<string>(2000, ">", new FilterExpressionException("Filter operator 'GreaterThan' not allowed for property type 'System.String'")),

        FilterTestCase.Create<string>(2100, ">=", new FilterExpressionException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.String'")),

        FilterTestCase.Create<string>(2200, "ISNULL", x => x == null),

        FilterTestCase.Create<string>(2300, "NOTNULL", x => x != null)
    ];
    // ReSharper restore ReplaceWithStringIsNullOrEmpty
}
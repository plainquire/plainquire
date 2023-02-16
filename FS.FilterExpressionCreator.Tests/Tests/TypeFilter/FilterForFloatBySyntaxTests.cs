using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilter
{
    [TestClass, ExcludeFromCodeCoverage]
    public class FilterForFloatBySyntaxTests : TestBase<float>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForFloatBySyntax_WorksAsExpected(FilterTestCase<float, float> testCase, TestModelFilterFunc<float> filterFunc)
            => testCase.Run(_testItems, filterFunc);

        private static readonly TestModel<float>[] _testItems = {
            new() { ValueA = -9f },
            new() { ValueA = -5.5f },
            new() { ValueA = -0f },
            new() { ValueA = +5.5f },
            new() { ValueA = +9f },
        };

        // ReSharper disable CompareOfFloatsByEqualityOperator
        private static readonly FilterTestCase<float, float>[] _testCases = {
            FilterTestCase.Create<float>(1000, "-5\\,5", x => x == -5.5f, CultureDeDe),
            FilterTestCase.Create<float>(1001, "-5\\,5,+5\\,5", x => x == -5.5f ||  x == 5.5f, CultureDeDe),
            FilterTestCase.Create<float>(1002, "-5,5,+5,5", _ => NONE, CultureDeDe),

            FilterTestCase.Create<float>(1100, "null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1101, "", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1102, "-5.5", x => x == -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1103, "-10", _ => NONE),
            FilterTestCase.Create<float>(1104, "+5.5", x => x == +5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1105, "-5.5,+5.5", x => x == -5.5f || x == +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1200, "~5", x => x == +5.5f || x == -5.5f),
            FilterTestCase.Create<float>(1201, "~-5", x => x == -5.5f),
            FilterTestCase.Create<float>(1202, "~3", _ => NONE),
            FilterTestCase.Create<float>(1203, "~0", x => x == 0),

            FilterTestCase.Create<float>(1300, "=null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1301, "=", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1302, "=-5.5", x => x == -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1303, "=-10", _ => NONE),
            FilterTestCase.Create<float>(1304, "=+5.5", x => x == +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1400, "==null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1401, "==", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1402, "==-5.5", x => x == -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1403, "==-10", _ => NONE),
            FilterTestCase.Create<float>(1404, "==+5.5", x => x == +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1500, "!null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1501, "!", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1502, "!-5.5", x => x != -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1503, "!-10", _ => ALL),
            FilterTestCase.Create<float>(1504, "!+5.5", x => x != +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1600, "<null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1601, "<", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1602, "<-5.5", x => x < -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1603, "<-10", _ => NONE),
            FilterTestCase.Create<float>(1604, "<+5.5", x => x < +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1700, "<=null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1701, "<=", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1702, "<=-5.5", x => x <= -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1703, "<=-10", _ => NONE),
            FilterTestCase.Create<float>(1704, "<=+5.5", x => x <= +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1800, ">null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1801, ">", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1802, ">-5.5", x => x > -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1803, ">-10", _ => ALL),
            FilterTestCase.Create<float>(1804, ">+5.5", x => x > +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(1900, ">=null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1901, ">=", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<float>(1902, ">=-5.5", x => x >= -5.5f, CultureEnUs),
            FilterTestCase.Create<float>(1903, ">=-10", _ => ALL),
            FilterTestCase.Create<float>(1904, ">=+5.5", x => x >= +5.5f, CultureEnUs),

            FilterTestCase.Create<float>(2000, "ISNULL", new FilterExpressionCreationException("Filter operator 'IsNull' not allowed for property type 'System.Single'")),

            FilterTestCase.Create<float>(2100, "NOTNULL", new FilterExpressionCreationException("Filter operator 'NotNull' not allowed for property type 'System.Single'")),
        };
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }
}

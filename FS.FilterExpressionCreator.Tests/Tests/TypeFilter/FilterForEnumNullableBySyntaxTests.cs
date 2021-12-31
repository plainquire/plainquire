using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.TypeFilter
{
    [TestClass, ExcludeFromCodeCoverage]
    public class FilterForEnumNullableBySyntaxTests : TestBase<TestEnum?>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForEnumNullableBySyntax_WorksAsExpected(FilterTestCase<TestEnum?, TestEnum?> testCase, TestModelFilterFunc<TestEnum?> filterFunc)
            => testCase.Run(_testItems, filterFunc);

        private static readonly TestModel<TestEnum?>[] _testItems = {
            new() { ValueA = TestEnum.Negative },
            new() { ValueA = TestEnum.Neutral },
            new() { ValueA = TestEnum.Positive },
            new() { ValueA = TestEnum.Positive2 },
            new() { ValueA = TestEnum.Positive4 },
        };

        private static readonly FilterTestCase<TestEnum?, TestEnum?>[] _testCases = {
            FilterTestCase.Create<TestEnum?>(1100, "null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1101, "", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1102, "Negative", x => x == TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1103, "-10", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1104, "1", x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1105, "Positive" , x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1106, "positive", x => x == TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(1200, "~null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1201, "~", _ => ALL),
            FilterTestCase.Create<TestEnum?>(1202, "~Negative", x => x == TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1203, "~-10", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1204, "~1", x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1205, "~Positive" , x => x == TestEnum.Positive || x == TestEnum.Positive2 || x == TestEnum.Positive4),
            FilterTestCase.Create<TestEnum?>(1206, "~positive", x => x == TestEnum.Positive || x == TestEnum.Positive2 || x == TestEnum.Positive4),

            FilterTestCase.Create<TestEnum?>(1300, "=null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1301, "=", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1302, "=Negative", x => x == TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1303, "=-10", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1304, "=1", x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1305, "=Positive" , x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1306, "=positive", x => x == TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(1400, "==null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1401, "==", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1402, "==Negative", x => x == TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1403, "==-10", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1404, "==1", x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1405, "==Positive" , x => x == TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1406, "==positive", _ => NONE),

            FilterTestCase.Create<TestEnum?>(1500, "!null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1501, "!", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1502, "!Negative", x => x != TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1503, "!-10", _ => ALL),
            FilterTestCase.Create<TestEnum?>(1504, "!1", x => x != TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1505, "!Positive" , x => x != TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1506, "!positive", x => x != TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(1600, "<null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1601, "<", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1602, "<Negative", x => x < TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1603, "<-10", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1604, "<1", x => x < TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1605, "<Positive" , x => x < TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1606, "<positive", x => x < TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(1700, "<=null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1701, "<=", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1702, "<=Negative", x => x <= TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1703, "<=-10", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1704, "<=1", x => x <= TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1705, "<=Positive" , x => x <= TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1706, "<=positive", x => x <= TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(1800, ">null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1801, ">", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1802, ">Negative", x => x > TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1803, ">-10", _ => ALL),
            FilterTestCase.Create<TestEnum?>(1804, ">1", x => x > TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1805, ">Positive" , x => x > TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1806, ">positive", x => x > TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(1900, ">=null", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1901, ">=", _ => NONE),
            FilterTestCase.Create<TestEnum?>(1902, ">=Negative", x => x >= TestEnum.Negative),
            FilterTestCase.Create<TestEnum?>(1903, ">=-10", _ => ALL),
            FilterTestCase.Create<TestEnum?>(1904, ">=1", x => x >= TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1905, ">=Positive" , x => x >= TestEnum.Positive),
            FilterTestCase.Create<TestEnum?>(1906, ">=positive", x => x >= TestEnum.Positive),

            FilterTestCase.Create<TestEnum?>(2000, "ISNULL", x => x == null),

            FilterTestCase.Create<TestEnum?>(2100, "NOTNULL", x => x != null),
        };
    }
}

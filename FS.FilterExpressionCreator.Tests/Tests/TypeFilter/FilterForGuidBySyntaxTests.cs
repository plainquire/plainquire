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
    public class FilterForGuidBySyntaxTests : TestBase<Guid>
    {
        [DataTestMethod]
        [FilterTestDataSource(nameof(_testCases), nameof(TestModelFilterFunctions))]
        public void FilterForGuidBySyntax_WorksAsExpected(FilterTestCase<Guid, Guid> testCase, TestModelFilterFunc<Guid> filterFunc)
            => testCase.Run(_testItems, filterFunc);

        private static readonly TestModel<Guid>[] _testItems = {
            new() { ValueA = Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab") },
            new() { ValueA = Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2") },
            new() { ValueA = Guid.Parse("6cda682c-c0cc-4d4d-bea3-7058777e98d1") },
        };

        // ReSharper disable RedundantExplicitArrayCreation
        private static readonly FilterTestCase<Guid, Guid>[] _testCases = {
            FilterTestCase.Create<Guid>(1100, "null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1101, "", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1102, "6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
            FilterTestCase.Create<Guid>(1103, "6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

            FilterTestCase.Create<Guid>(1200, "~686c", x => x == Guid.Parse("df72ce74-686c-4c0f-a11f-5c8e50a213ab")),
            FilterTestCase.Create<Guid>(1201, "~6cda682c", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2") || x == Guid.Parse("6cda682c-c0cc-4d4d-bea3-7058777e98d1")),

            FilterTestCase.Create<Guid>(1300, "=null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1301, "=", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1302, "=6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
            FilterTestCase.Create<Guid>(1303, "=6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

            FilterTestCase.Create<Guid>(1400, "==null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1401, "==", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1402, "==6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
            FilterTestCase.Create<Guid>(1403, "==6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x == Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

            FilterTestCase.Create<Guid>(1500, "!null", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1501, "!", new FilterExpressionCreationException("Unable to parse given filter value")),
            FilterTestCase.Create<Guid>(1502, "!6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2", x => x != Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),
            FilterTestCase.Create<Guid>(1503, "!6CDA682C-E7FF-43E8-B4D9-F8B27A7D62F2", x => x != Guid.Parse("6cda682c-e7ff-43e8-b4d9-f8b27a7d62f2")),

            FilterTestCase.Create<Guid>(1600, "<", new FilterExpressionCreationException("Filter operator 'LessThan' not allowed for property type 'System.Guid'")),

            FilterTestCase.Create<Guid>(1700, "<=", new FilterExpressionCreationException("Filter operator 'LessThanOrEqual' not allowed for property type 'System.Guid'")),

            FilterTestCase.Create<Guid>(1800, ">", new FilterExpressionCreationException("Filter operator 'GreaterThan' not allowed for property type 'System.Guid'")),

            FilterTestCase.Create<Guid>(1900, ">=", new FilterExpressionCreationException("Filter operator 'GreaterThanOrEqual' not allowed for property type 'System.Guid'")),

            FilterTestCase.Create<Guid>(2000, "ISNULL", new FilterExpressionCreationException("Filter operator 'IsNull' not allowed for property type 'System.Guid'")),

            FilterTestCase.Create<Guid>(2100, "NOTNULL", new FilterExpressionCreationException("Filter operator 'NotNull' not allowed for property type 'System.Guid'")),
        };
        // ReSharper restore RedundantExplicitArrayCreation
    }
}

using FluentAssertions;
using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilter
{
    [TestClass, ExcludeFromCodeCoverage]
    public class EntityFilterExtensionsTests
    {
        [TestMethod]
        public void WhenFilterAddedViaSyntax_FilterIsChanged()
        {
            var filter = new EntityFilter<TestModel<int>>()
                .Add(x => x.ValueA, "1")
                .Add(x => x.ValueA, "2");
            filter.ToString().Should().Be("x => ((x.ValueA == 1) AndAlso (x.ValueA == 2))");
        }

        [TestMethod]
        public void WhenNullFilterSyntaxIsAdded_FilterIsReturnedUnchanged()
        {
            var filter = new EntityFilter<TestModel<bool>>()
                .Add(x => x.ValueA, "1")
                .Add(x => x.ValueA, (string)null);
            filter.ToString().Should().Be("x => (x.ValueA == True)");
        }

        [TestMethod]
        public void WhenAddedFilterValuesAreNull_FilterIsReturnedUnchanged()
        {
            var filter1 = new EntityFilter<TestModel<int>>()
                .Add(x => x.ValueA, FilterOperator.Default, 1)
                .Add(x => x.ValueA, FilterOperator.Default, (int[])null);
            filter1.ToString().Should().Be("x => (x.ValueA == 1)");


            var filter2 = new EntityFilter<TestModel<int?>>()
                .Add(x => x.ValueA, FilterOperator.IsNull, 1)
                .Add(x => x.ValueA, FilterOperator.IsNull, (int[])null);
            filter2.ToString().Should().Be("x => (x.ValueA == null)");

            var filter3 = new EntityFilter<TestModel<int>>()
                .Add(x => x.ValueA, 1)
                .Add(x => x.ValueA, (int[])null);
            filter3.ToString().Should().Be("x => (x.ValueA == 1)");
        }

        [TestMethod]
        public void WhenAddedFilterValuesAreEmpty_FilterIsReturnedUnchanged()
        {
            var filter1 = new EntityFilter<TestModel<int>>()
                .Add(x => x.ValueA, FilterOperator.Default, 1)
                .Add(x => x.ValueA, FilterOperator.Default, Array.Empty<int>());
            filter1.ToString().Should().Be("x => (x.ValueA == 1)");


            var filter2 = new EntityFilter<TestModel<int?>>()
                .Add(x => x.ValueA, FilterOperator.IsNull, 1)
                .Add(x => x.ValueA, FilterOperator.IsNull, Array.Empty<int>());
            filter2.ToString().Should().Be("x => (x.ValueA == null)");

            var filter3 = new EntityFilter<TestModel<int>>()
                .Add(x => x.ValueA, 1)
                .Add(x => x.ValueA, Array.Empty<int>());
            filter3.ToString().Should().Be("x => (x.ValueA == 1)");
        }

        [TestMethod]
        public void WhenFilterReplacedViaSyntax_FilterIsChanged()
        {
            var filter = new EntityFilter<TestModel<int>>()
                .Replace(x => x.ValueA, "1")
                .Replace(x => x.ValueA, "2");
            filter.ToString().Should().Be("x => (x.ValueA == 2)");
        }

        [TestMethod]
        public void WhenNullFilterSyntaxIsReplaced_FilterIsReturnedUnchanged()
        {
            var filter = new EntityFilter<TestModel<bool>>()
                .Replace(x => x.ValueA, "1")
                .Replace(x => x.ValueA, (string)null);
            filter.ToString().Should().Be(string.Empty);
        }

        [TestMethod]
        public void WhenReplacedFilterValuesAreNull_FilterIsReturnedUnchanged()
        {
            var filter1 = new EntityFilter<TestModel<int>>()
                .Replace(x => x.ValueA, FilterOperator.Default, 1)
                .Replace(x => x.ValueA, FilterOperator.Default, (int[])null);
            filter1.ToString().Should().Be(string.Empty);


            var filter2 = new EntityFilter<TestModel<int?>>()
                .Replace(x => x.ValueA, FilterOperator.IsNull, 1)
                .Replace(x => x.ValueA, FilterOperator.IsNull, (int[])null);
            filter2.ToString().Should().Be(string.Empty);

            var filter3 = new EntityFilter<TestModel<int>>()
                .Replace(x => x.ValueA, 1)
                .Replace(x => x.ValueA, (int[])null);
            filter3.ToString().Should().Be(string.Empty);
        }

        [TestMethod]
        public void WhenReplacedFilterValuesAreEmpty_FilterIsReturnedUnchanged()
        {
            var filter1 = new EntityFilter<TestModel<int>>()
                .Replace(x => x.ValueA, FilterOperator.Default, 1)
                .Replace(x => x.ValueA, FilterOperator.Default, Array.Empty<int>());
            filter1.ToString().Should().Be(string.Empty);

            var filter2 = new EntityFilter<TestModel<int?>>()
                .Replace(x => x.ValueA, FilterOperator.IsNull, 1)
                .Replace(x => x.ValueA, FilterOperator.IsNull, Array.Empty<int>());
            filter2.ToString().Should().Be(string.Empty);

            var filter3 = new EntityFilter<TestModel<int>>()
                .Replace(x => x.ValueA, 1)
                .Replace(x => x.ValueA, Array.Empty<int>());
            filter3.ToString().Should().Be(string.Empty);
        }

        [TestMethod]
        public void WhenConvertedToQueryParams_FilterAttributesAreTakenIntoAccount()
        {
            var modelFilter = new EntityFilter<FilterAttributeTestModel>()
                .Add(x => x.FirstName, "John,Jane")
                .Add(x => x.LastName, "=Doe")
                .Add(x => x.Gender, "ShouldNotBeConverted")
                .Add(x => x.Birthday, ">2020-01-01,ISNULL")
                .Add(x => x.Birthday, "<2021-01-01");

            var queryParams = modelFilter.ToQueryParams();

            queryParams.Should().Be("testFirstName=John,Jane&testSurname=%3dDoe&testBirthday=%3e2020-01-01,ISNULL&testBirthday=%3c2021-01-01");
        }

        [ExcludeFromCodeCoverage]
        [FilterEntity(Prefix = "Test")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class FilterAttributeTestModel
        {
            public string FirstName { get; set; }

            [Filter(Name = "Surname")]
            public string LastName { get; set; }

            [Filter(Visible = false)]
            public string Gender { get; set; }

            public DateTime? Birthday { get; set; }
        }
    }
}

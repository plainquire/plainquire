using FluentAssertions;
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
    }
}

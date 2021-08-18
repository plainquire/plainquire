using FluentAssertions;
using FluentAssertions.Execution;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Models;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilterTests
{
    [TestClass]
    public class EntityFilterTests : TestBase
    {
        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<DateTime>))]
        public void WhenPropertyFilterIsAdded_ThenSamePropertyFiltersAreKept(EntityFilterFunc<TestModel<DateTime>> filterFunc)
        {
            var testItemsFilter = new EntityFilter<TestModel<DateTime>>()
                .Add(x => x.ValueA, ">=2000")
                .Add(x => x.ValueA, "<2001");

            var testItems = new List<TestModel<DateTime>>
            {
                new() { ValueA = new DateTime(1999, 01, 01)},
                new() { ValueA = new DateTime(2000, 01, 01)},
                new() { ValueA = new DateTime(2000, 12, 31)},
                new() { ValueA = new DateTime(2001, 01, 01)},
            };

            var filteredEntitiesLinq = filterFunc(testItems, testItemsFilter);
            filteredEntitiesLinq.Should().BeEquivalentTo(new[] { testItems[1], testItems[2] });
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenPropertyFilterIsReplaced_ThenSamePropertyFiltersAreRemoved(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var testItemsFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, "=A")
                .Replace(x => x.ValueA, "=B");

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "A" },
                new() { ValueA = "B" },
            };

            var filteredEntities = filterFunc(testItems, testItemsFilter).ToList();
            filteredEntities.Should().BeEquivalentTo(new[] { testItems[1] });
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenPropertyFilterIsCleared_ThenOtherPropertyFiltersAreKept(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var testItemsFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, "=A")
                .Replace(x => x.ValueB, "=2")
                .Clear(x => x.ValueB);

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "A", ValueB = "1" },
                new() { ValueA = "B", ValueB = "2" },
            };

            var filteredEntities = filterFunc(testItems, testItemsFilter).ToList();
            filteredEntities.Should().BeEquivalentTo(new[] { testItems[0] });
        }

        [TestMethod]
        public void WhenFilterIsCleared_ThenAllPropertyFiltersAreRemoved()
        {
            var entityFilter = new EntityFilter<TestModel<string>>()
                .Add(x => x.ValueA, "=A")
                .Add(x => x.ValueB, "=2")
                .Clear();

            var filter = entityFilter.CreateFilterExpression();
            filter.Should().BeNull();
        }

        [TestMethod]
        public void WhenNoValueForPropertyIsProvided_ThenArgumentExceptionIsThrown()
        {
            using var _ = new AssertionScope();

            Action createInvalidEntityFilter1 = () => new EntityFilter<TestModel<string>>()
                .Add(x => x.ValueA, (string[])null);

            Action createInvalidEntityFilter2 = () => new EntityFilter<TestModel<string>>()
                .Add(x => x.ValueA, Array.Empty<string>());

            createInvalidEntityFilter1.Should().Throw<ArgumentException>().WithMessage("At least one value is required");
            createInvalidEntityFilter2.Should().Throw<ArgumentException>().WithMessage("At least one value is required");
        }

        [TestMethod]
        public void WhenNullValueForPropertyIsProvided_ThenArgumentExceptionIsThrown()
        {
            Action createInvalidEntityFilter = () => new EntityFilter<TestModel<string>>()
                .Add(x => x.ValueA, "A", null);

            createInvalidEntityFilter.Should().Throw<ArgumentException>().WithMessage("Filter values cannot be null. If filtering for NULL is intended, use filter operator 'IsNull' or 'NotNull'");
        }

        [TestMethod]
        public void WhenUnknownTypeIsFiltered_ThenArgumentExceptionIsThrown()
        {
            using var _ = new AssertionScope();

            Action createInvalidEntityFilter1 = () => new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, new List<string>());

            Action createInvalidEntityFilter2 = () => new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, DateTimeOffset.Now);

            createInvalidEntityFilter1.Should().Throw<ArgumentException>().WithMessage("The type 'System.Collections.Generic.List`1[System.String]' is not filterable by any known expression creator");
            createInvalidEntityFilter2.Should().Throw<ArgumentException>().WithMessage("The type 'System.DateTimeOffset' is not filterable by any known expression creator");
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenPropertyIsFilteredForNull_FilterOperatorIsRecognizedCorrectly(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var isNullFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, FilterOperator.IsNull);

            var isNotNullFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, "NOTNULL");

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "A" },
                new() { ValueA = null },
            };

            var isNullItems = filterFunc(testItems, isNullFilter).ToList();
            var notNullItems = filterFunc(testItems, isNotNullFilter).ToList();

            isNullItems.Should().BeEquivalentTo(new[] { testItems[1] });
            notNullItems.Should().BeEquivalentTo(new[] { testItems[0] });
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenPropertyIsFilteredForNull_GivenValuesAreIgnored(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var isNullFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, FilterOperator.IsNull, "A", "B");

            var isNotNullFilter = new EntityFilter<TestModel<string>>()
                // ReSharper disable once StringLiteralTypo
                .Replace(x => x.ValueA, "NOTNULLA,B");

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "A" },
                new() { ValueA = null },
            };

            var isNullItems = filterFunc(testItems, isNullFilter).ToList();
            var notNullItems = filterFunc(testItems, isNotNullFilter).ToList();

            isNullItems.Should().BeEquivalentTo(new[] { testItems[1] });
            notNullItems.Should().BeEquivalentTo(new[] { testItems[0] });
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenPropertyIsFilteredForEmptyString_MatchingEntitiesAreReturned(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var filter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, "");

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "A" },
                new() { ValueA = "" },
                new() { ValueA = null },
            };

            var filteredItems = filterFunc(testItems, filter).ToList();

            filteredItems.Should().BeEquivalentTo(new[] { testItems[1] });
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<bool>))]
        public void WhenPropertyFilterValueIsInvalid_FilterExpressionCreationExceptionIsThrown(EntityFilterFunc<TestModel<bool>> filterFunc)
        {
            var filter = new EntityFilter<TestModel<bool>>()
                .Replace(x => x.ValueA, "=");

            var testItems = new List<TestModel<bool>> { new() { ValueA = true } };

            Action filterItems = () => filterFunc(testItems, filter);

            filterItems.Should().Throw<FilterExpressionCreationException>().WithMessage("Unable to parse given filter value");
        }

        [TestMethod]
        public void WhenPropertyFilterIsRetrieved_ThenFilterSyntaxIsSameAsGiven()
        {
            var filterSyntax = ">=2000";
            var filter = new EntityFilter<TestModel<DateTime>>()
                .Add(x => x.ValueA, filterSyntax);

            var retrievedFilterSyntax = filter.GetPropertyFilter(x => x.ValueA);

            retrievedFilterSyntax.Should().Be(filterSyntax);
        }

        [TestMethod]
        public void WhenNestedFilterIsRetrieved_ThenFilterIsSameAsGiven()
        {
            using var _ = new AssertionScope();

            var nestedFilter = new EntityFilter<TestModelNested>()
                .Add(x => x.Value, FilterOperator.EqualCaseSensitive, "A");

            var objectFilter = new EntityFilter<TestModel<string>>()
                .Add(x => x.NestedObject, nestedFilter);
            var retrievedObjectFilter = objectFilter
                .GetNestedFilter(x => x.NestedObject);
            retrievedObjectFilter.Should().BeEquivalentTo(nestedFilter);

            var listFilter = new EntityFilter<TestModel<string>>()
                .Add(x => x.NestedList, nestedFilter);
            var retrievedListFilter = listFilter
                .GetNestedFilter<List<TestModelNested>, TestModelNested>(x => x.NestedList);
            retrievedListFilter.Should().BeEquivalentTo(nestedFilter);
        }
    }
}

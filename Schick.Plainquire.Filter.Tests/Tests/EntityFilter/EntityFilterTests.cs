using FluentAssertions;
using FluentAssertions.Execution;
using Schick.Plainquire.Filter.Enums;
using Schick.Plainquire.Filter.Exceptions;
using Schick.Plainquire.Filter.Extensions;
using Schick.Plainquire.Filter.Filters;
using Schick.Plainquire.Filter.Tests.Attributes;
using Schick.Plainquire.Filter.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Schick.Plainquire.Filter.Tests.Tests.EntityFilter;

[TestClass, ExcludeFromCodeCoverage]
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
            new() { ValueA = new DateTime(2001, 01, 01)}
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
            new() { ValueA = "B" }
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
            new() { ValueA = "B", ValueB = "2" }
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

        var filter = entityFilter.CreateFilter();
        filter.Should().BeNull();
    }

    [TestMethod]
    public void WhenNoValueForPropertyIsProvided_ThenFilterIsLeftUnchanged()
    {
        using var _ = new AssertionScope();

        var entityFilter1 = new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, (string[]?)null);

        var entityFilter2 = new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, Array.Empty<string>());

        entityFilter1.Should().Match<EntityFilter<TestModel<string>>>(x => x.IsEmpty());
        entityFilter2.Should().Match<EntityFilter<TestModel<string>>>(x => x.IsEmpty());
    }

    [TestMethod]
    public void WhenNullValueForPropertyIsProvided_ThenArgumentExceptionIsThrown()
    {
        Action createInvalidEntityFilter = () => new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, "A", null);

        createInvalidEntityFilter.Should().Throw<ArgumentException>().WithMessage("Filter values cannot be null. If filtering for NULL is intended, use filter operator 'IsNull' or 'NotNull' (Parameter 'value')");
    }

    [TestMethod]
    public void WhenUnknownTypeIsFiltered_ThenArgumentExceptionIsThrown()
    {
        using var _ = new AssertionScope();

        Action createInvalidEntityFilter1 = () => new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, new List<string>());

        createInvalidEntityFilter1.Should().Throw<ArgumentException>().WithMessage("The type 'System.Collections.Generic.List`1[System.String]' is not filterable by any known expression creator (Parameter 'value')");
    }

    [DataTestMethod]
    [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
    public void WhenPropertyIsFilteredForNull_FilterOperatorIsRecognizedCorrectly(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var isNullFilterAdded = new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, FilterOperator.IsNull);

        var isNotNullFilterAdded = new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, "NOTNULL");

        var isNullFilterReplaced = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, FilterOperator.IsNull);

        var isNotNullFilterReplaced = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, "NOTNULL");

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "A" },
            new() { ValueA = null }
        };

        var isNullItemsAdded = filterFunc(testItems, isNullFilterAdded).ToList();
        var notNullItemsAdded = filterFunc(testItems, isNotNullFilterAdded).ToList();
        var isNullItemsReplaced = filterFunc(testItems, isNullFilterReplaced).ToList();
        var notNullItemsReplaced = filterFunc(testItems, isNotNullFilterReplaced).ToList();

        isNullItemsAdded.Should().BeEquivalentTo(new[] { testItems[1] });
        notNullItemsAdded.Should().BeEquivalentTo(new[] { testItems[0] });
        isNullItemsReplaced.Should().BeEquivalentTo(new[] { testItems[1] });
        notNullItemsReplaced.Should().BeEquivalentTo(new[] { testItems[0] });
    }

    [DataTestMethod]
    [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
    public void WhenPropertyIsFilteredForNull_GivenValuesAreIgnored(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var isNullFilterAdded = new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, FilterOperator.IsNull, "A", "B");

        var isNotNullFilterAdded = new EntityFilter<TestModel<string>>()
            // ReSharper disable once StringLiteralTypo
            .Add(x => x.ValueA, "NOTNULLA,NOTNULLB");

        var isNullFilterReplaced = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, FilterOperator.IsNull, "A", "B");

        var isNotNullFilterReplaced = new EntityFilter<TestModel<string>>()
            // ReSharper disable once StringLiteralTypo
            .Replace(x => x.ValueA, "NOTNULLA,NOTNULLB");

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "A" },
            new() { ValueA = null }
        };

        var isNullItemsAdded = filterFunc(testItems, isNullFilterAdded).ToList();
        var notNullItemsAdded = filterFunc(testItems, isNotNullFilterAdded).ToList();
        var isNullItemsReplaced = filterFunc(testItems, isNullFilterReplaced).ToList();
        var notNullItemsReplaced = filterFunc(testItems, isNotNullFilterReplaced).ToList();

        isNullItemsAdded.Should().BeEquivalentTo(new[] { testItems[1] });
        notNullItemsAdded.Should().BeEquivalentTo(new[] { testItems[0] });
        isNullItemsReplaced.Should().BeEquivalentTo(new[] { testItems[1] });
        notNullItemsReplaced.Should().BeEquivalentTo(new[] { testItems[0] });
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
            new() { ValueA = null }
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

        filterItems.Should().Throw<FilterExpressionException>().WithMessage("Unable to parse given filter value");
    }

    [TestMethod]
    public void WhenPropertyFilterStringForSingleValueIsRetrieved_ThenFilterSyntaxIsSameAsGiven()
    {
        const string filterSyntax = ">=2000";
        var filter = new EntityFilter<TestModel<DateTime>>()
            .Add(x => x.ValueA, filterSyntax);

        var retrievedFilterSyntax = filter.GetPropertyFilterSyntax(x => x.ValueA);

        retrievedFilterSyntax.Should().Be(filterSyntax);
    }

    [TestMethod]
    public void WhenPropertyFilterStringForMultipleValuesAreRetrieved_ThenFilterSyntaxIsSameAsGiven()
    {
        const string filterSyntax = ">=2000,<3000";
        var filter = new EntityFilter<TestModel<DateTime>>()
            .Add(x => x.ValueA, filterSyntax);

        var retrievedFilterSyntax = filter.GetPropertyFilterSyntax(x => x.ValueA);

        retrievedFilterSyntax.Should().Be(filterSyntax);
    }

    [TestMethod]
    public void WhenPropertyFilterValuesForSingleValueIsRetrieved_ThenFilterSyntaxIsSameAsGiven()
    {
        var filterValues = new[] { Filters.ValueFilter.Create(">=2000") };
        var filter = new EntityFilter<TestModel<DateTime>>()
            .Add(x => x.ValueA, filterValues);

        var retrievedFilterValues = filter.GetPropertyFilterValues(x => x.ValueA);

        retrievedFilterValues.Should().BeEquivalentTo(filterValues);
    }

    [TestMethod]
    public void WhenPropertyFilterValuesForMultipleValuesAreRetrieved_ThenFilterSyntaxIsSameAsGiven()
    {
        var filterValues = new[] { Filters.ValueFilter.Create(">=2000"), Filters.ValueFilter.Create("<2000") };
        var filter = new EntityFilter<TestModel<DateTime>>()
            .Add(x => x.ValueA, filterValues);

        var retrievedFilterValues = filter.GetPropertyFilterValues(x => x.ValueA);

        retrievedFilterValues.Should().BeEquivalentTo(filterValues);
    }

    [TestMethod]
    public void WhenNestedFilterIsRetrieved_ThenFilterIsSameAsGiven()
    {
        using var _ = new AssertionScope();

        var nestedFilter = new EntityFilter<TestModelNested>()
            .Add(x => x.Value, FilterOperator.EqualCaseSensitive, "A");

        var objectFilter = new EntityFilter<TestModel<string>>()
            .AddNested(x => x.NestedObject, nestedFilter);
        var retrievedObjectFilter = objectFilter
            .GetNestedFilter(x => x.NestedObject);
        retrievedObjectFilter.Should().BeEquivalentTo(nestedFilter);

        var listFilter = new EntityFilter<TestModel<string>>()
            .AddNested(x => x.NestedList, nestedFilter);
        var retrievedListFilter = listFilter
            .GetNestedFilter<List<TestModelNested>, TestModelNested>(x => x.NestedList);
        retrievedListFilter.Should().BeEquivalentTo(nestedFilter);
    }
}
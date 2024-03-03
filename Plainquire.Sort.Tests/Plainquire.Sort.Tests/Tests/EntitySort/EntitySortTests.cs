using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Tests.Models;
using Plainquire.Sort.Tests.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.EntitySort;

[TestClass, ExcludeFromCodeCoverage]
public class EntitySortTests
{
    private static readonly SortConfiguration _defaultConfiguration = new();

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenObjectIsNull_ThenValidQueryableIsCreated(EntitySortFunction<TestModel<string>> sortFunc)
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value);

        var testItems = new List<TestModel<string>>
        {
            new() { Value = null},
            new() { Value = "even"  },
            new() { Value = null },
            new() { Value = "even" }
        };

        var sortedEntities = sortFunc(testItems, sort);

        sortedEntities.Should().ContainInOrder(testItems[0], testItems[2], testItems[1], testItems[3]);
    }

    [TestMethod]
    public void WhenPropertySortIsAdded_ThenSamePropertySorOrdersAreKept()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.Value);

        sort.PropertySorts.Should().HaveCount(2);
    }

    [TestMethod]
    public void WhenPropertySortIsRemoved_OtherSortsAreKept()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.Value2);

        sort.Remove(x => x.Value);

        sort.PropertySorts.Should().ContainSingle(x => x.PropertyPath == "Value2");
    }

    [TestMethod]
    public void WhenNestedPropertySortIsRemoved_OtherSortsAreKept()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.NestedObject!.Value);

        sort.Remove(x => x.NestedObject!.Value);

        sort.PropertySorts.Should().ContainSingle(x => x.PropertyPath == "Value");
    }

    [TestMethod]
    public void WhenSortIsCleared_AllPropertySortsAreRemoved()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.Value2);

        sort.Clear();

        sort.PropertySorts.Should().BeEmpty();
    }

    [TestMethod]
    public void WhenPropertySortStringForSingleValueIsRetrieved_ThenSortSyntaxIsSameAsGiven()
    {
        var syntax = $"NestedObject.Id{_defaultConfiguration.PrimaryDescendingPostfix}";
        var sort = new EntitySort<TestModel<string>>()
            .Add(syntax);

        sort.ToString().Should().Be(syntax);
    }

    [TestMethod]
    public void WhenSortToStringIsCalled_SortSyntaxIsReturned()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value2, SortDirection.Descending, 20)
            .Add(x => x.NestedObject!.Id, default, 10);

        sort.ToString().Should().Be($"NestedObject.Id{_defaultConfiguration.PrimaryAscendingPostfix}, Value2{_defaultConfiguration.PrimaryDescendingPostfix}");
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenEntitiesSortedByMultiplePropertyExpressions_EntitiesSortedAsExpected(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "222" } },
            new() { Value = "even", NestedObject = new() { Value = "1111" } },
            new() { Value = "odd", NestedObject = new() { Value = "4" } },
            new() { Value = "even", NestedObject = new() { Value = "33" } }
        ];

        var entitySort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value, SortDirection.Ascending)
            .Add(x => x.NestedObject!.Value!.Length, SortDirection.Descending);

        var sortedItems = sortFunc(testItems, entitySort);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[3], testItems[0], testItems[2]);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenEntitiesSortedByMultiplePropertySyntax_EntitiesSortedAsExpected(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "222" } },
            new() { Value = "even", NestedObject = new() { Value = "1111" } },
            new() { Value = "odd", NestedObject = new() { Value = "4" } },
            new() { Value = "even", NestedObject = new() { Value = "33" } }
        ];

        var entitySort = new EntitySort<TestModel<string>>()
            .Add("Value")
            .Add("NestedObject.Value.Length-desc");

        var sortedItems = sortFunc(testItems, entitySort);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[3], testItems[0], testItems[2]);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenEntitiesSortedWithExplicitPositions_EntitiesSortedAsExpected(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "2" } },
            new() { Value = "even", NestedObject = new() { Value = "1" } },
            new() { Value = "odd", NestedObject = new() { Value = "4" } },
            new() { Value = "even", NestedObject = new() { Value = "3" } }
        ];

        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value, SortDirection.Ascending, 20)
            .Add(x => x.NestedObject!.Value, SortDirection.Ascending, 10);

        var sortedItems = sortFunc(testItems, sort);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[0], testItems[3], testItems[2]);
    }

    [TestMethod]
    public void WhenEntitiesSortedWithoutExplicitPositions_PositionIsGenerated()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value, SortDirection.Ascending)
            .Add(x => x.NestedObject!.Value, SortDirection.Ascending);

        sort.PropertySorts.Select(x => x.Position).Should().ContainInOrder(0, 1);
    }

    [TestMethod]
    public void WhenEntitiesSortedWithPartialExplicitPositions_PositionIsIncremented()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value, SortDirection.Ascending, 10)
            .Add(x => x.NestedObject!.Value, SortDirection.Ascending);

        sort.PropertySorts.Select(x => x.Position).Should().ContainInOrder(10, 11);
    }

    [TestMethod]
    public void WhenPropertySortSyntaxIsRetrieved_FinalSortSyntaxIsReturned()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value, SortDirection.Descending)
            .Add(x => x.Value);

        var valueASyntax = sort.GetPropertySortSyntax(x => x.Value);
        valueASyntax.Should().Be($"Value{_defaultConfiguration.PrimaryAscendingPostfix}");
    }

    [TestMethod]
    public void WhenPropertySortDirectionIsRetrieved_FinalSortDirectionIsReturned()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.Value, SortDirection.Descending);

        var valueASortDirection = sort.GetPropertySortDirection(x => x.Value);
        valueASortDirection.Should().Be(SortDirection.Descending);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenEntitiesSortedByLowercaseSyntax_EntitiesSortedAsExpected(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "222" } },
            new() { Value = "even", NestedObject = new() { Value = "1111" } },
        ];

        var entitySort = new EntitySort<TestModel<string>>()
            .Add("nestedObject.value.length-desc");

        var sortedItems = sortFunc(testItems, entitySort);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[0]);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenEntitiesSortedWithLowercaseSyntaxForbiddenByConfig_ExceptionIsThrow(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems = [];

        var configuration = new SortConfiguration { CaseInsensitivePropertyMatching = false };

        var entitySort = new EntitySort<TestModel<string>>(configuration)
            .Add("NestedObject.Value.length-desc");

        var sortItems = () => sortFunc(testItems, entitySort);
        sortItems.Should()
            .Throw<ArgumentException>()
            .WithMessage("Property 'length' not found on type 'String'.");
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenIgnoreParseExceptionsIsConfiguredAndPropertyNotFound_PropertyIsIgnoredWhileSorting(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "3" } },
            new() { Value = "even", NestedObject = new() { Value = "2" } },
            new() { Value = "odd", NestedObject = new() { Value = "1" } },
        ];

        var configuration = new SortConfiguration { IgnoreParseExceptions = true };

        var entitySort = new EntitySort<TestModel<string>>(configuration)
            .Add("notExists")
            .Add("NestedObject.Value");

        var sortedItems = sortFunc(testItems, entitySort);
        sortedItems.Should().ContainInOrder(testItems[2], testItems[1], testItems[0]);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenNoSortIsAdded_EntitySortIsEmpty(EntitySortFunction<TestModel<string>> sortFunc)
    {
        var entitySort = new EntitySort<TestModel<string>>();
        entitySort.Should().Match<EntitySort<TestModel<string>>>(x => x.IsEmpty());
    }
}
using FluentAssertions;
using FluentAssertions.Execution;
using FS.SortQueryableCreator.Sorts;
using FS.SortQueryableCreator.Tests.Models;
using FS.SortQueryableCreator.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static FS.SortQueryableCreator.Sorts.PropertySort;

namespace FS.SortQueryableCreator.Tests.Tests.EntitySort;

[TestClass, ExcludeFromCodeCoverage]
public class NestedSortTests
{
    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenNestedSortIsApplied_ThenNestedPropertyIsSorted(EntitySortFunction<TestModel<string>> sortFunc)
    {
        var nestedSort = new EntitySort<TestModelNested<string>>()
            .Add(x => x.Value);

        var outerSort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .AddNested(x => x.NestedObject, nestedSort);

        var testItems = new List<TestModel<string>>
        {
            new() { Value = "odd", NestedObject = new() { Value = "4" } },
            new() { Value = "even", NestedObject = new() { Value = "3" } },
            new() { Value = "odd", NestedObject = new() { Value = "2" } },
            new() { Value = "even", NestedObject = new() { Value = "1" } }
        };

        var sortedEntities = sortFunc(testItems, outerSort);

        sortedEntities.Should().ContainInOrder(testItems[3], testItems[1], testItems[2], testItems[0]);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenNestedObjectIsNull_ThenValidQueryableIsCreated(EntitySortFunction<TestModel<string>> sortFunc)
    {
        var nestedSort = new EntitySort<TestModelNested<string>>()
            .Add(x => x.Value);

        var outerSort = new EntitySort<TestModel<string>>()
            .AddNested(x => x.NestedObject, nestedSort);

        var testItems = new List<TestModel<string>>
        {
            new() { Value = "odd", NestedObject = null },
            new() { Value = "even",NestedObject = null },
            new() { Value = "odd", NestedObject = new TestModelNested<string>{ Value = "20" } },
            new() { Value = "even", NestedObject = new TestModelNested<string>{ Value = "10" } }
        };

        var sortedEntities = sortFunc(testItems, outerSort);

        sortedEntities.Should().ContainInOrder(testItems[0], testItems[1], testItems[3], testItems[2]);
    }

    [TestMethod]
    public void WhenNestedSortIsAdded_PathToSelfIsReplacedByPropertyName()
    {
        var nestedSort = new EntitySort<TestModelNested<string>>()
            .Add(x => x.Value);

        var outerSort = new EntitySort<TestModel<string>>()
            .AddNested(x => x.NestedObject, nestedSort);

        outerSort._propertySorts.Should().ContainSingle(x => x.PropertyPath == "NestedObject.Value");
    }

    [TestMethod]
    public void WhenNestedSortIsAdded_NestedPathsAreKept()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject)
            .Add(x => x.NestedObject!.Value!.Length);

        var nestedSort = sort
            .GetNested(x => x.NestedObject);

        using var _ = new AssertionScope();
        nestedSort._propertySorts.Should().HaveCount(2);
        nestedSort._propertySorts.Should().Contain(x => x.PropertyPath == PATH_TO_SELF);
        nestedSort._propertySorts.Should().Contain(x => x.PropertyPath == "Value.Length");
    }

    [TestMethod]
    public void WhenNestedSortIsRetrieved_ItEqualsToAddedOne()
    {
        var nestedSort = new EntitySort<TestModelNested<string>>()
            .Add(x => x.Value);

        var outerSort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .AddNested(x => x.NestedObject, nestedSort);

        var retrievedNestedSort = outerSort
            .GetNested(x => x.NestedObject);

        retrievedNestedSort._propertySorts.Should().BeEquivalentTo(nestedSort._propertySorts);
    }

    [TestMethod]
    public void WhenNestedSortIsCleared_ChildSortsAreRemoved()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.NestedObject!.Value)
            .Add(x => x.NestedObject!.Value!.Length);

        sort.ClearNested(x => x.NestedObject);

        sort._propertySorts.Should().ContainSingle(x => x.PropertyPath == "Value");
    }

    [TestMethod]
    public void WhenNestedSortIsSetToNull_ArgumentNullExceptionIsThrown()
    {
        Action setNestedSortToNull = () => new EntitySort<TestModel<string>>()
            .AddNested(x => x.NestedObject, null!);

        setNestedSortToNull.Should().Throw<ArgumentNullException>();
    }
}
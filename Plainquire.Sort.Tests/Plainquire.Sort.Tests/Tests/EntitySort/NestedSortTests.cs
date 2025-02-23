﻿using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Sort.Tests.Models;
using Plainquire.Sort.Tests.Services;
using Plainquire.TestSupport.Services;
using System;
using System.Collections.Generic;
using static Plainquire.Sort.PropertySort;

namespace Plainquire.Sort.Tests.Tests.EntitySort;

[TestFixture]
public class NestedSortTests : TestContainer
{
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

    [Test]
    public void WhenNestedSortIsAdded_PathToSelfIsReplacedByPropertyName()
    {
        var nestedSort = new EntitySort<TestModelNested<string>>()
            .Add(x => x.Value);

        var outerSort = new EntitySort<TestModel<string>>()
            .AddNested(x => x.NestedObject, nestedSort);

        outerSort.PropertySorts.Should().ContainSingle(x => x.PropertyPath == "NestedObject.Value");
    }

    [Test]
    public void WhenNestedSortIsAdded_NestedPathsAreKept()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject)
            .Add(x => x.NestedObject!.Value!.Length);

        var nestedSort = sort
            .GetNested(x => x.NestedObject);

        using var _ = new AssertionScope();
        nestedSort.PropertySorts.Should().HaveCount(2);
        nestedSort.PropertySorts.Should().Contain(x => x.PropertyPath == PATH_TO_SELF);
        nestedSort.PropertySorts.Should().Contain(x => x.PropertyPath == "Value.Length");
    }

    [Test]
    public void WhenNestedSortIsRetrieved_ItEqualsToAddedOne()
    {
        var nestedSort = new EntitySort<TestModelNested<string>>()
            .Add(x => x.Value);

        var outerSort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .AddNested(x => x.NestedObject, nestedSort);

        var retrievedNestedSort = outerSort
            .GetNested(x => x.NestedObject);

        retrievedNestedSort.PropertySorts.Should().BeEquivalentTo(nestedSort.PropertySorts);
    }

    [Test]
    public void WhenNestedSortIsRemoved_ChildSortsAreRemoved()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.NestedObject!.Value)
            .Add(x => x.NestedObject!.Value!.Length);

        sort.RemoveNested(x => x.NestedObject);

        sort.PropertySorts.Should().ContainSingle(x => x.PropertyPath == "Value");
    }

    [Test]
    public void WhenNestedSortIsSetToNull_ArgumentNullExceptionIsThrown()
    {
        Action setNestedSortToNull = () => new EntitySort<TestModel<string>>()
            .AddNested(x => x.NestedObject, null!);

        setNestedSortToNull.Should().Throw<ArgumentNullException>();
    }
}
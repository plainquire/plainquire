using FluentAssertions;
using NUnit.Framework;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Collections.Generic;

namespace Plainquire.Filter.Tests.Tests.EntityFilter;

[TestFixture]
public class NestedFilterTests
{
    [FilterFuncDataSource<TestModel<string>>]
    public void WhenNestedEntityFilterIsApplied_ThenNestedPropertyIsFiltered(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var nestedFilter = new EntityFilter<TestModelNested?>()
            .Replace(x => x!.Value, "=NestedA");

        var outerFilter = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, "=OuterA")
            .ReplaceNested(x => x.NestedObject, nestedFilter);

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "OuterA", NestedObject = new() { Value = "NestedA" } },
            new() { ValueA = "OuterA", NestedObject = new() { Value = "NestedB" } },
            new() { ValueA = "OuterB", NestedObject = new() { Value = "NestedB" } },
            new() { ValueA = "OuterA", NestedObject = null },
        };

        var filteredEntities = filterFunc(testItems, outerFilter);

        filteredEntities.Should().BeEquivalentTo([testItems[0]]);
    }

    [FilterFuncDataSource<TestModel<string>>]
    public void WhenNestedEntityFilterIsApplied_ThenNestedListIsFiltered(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var nestedFilter = new EntityFilter<TestModelNested>()
            .Replace(x => x.Value, "=NestedA");

        var outerFilter = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, "=OuterA")
            .ReplaceNested(x => x.NestedList, nestedFilter);

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "OuterA", NestedList = [new() { Value = "NestedA" }] },
            new() { ValueA = "OuterA", NestedList = [new() { Value = "NestedB" }] },
            new() { ValueA = "OuterA", NestedList = [new() { Value = "NestedA" }, new() { Value = "NestedB" }] },
            new() { ValueA = "OuterB", NestedList = [new() { Value = "NestedA" }] },
            new() { ValueA = "OuterA", NestedList = null },
        };

        var filteredEntities = filterFunc(testItems, outerFilter);

        filteredEntities.Should().BeEquivalentTo([testItems[0], testItems[2]]);
    }

    [FilterFuncDataSource<TestModel<string>>]
    public void WhenNestedObjectIsNUll_ConditionPropertyAccessIsGeneratedAsNeeded(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var nestedFilter = new EntityFilter<TestModelNested?>()
            .Replace(x => x!.Value, "=NestedA");

        var outerFilter = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, "=OuterA")
            .ReplaceNested(x => x.NestedObject, nestedFilter);

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "OuterA", NestedObject = new() { Value = "NestedA" } },
            new() { ValueA = "OuterA", NestedObject = new() { Value = "NestedB" } },
            new() { ValueA = "OuterB", NestedObject = new() { Value = "NestedB" } },
            new() { ValueA = "OuterA", NestedObject = null },
        };

        var filteredEntities = filterFunc(testItems, outerFilter);

        filteredEntities.Should().BeEquivalentTo([testItems[0]]);
    }

    [FilterFuncDataSource<TestModel<string>>]
    public void WhenNestedListIsNUll_ConditionPropertyAccessIsGeneratedAsNeeded(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var nestedFilter = new EntityFilter<TestModelNested>()
            .Replace(x => x.Value, "=NestedA");

        var outerFilter = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, "=OuterA")
            .ReplaceNested(x => x.NestedList, nestedFilter);

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "OuterA", NestedList = [new() { Value = "NestedA" }] },
            new() { ValueA = "OuterA", NestedList = [new() { Value = "NestedB" }] },
            new() { ValueA = "OuterA", NestedList = [new() { Value = "NestedA" }, new() { Value = "NestedB" }] },
            new() { ValueA = "OuterB", NestedList = [new() { Value = "NestedA" }] },
            new() { ValueA = "OuterA", NestedList = null },
        };

        var filteredEntities = filterFunc(testItems, outerFilter);

        filteredEntities.Should().BeEquivalentTo([testItems[0], testItems[2]]);
    }

    [Test]
    public void WhenNestedPropertyFilterIsAdded_ThenArgumentExceptionIsThrown()
    {
        ((Action)(() => new EntityFilter<TestModel<string>>().Add(x => x.NestedObject!.Value, "=NestedA")))
            .Should()
            .Throw<ArgumentException>().WithMessage("Given property must be a first level property access expression like person => person.Name (Parameter 'property')");
    }

    [FilterFuncDataSource<TestModel<string>>]
    public void WhenNestedFilterIsNull_ThenEmptyNestedFilterIsCreated(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var outerFilter = new EntityFilter<TestModel<string>>()
            .Add(x => x.ValueA, "A")
            .AddNested(x => x.NestedList, (EntityFilter<TestModelNested>?)null);

        var testItems = new TestModel<string>[] { new() { ValueA = "A" } };

        var filteredEntities = filterFunc(testItems, outerFilter);
        filteredEntities.Should().BeEquivalentTo([testItems[0]]);
    }
}
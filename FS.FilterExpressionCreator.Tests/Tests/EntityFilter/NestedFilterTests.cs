using FluentAssertions;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilter
{
    [TestClass, ExcludeFromCodeCoverage]
    public class NestedFilterTests : TestBase
    {
        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenNestedEntityFilterIsApplied_ThenNestedPropertyIsFiltered(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var nestedFilter = new EntityFilter<TestModelNested>()
                .Replace(x => x.Value, "=NestedA");

            var outerFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, "=OuterA")
                .ReplaceNested(x => x.NestedObject, nestedFilter);

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "OuterA", NestedObject = new() { Value = "NestedA" } },
                new() { ValueA = "OuterA", NestedObject = new() { Value = "NestedB" } },
                new() { ValueA = "OuterB", NestedObject = new() { Value = "NestedB" } },
            };

            var filteredEntities = filterFunc(testItems, outerFilter);

            filteredEntities.Should().BeEquivalentTo(new[] { testItems[0] });
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenNestedEntityFilterIsApplied_ThenNestedListIsFiltered(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var nestedFilter = new EntityFilter<TestModelNested>()
                .Replace(x => x.Value, "=NestedA");

            var outerFilter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, "=OuterA")
                .ReplaceNested(x => x.NestedList, nestedFilter);

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "OuterA", NestedList = new() { new() { Value = "NestedA" } } },
                new() { ValueA = "OuterA", NestedList = new() { new() { Value = "NestedB" } } },
                new() { ValueA = "OuterA", NestedList = new() { new() { Value = "NestedA" }, new() { Value = "NestedB" } } },
                new() { ValueA = "OuterB", NestedList = new() { new() { Value = "NestedB" } } },
            };

            var filteredEntities = filterFunc(testItems, outerFilter);

            filteredEntities.Should().BeEquivalentTo(new[] { testItems[0], testItems[2] });
        }

        [TestMethod]
        public void WhenNestedPropertyFilterIsAdded_ThenArgumentExceptionIsThrown()
        {
            ((Action)(() => new EntityFilter<TestModel<string>>().Add(x => x.NestedObject.Value, "=NestedA")))
                .Should()
                .Throw<ArgumentException>().WithMessage("Given property must be a first level property access expression like person => person.Name (Parameter 'property')");
        }

        [TestMethod]
        public void WhenNestedObjectOfFilterIsNull_ThenValidFilterIsCreated()
        {
            var nestedFilter = new EntityFilter<TestModelNested>()
                .Replace(x => x.Value, "=NestedA");

            var outerFilter = new EntityFilter<TestModel<string>>()
                .AddNested(x => x.NestedObject, nestedFilter);

            var items = new TestModel<string>[] { new() { NestedObject = null } };

            var filteredItems = items.Where(outerFilter).ToList();
            filteredItems.Should().BeEmpty();
        }

        [TestMethod]
        public void WhenNestedListOfFilterIsNull_ThenValidFilterIsCreated()
        {
            var nestedFilter = new EntityFilter<TestModelNested>()
                .Replace(x => x.Value, "=NestedA");

            var outerFilter = new EntityFilter<TestModel<string>>()
                .AddNested(x => x.NestedList, nestedFilter);

            var items = new TestModel<string>[] { new() { NestedList = null } };

            var filteredItems = items.Where(outerFilter).ToList();
            filteredItems.Should().BeEmpty();
        }

        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenNestedGivenNestedFilterIsNull_ThenEmptyNestedFilterIsCreated(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var outerFilter = new EntityFilter<TestModel<string>>()
                .Add(x => x.ValueA, "A")
                .AddNested(x => x.NestedList, (EntityFilter<TestModelNested>)null);

            var testItems = new TestModel<string>[] { new() { ValueA = "A" } };

            var filteredEntities = filterFunc(testItems, outerFilter);
            filteredEntities.Should().BeEquivalentTo(new[] { testItems[0] });
        }
    }
}

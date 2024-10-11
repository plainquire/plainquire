using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.EntitySort;

[TestClass]
public class SortConfigurationTests
{
    [TestMethod]
    public void WhenConfigurationIsCreated_ThenDefaultValuesAreSet()
    {
        var configuration = new SortConfiguration();

        configuration.Should().BeEquivalentTo(new SortConfiguration
        {
            UseConditionalAccess = SortConditionalAccess.WhenEnumerableQuery
        });
    }

    [TestMethod]
    [SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Required to execute sorting")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Required to execute sorting")]
    public void WhenConditionalAccessIsSetToNever_CallAgainstEnumerableWithNullValuesThrows()
    {
        var testItems = new List<TestModel<string>>
        {
            new() { NestedObject = null },
            new() { NestedObject = new TestModelNested<string>{Value = "" } },
        };

        var configuration = new SortConfiguration
        {
            UseConditionalAccess = SortConditionalAccess.Never
        };

        var sort = new EntitySort<TestModel<string>>(configuration)
            .Add(x => x.NestedObject!.Value);

        Action sortEnumerableWithNullValues = () => testItems.OrderBy(sort).ToList();

        sortEnumerableWithNullValues.Should().Throw<NullReferenceException>();
    }

    [TestMethod]
    public void WhenConditionalAccessIsSetToAlways_ConditionalAccessGeneratedForProvidersOtherThanEnumerableQuery()
    {
        var configuration = new SortConfiguration
        {
            UseConditionalAccess = SortConditionalAccess.Always
        };

        var sort = new EntitySort<TestModel<string>>(configuration)
            .Add(x => x.NestedObject!.Value);

        using var dbContext = new TestDbContext<TestModel<string>>(useSqlite: true);

        var queryable = dbContext
            .Set<TestModel<string>>()
            .OrderBy(sort);

        queryable.Expression.ToString()
            .Should()
            .Contain("IIF((x == null), null, x.NestedObject)");
    }

    [TestMethod]
    public void WhenConditionalAccessIsNotSet_ConditionalAccessIsNotGeneratedForProvidersOtherThanEnumerableQuery()
    {
        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject!.Value);

        using var dbContext = new TestDbContext<TestModel<string>>(useSqlite: true);

        var queryable = dbContext
            .Set<TestModel<string>>()
            .OrderBy(sort);

        queryable.Expression.ToString()
            .Should()
            .Contain("OrderBy(x => x.NestedObject.Value)");
    }
}
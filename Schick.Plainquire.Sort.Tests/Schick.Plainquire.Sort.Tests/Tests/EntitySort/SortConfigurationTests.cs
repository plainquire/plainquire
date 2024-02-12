using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Sort.Abstractions.Configurations;
using Schick.Plainquire.Sort.Extensions;
using Schick.Plainquire.Sort.Sorts;
using Schick.Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Schick.Plainquire.Sort.Tests.Tests.EntitySort;

[TestClass, ExcludeFromCodeCoverage]
public class SortConfigurationTests
{
    [TestMethod]
    public void WhenConfigurationIsCreated_ThenDefaultValuesAreSet()
    {
        var configuration = new SortConfiguration();

        configuration.Should().BeEquivalentTo(new SortConfiguration
        {
            UseConditionalAccess = ConditionalAccess.WhenEnumerableQuery
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
            UseConditionalAccess = ConditionalAccess.Never
        };

        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject!.Value);

        Action sortEnumerableWithNullValues = () => testItems.OrderBy(sort, configuration).ToList();

        sortEnumerableWithNullValues.Should().Throw<NullReferenceException>();
    }

    [TestMethod]
    public void WhenConditionalAccessIsSetToAlways_ConditionalAccessGeneratedForProvidersOtherThanEnumerableQuery()
    {
        var configuration = new SortConfiguration
        {
            UseConditionalAccess = ConditionalAccess.Always
        };

        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject!.Value);

        using var dbContext = new TestDbContext<TestModel<string>>(useSqlite: true);

        var queryable = dbContext
            .Set<TestModel<string>>()
            .OrderBy(sort, configuration);

        queryable.Expression.ToString()
            .Should()
            .Contain(".OrderBy(x => IIF((IIF((x == null), null, x.NestedObject) == null), null, IIF((x == null), null, x.NestedObject).Value))");
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

    [TestMethod]
    [SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Required to execute sorting")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Required to execute sorting")]
    public void WhenConfigurationIsSetViaStaticDefault_ConfigurationIsUsed()
    {
        Sorts.EntitySort.DefaultConfiguration = new SortConfiguration
        {
            UseConditionalAccess = ConditionalAccess.Always
        };

        var sort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject!.Value);

        using var dbContext = new TestDbContext<TestModel<string>>(useSqlite: true);
        var queryable = dbContext
            .Set<TestModel<string>>()
            .OrderBy(sort);

        queryable.Expression.ToString()
            .Should()
            .Contain(".OrderBy(x => IIF((IIF((x == null), null, x.NestedObject) == null), null, IIF((x == null), null, x.NestedObject).Value))");
    }
}
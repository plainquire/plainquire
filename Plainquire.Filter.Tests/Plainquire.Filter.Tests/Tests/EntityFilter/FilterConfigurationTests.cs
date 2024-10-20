using FluentAssertions;
using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Models;
using Plainquire.TestSupport.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Filter.Tests.Tests.EntityFilter;

[TestFixture]
public class FilterConfigurationTests : TestContainer
{
    [Test]
    public void WhenConfigurationIsCreated_ThenDefaultValuesAreSet()
    {
        var configuration = new FilterConfiguration();

        configuration.Should().BeEquivalentTo(new FilterConfiguration
        {
            UseConditionalAccess = FilterConditionalAccess.WhenCompiled
        });
    }

    [Test]
    [SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Required to execute sorting")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Required to execute sorting")]
    public void WhenConditionalAccessIsSetToNever_CallAgainstEnumerableWithNullValuesThrows()
    {
        var configuration = new FilterConfiguration
        {
            UseConditionalAccess = FilterConditionalAccess.Never
        };

        var nestedFilter = new EntityFilter<TestModelNested>(configuration)
            .Replace(x => x.Value, "=NestedA");

        var outerFilter = new EntityFilter<TestModel<string>>(configuration)
            .Replace(x => x.ValueA, "=OuterA")
            .ReplaceNested(x => x.NestedList, nestedFilter);

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "OuterA", NestedList = null },
        };

        Action filterEnumerableWithNullValues = () => testItems.Where(outerFilter).ToList();

        filterEnumerableWithNullValues.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void WhenConditionalAccessIsSetToAlways_ConditionalAccessGeneratedForProvidersOtherThanEnumerableQuery()
    {
        var configuration = new FilterConfiguration
        {
            UseConditionalAccess = FilterConditionalAccess.Always
        };

        var nestedFilter = new EntityFilter<TestModelNested>(configuration)
            .Replace(x => x.Value, "=NestedA");

        var outerFilter = new EntityFilter<TestModel<string>>(configuration)
            .Replace(x => x.ValueA, "=OuterA")
            .ReplaceNested(x => x.NestedList, nestedFilter);

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "OuterA", NestedList = null },
        };

        var queryable = testItems.AsQueryable().Where(outerFilter);

        queryable.Expression.ToString()
            .Should()
            .Contain("(x.NestedList != null)");
    }
}
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Plainquire.Filter.Tests.Tests.EntityFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterConfigurationTests
{
    [TestMethod]
    public void WhenConfigurationIsCreated_ThenDefaultValuesAreSet()
    {
        var configuration = new FilterConfiguration();
        configuration.Should().BeEquivalentTo(new FilterConfiguration { CultureInfo = CultureInfo.CurrentCulture });
    }

    [DataTestMethod, DoNotParallelize]
    [FilterFuncDataSource<TestModel<DateTime>>]
    public void WhenDefaultConfigurationIsSet_ConfigurationIsUsed(EntityFilterFunc<TestModel<DateTime>> filterFunc)
    {
        var filter = new EntityFilter<TestModel<DateTime>>()
            .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, "InvalidTimeTimeSyntax");

        var configuration = new FilterConfiguration { IgnoreParseExceptions = true };
        Filter.EntityFilter.DefaultFilterConfiguration = configuration;
        var filteredItems = () => filterFunc([], filter);

        filteredItems.Should().NotThrow();

        // Cleanup
        Filter.EntityFilter.DefaultFilterConfiguration = new FilterConfiguration();
    }
}
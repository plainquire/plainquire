using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions.Configurations;
using Plainquire.Filter.Enums;
using Plainquire.Filter.Extensions;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;

namespace Plainquire.Filter.Tests.Tests.EntityFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterConfigurationTests
{
    [TestMethod]
    public void WhenConfigurationIsCreated_ThenDefaultValuesAreSet()
    {
        var configuration = new FilterConfiguration();

        configuration.Should()
            .BeEquivalentTo(new FilterConfiguration
            {
                CultureInfo = CultureInfo.CurrentCulture,
                BoolFalseStrings = ["NO", "0"],
                BoolTrueStrings = ["YES", "1"],
            });
    }

    [DataTestMethod, DoNotParallelize]
    [FilterFuncDataSource<TestModel<DateTime>>]
    public void WhenDefaultConfigurationIsSet_ConfigurationIsUsed(EntityFilterFunc<TestModel<DateTime>> filterFunc)
    {
        var filter = new Filters.EntityFilter<TestModel<DateTime>>()
            .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, "InvalidTimeTimeSyntax");

        var configuration = new FilterConfiguration { IgnoreParseExceptions = true };
        Filters.EntityFilter.DefaultConfiguration = configuration;
        var filteredItems = () => filterFunc([], filter);

        filteredItems.Should().NotThrow();

        // Cleanup
        Filters.EntityFilter.DefaultConfiguration = new FilterConfiguration();
    }
}
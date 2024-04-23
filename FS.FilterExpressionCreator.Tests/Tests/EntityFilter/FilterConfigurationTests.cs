using FluentAssertions;
using FS.FilterExpressionCreator.Abstractions.Configurations;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilter;

[TestClass, ExcludeFromCodeCoverage]
public class FilterConfigurationTests : TestBase
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
    [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<DateTime>))]
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
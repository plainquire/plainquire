using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Mvc.ModelBinders;
using Plainquire.Filter.Tests.Models;
using Plainquire.TestSupport.Extensions;
using Plainquire.TestSupport.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Filter.Tests.Tests.Configuration;

[TestFixture]
public class ConfigurationPrecedenceTests : TestContainer
{
    private static readonly FilterConfiguration _unusedConfiguration = new();
    private static readonly FilterConfiguration _usedConfiguration = new()
    {
        IgnoreParseExceptions = true,
        FilterOperatorMap = { ["="] = FilterOperator.Contains }
    };

    [Test]
    [NonParallelizable]
    public async Task WhenEntityFilterPrototypeIsRegisteredInDI_ConfigurationsFromPrototypeIsUsedAsFirstPrecedence()
    {
        // Arrange
        var configuredTestFilter = new EntityFilter<TestModel<DateTime>>(_usedConfiguration);
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(EntityFilter<TestModel<DateTime>>))).Returns(configuredTestFilter);
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<FilterConfiguration>))).Returns(Options.Create(_unusedConfiguration));
        FilterConfiguration.Default = _unusedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["TestModelDateTimeValueA"] = "=Hello" };

        var binder = new EntityFilterModelBinder();
        const string actionName = nameof(EntityFilterController.SingleFilter);
        var filterBindingContext = BindingExtensions.CreateBindingContext<EntityFilterController>(actionName, "testModel", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(filterBindingContext);

        // Assert
        using var _ = new AssertionScope();

        filterBindingContext.Result.IsModelSet.Should().BeTrue();

        var filter = (EntityFilter<TestModel<DateTime>>)filterBindingContext.Result.Model!;
        filter.Configuration.Should().Be(_usedConfiguration);

        filter.PropertyFilters.Single().ValueFilters.Single().Operator.Should().Be(FilterOperator.Contains);

        var createFilter = () => filter.CreateFilter();
        createFilter.Should().NotThrow();

        // Cleanup
        FilterConfiguration.Default = null;
    }

    [Test]
    [NonParallelizable]
    public async Task WhenEntityFilterConfigurationIsRegisteredViaDI_ConfigurationsFromDIIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<FilterConfiguration>))).Returns(Options.Create(_usedConfiguration));
        FilterConfiguration.Default = _unusedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["TestModelDateTimeValueA"] = "=Hello" };

        var binder = new EntityFilterModelBinder();
        const string actionName = nameof(EntityFilterController.SingleFilter);
        var filterBindingContext = BindingExtensions.CreateBindingContext<EntityFilterController>(actionName, "testModel", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(filterBindingContext);

        // Assert
        using var _ = new AssertionScope();

        filterBindingContext.Result.IsModelSet.Should().BeTrue();

        var filter = (EntityFilter<TestModel<DateTime>>)filterBindingContext.Result.Model!;
        filter.Configuration.Should().Be(_usedConfiguration);

        filter.PropertyFilters.Single().ValueFilters.Single().Operator.Should().Be(FilterOperator.Contains);

        var createFilter = () => filter.CreateFilter();
        createFilter.Should().NotThrow();

        // Cleanup
        FilterConfiguration.Default = null;
    }

    [Test]
    [NonParallelizable]
    public async Task WhenDefaultFilterConfigurationIsSet_StaticDefaultConfigurationsIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        FilterConfiguration.Default = _usedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["TestModelDateTimeValueA"] = "=Hello" };

        var binder = new EntityFilterModelBinder();
        const string actionName = nameof(EntityFilterController.SingleFilter);
        var filterBindingContext = BindingExtensions.CreateBindingContext<EntityFilterController>(actionName, "testModel", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(filterBindingContext);

        // Assert
        using var _ = new AssertionScope();

        filterBindingContext.Result.IsModelSet.Should().BeTrue();

        var filter = (EntityFilter<TestModel<DateTime>>)filterBindingContext.Result.Model!;
        filter.Configuration.Should().BeNull();

        filter.PropertyFilters.Single().ValueFilters.Single().Operator.Should().Be(FilterOperator.Contains);

        var createFilter = () => filter.CreateFilter();
        createFilter.Should().NotThrow();

        // Cleanup
        FilterConfiguration.Default = null;
    }

    [Test]
    [NonParallelizable]
    public async Task WhenNoConfigurationIsSet_DefaultConfigurationsIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["TestModelDateTimeValueA"] = "=Hello" };

        var binder = new EntityFilterModelBinder();
        const string actionName = nameof(EntityFilterController.SingleFilter);
        var filterBindingContext = BindingExtensions.CreateBindingContext<EntityFilterController>(actionName, "testModel", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(filterBindingContext);

        // Assert
        using var _ = new AssertionScope();

        filterBindingContext.Result.IsModelSet.Should().BeTrue();

        var filter = (EntityFilter<TestModel<DateTime>>)filterBindingContext.Result.Model!;
        filter.Configuration.Should().BeNull();

        filter.PropertyFilters.Single().ValueFilters.Single().Operator.Should().Be(FilterOperator.EqualCaseInsensitive);

        var createFilter = () => filter.CreateFilter();
        createFilter.Should().Throw<FilterExpressionException>();
    }
}
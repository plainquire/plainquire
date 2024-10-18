using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Mvc.ModelBinders;
using Plainquire.Sort.Tests.Models;
using Plainquire.TestSupport.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Sort.Tests.Tests.Configuration;

[TestFixture]
public class ConfigurationPrecedenceTests
{
    private static readonly SortConfiguration _unusedConfiguration = new();
    private static readonly SortConfiguration _usedConfiguration = new()
    {
        IgnoreParseExceptions = true,
        DescendingPrefixes = ["--"]
    };

    [Test]
    [NonParallelizable]
    public async Task WhenEntitySortPrototypeIsRegisteredInDI_ConfigurationsFromPrototypeIsUsedAsFirstPrecedence()
    {
        var configuredTestSort = new EntitySort<TestModel<string>>(_usedConfiguration);

        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestModel<string>>))).Returns(configuredTestSort);
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<SortConfiguration>))).Returns(Options.Create(_unusedConfiguration));
        SortConfiguration.Default = _unusedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "--TestModelStringValue.NotExists" };

        var binder = new EntitySortModelBinder();
        const string actionName = nameof(EntitySortNameController.SingleSort);
        var sortBindingContext = BindingExtensions.CreateBindingContext<EntitySortNameController>(actionName, "orderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(sortBindingContext);

        // Assert
        using var _ = new AssertionScope();

        sortBindingContext.Result.IsModelSet.Should().BeTrue();
        var sort = (EntitySort<TestModel<string>>)sortBindingContext.Result.Model!;
        sort.Configuration.Should().Be(_usedConfiguration);

        sort.PropertySorts.Single().Direction.Should().Be(SortDirection.Descending);
        sort.PropertySorts.Single().PropertyPath.Should().Be("Value.NotExists");

        var sortFunc = () => new List<TestModel<string>>().OrderBy(sort);
        sortFunc.Should().NotThrow();

        // Cleanup
        SortConfiguration.Default = null;
    }

    [Test]
    [NonParallelizable]
    public async Task WhenEntitySortConfigurationIsRegisteredViaDI_ConfigurationsFromDIIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<SortConfiguration>))).Returns(Options.Create(_usedConfiguration));
        SortConfiguration.Default = _unusedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "--TestModelStringValue.NotExists" };

        var binder = new EntitySortModelBinder();
        const string actionName = nameof(EntitySortNameController.SingleSort);
        var sortBindingContext = BindingExtensions.CreateBindingContext<EntitySortNameController>(actionName, "orderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(sortBindingContext);

        // Assert
        using var _ = new AssertionScope();

        sortBindingContext.Result.IsModelSet.Should().BeTrue();
        var sort = (EntitySort<TestModel<string>>)sortBindingContext.Result.Model!;
        sort.Configuration.Should().Be(_usedConfiguration);

        sort.PropertySorts.Single().Direction.Should().Be(SortDirection.Descending);
        sort.PropertySorts.Single().PropertyPath.Should().Be("Value.NotExists");

        var sortFunc = () => new List<TestModel<string>>().OrderBy(sort);
        sortFunc.Should().NotThrow();

        // Cleanup
        SortConfiguration.Default = null;
    }

    [Test]
    [NonParallelizable]
    public async Task WhenStaticDefaultSortConfigurationIsSet_StaticDefaultConfigurationsIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        SortConfiguration.Default = _usedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "--TestModelStringValue.NotExists" };

        var binder = new EntitySortModelBinder();
        const string actionName = nameof(EntitySortNameController.SingleSort);
        var sortBindingContext = BindingExtensions.CreateBindingContext<EntitySortNameController>(actionName, "orderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(sortBindingContext);

        // Assert
        using var _ = new AssertionScope();

        sortBindingContext.Result.IsModelSet.Should().BeTrue();

        var sort = (EntitySort<TestModel<string>>)sortBindingContext.Result.Model!;
        sort.Configuration.Should().BeNull();

        sort.PropertySorts.Single().Direction.Should().Be(SortDirection.Descending);
        sort.PropertySorts.Single().PropertyPath.Should().Be("Value.NotExists");

        var sortFunc = () => new List<TestModel<string>>().OrderBy(sort);
        sortFunc.Should().NotThrow();

        // Cleanup
        SortConfiguration.Default = null;
    }

    [Test]
    public async Task WhenNoConfigurationIsSet_DefaultConfigurationsIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "-TestModelStringValue.NotExists" };

        var binder = new EntitySortModelBinder();
        const string actionName = nameof(EntitySortNameController.SingleSort);
        var sortBindingContext = BindingExtensions.CreateBindingContext<EntitySortNameController>(actionName, "orderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(sortBindingContext);

        // Assert
        using var _ = new AssertionScope();

        sortBindingContext.Result.IsModelSet.Should().BeTrue();

        var sort = (EntitySort<TestModel<string>>)sortBindingContext.Result.Model!;
        sort.Configuration.Should().BeNull();

        sort.PropertySorts.Single().Direction.Should().Be(SortDirection.Descending);
        sort.PropertySorts.Single().PropertyPath.Should().Be("Value.NotExists");

        var sortFunc = () => new List<TestModel<string>>().OrderBy(sort);
        sortFunc.Should().Throw<ArgumentException>();
    }
}
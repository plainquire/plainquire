using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Abstractions;
using Plainquire.Page.Mvc.ModelBinders;
using Plainquire.Page.Tests.Models;
using Plainquire.TestSupport.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Plainquire.Page.Tests.Tests.Configuration;

[TestClass]
public class ConfigurationPrecedenceTests
{
    private static readonly PageConfiguration _unusedConfiguration = new();
    private static readonly PageConfiguration _usedConfiguration = new()
    {
        IgnoreParseExceptions = true,
    };

    [TestMethod]
    [DoNotParallelize]
    public async Task WhenEntityPagePrototypeIsRegisteredInDI_ConfigurationsFromPrototypeIsUsedAsFirstPrecedence()
    {
        var configuredTestSort = new EntityPage<TestAddress> { Configuration = _usedConfiguration };

        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(EntityPage<TestAddress>))).Returns(configuredTestSort);
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<PageConfiguration>))).Returns(Options.Create(_unusedConfiguration));
        PageConfiguration.Default = _unusedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["page"] = "2", ["pageSize"] = "N/A" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.PageSizeByFilterAttribute);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (EntityPage<TestAddress>)pageBindingContext.Result.Model!;
        page.Configuration.Should().Be(_usedConfiguration);

        page.PageNumberValue.Should().Be("2");
        page.PageSizeValue.Should().Be("N/A");

        var sortFunc = () => new List<TestModel<string>>().Page(page);
        sortFunc.Should().NotThrow();

        // Cleanup
        PageConfiguration.Default = null;
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task WhenEntityPageConfigurationIsRegisteredViaDI_ConfigurationsFromDIIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<PageConfiguration>))).Returns(Options.Create(_usedConfiguration));
        PageConfiguration.Default = _unusedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["page"] = "2", ["pageSize"] = "N/A" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.PageSizeByFilterAttribute);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (EntityPage<TestAddress>)pageBindingContext.Result.Model!;
        page.Configuration.Should().Be(_usedConfiguration);

        page.PageNumberValue.Should().Be("2");
        page.PageSizeValue.Should().Be("N/A");

        var sortFunc = () => new List<TestModel<string>>().Page(page);
        sortFunc.Should().NotThrow();

        // Cleanup
        PageConfiguration.Default = null;
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task WhenDefaultPageConfigurationIsSet_StaticDefaultConfigurationsIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        PageConfiguration.Default = _usedConfiguration;

        var queryParameters = new Dictionary<string, string> { ["page"] = "2", ["pageSize"] = "N/A" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.PageSizeByFilterAttribute);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (EntityPage<TestAddress>)pageBindingContext.Result.Model!;
        page.Configuration.Should().BeNull();

        page.PageNumberValue.Should().Be("2");
        page.PageSizeValue.Should().Be("N/A");

        var sortFunc = () => new List<TestModel<string>>().Page(page);
        sortFunc.Should().NotThrow();

        // Cleanup
        PageConfiguration.Default = null;
    }

    [TestMethod]
    public async Task WhenNoConfigurationIsSet_DefaultConfigurationsIsUsedAsFirstPrecedence()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["page"] = "2", ["pageSize"] = "N/A" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.PageSizeByFilterAttribute);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (EntityPage<TestAddress>)pageBindingContext.Result.Model!;
        page.Configuration.Should().BeNull();

        page.PageNumberValue.Should().Be("2");
        page.PageSizeValue.Should().Be("N/A");

        var sortFunc = () => new List<TestModel<string>>().Page(page);
        sortFunc.Should().Throw<FormatException>();
    }
}
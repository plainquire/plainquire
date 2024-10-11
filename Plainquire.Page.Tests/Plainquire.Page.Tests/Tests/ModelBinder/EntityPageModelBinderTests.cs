using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Mvc.ModelBinders;
using Plainquire.Page.Tests.Models;
using Plainquire.TestSupport.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Plainquire.Page.Tests.Tests.ModelBinder;

[TestClass]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityPageModelBinderTests
{
    [TestMethod]
    public async Task WhenUnnamedPageParameterIsGiven_PageParameterMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["page"] = "1", ["pageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.ParameterUnnamed);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WhenNumberNamedPageParameterIsGiven_PageParameterMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["defaultPage"] = "1", ["defaultPageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.ParameterNumberNamed);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WheSizeNamedPageParameterIsGiven_PageParameterMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["page"] = "1", ["myPageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.ParameterSizeNamed);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WhenBothNamedPageParameterIsGiven_PageParameterMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["defaultPage"] = "1", ["myPageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.ParameterBothNamed);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WhenMixedNamedPageParametersAreGiven_PageParameterMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["page1"] = "1", ["page2"] = "2", ["pageSize"] = "3" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.ParameterMixedNamed);
        var page1BindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page1", queryParameters, serviceProvider);
        var page2BindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page2", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(page1BindingContext);
        await binder.BindModelAsync(page2BindingContext);

        // Assert
        using var _ = new AssertionScope();

        page1BindingContext.Result.IsModelSet.Should().BeTrue();
        page2BindingContext.Result.IsModelSet.Should().BeTrue();

        var page1 = (Page.EntityPage)page1BindingContext.Result.Model!;
        page1.PageNumber.Should().Be(1);
        page1.PageSize.Should().Be(3);

        var page2 = (Page.EntityPage)page2BindingContext.Result.Model!;
        page2.PageNumber.Should().Be(2);
        page2.PageSize.Should().Be(3);
    }

    [TestMethod]
    public async Task WhenPageSizeSetByFilterAttribute_PageParameterMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["page"] = "1" };

        var binder = new EntityPageModelBinder();
        const string actionName = nameof(EntityPageNameController.PageSizeByFilterAttribute);
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(10);
    }

    [TestMethod]
    public async Task WhenUnnamedPagePropertyIsGiven_PagePropertyMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["pageUnnamed"] = "1", ["pageUnnamedSize"] = "2" };

        var binder = new EntityPageModelBinder();
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNamePageModel>("PageUnnamed", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WhenNumberNamedPagePropertyIsGiven_PagePropertyMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["defaultPage"] = "1", ["defaultPageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNamePageModel>("PageNumberNamed", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WheSizeNamedPagePropertyIsGiven_PagePropertyMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["pageSizeNamed"] = "1", ["MyPageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNamePageModel>("PageSizeNamed", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }

    [TestMethod]
    public async Task WhenBothNamedPagePropertyIsGiven_PagePropertyMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["defaultPage"] = "1", ["myPageSize"] = "2" };

        var binder = new EntityPageModelBinder();
        var pageBindingContext = BindingExtensions.CreateBindingContext<EntityPageNamePageModel>("PageBothNamed", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(2);
    }
}
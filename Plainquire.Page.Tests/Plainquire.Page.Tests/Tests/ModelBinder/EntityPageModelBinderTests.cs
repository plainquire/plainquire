using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Mvc.ModelBinders;
using Plainquire.Page.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Plainquire.Page.Tests.Tests.ModelBinder;

[TestClass, ExcludeFromCodeCoverage]
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
        var pageBindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

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
        var pageBindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

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
        var pageBindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

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
        var pageBindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

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
        var page1BindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page1", queryParameters, serviceProvider);
        var page2BindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page2", queryParameters, serviceProvider);

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
        var pageBindingContext = CreateBindingContext<EntityPageNameController>(actionName, "page", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(pageBindingContext);

        // Assert
        using var _ = new AssertionScope();

        pageBindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Page.EntityPage)pageBindingContext.Result.Model!;
        page.PageNumber.Should().Be(1);
        page.PageSize.Should().Be(10);
    }

    private static ModelBindingContext CreateBindingContext<TController>(string actionName, string parameterName, Dictionary<string, string> queryParameters, IServiceProvider serviceProvider)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request = { QueryString = new QueryBuilder(queryParameters).ToQueryString() },
                RequestServices = serviceProvider
            }
        };

        var bindingSource = new BindingSource("Query", "Query", false, true);
        var routeValueDictionary = new RouteValueDictionary(queryParameters!);
        var valueProvider = new RouteValueProvider(bindingSource, routeValueDictionary);

        var parameterInfo = typeof(TController)
            .GetMethod(actionName)?
            .GetParameters()
            .FirstOrDefault(parameter => parameter.Name == parameterName)
            ?? throw new ArgumentException("Method or parameter not found", nameof(actionName));

        var modelMetadata = (DefaultModelMetadata)new EmptyModelMetadataProvider().GetMetadataForParameter(parameterInfo, parameterInfo.ParameterType);
        var binderModelName = parameterInfo.GetCustomAttribute<FromQueryAttribute>()?.Name;

        var bindingContext = DefaultModelBindingContext
            .CreateBindingContext(
                actionContext,
                valueProvider,
                modelMetadata,
                bindingInfo: null,
                binderModelName ?? parameterInfo.Name ?? throw new InvalidOperationException("Parameter name not found")
            );

        return bindingContext;
    }
}
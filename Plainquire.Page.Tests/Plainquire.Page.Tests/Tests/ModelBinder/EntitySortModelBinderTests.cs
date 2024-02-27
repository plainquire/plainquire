using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions.Attributes;
using Plainquire.Page.Mvc.ModelBinders;
using Plainquire.Page.Pages;

namespace Plainquire.Page.Tests.Tests.ModelBinder;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityPageModelBinderTests
{
    [FilterEntity(PageSize = 4)]
    private record Person(string Name);

    [TestMethod]
    public void WhenQueryParametersAreParsedForTypedPage_EntityPageMatchesGivenParameters()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string>
        {
            ["page"] = "2",
            ["pageSize"] = "3",
        };

        var bindingContext = CreateBindingContext<EntityPage<Person>>(queryParameters);
        var binder = new EntityPageModelBinder();

        // Act
        binder.BindModelAsync(bindingContext);

        // Assert
        using var _ = new AssertionScope();

        bindingContext.Result.IsModelSet.Should().BeTrue();

        var personPage = (EntityPage<Person>)bindingContext.Result.Model!;

        personPage.PageNumber.Should().Be(2);
        personPage.PageSize.Should().Be(3);
    }

    [TestMethod]
    public void WhenQueryParameterMissing_EntityPageSizeIsTakenFromAttribute()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string>
        {
            ["page"] = "2",
        };

        var bindingContext = CreateBindingContext<EntityPage<Person>>(queryParameters);
        var binder = new EntityPageModelBinder();

        // Act
        binder.BindModelAsync(bindingContext);

        // Assert
        using var _ = new AssertionScope();

        bindingContext.Result.IsModelSet.Should().BeTrue();

        var personPage = (EntityPage<Person>)bindingContext.Result.Model!;

        personPage.PageNumber.Should().Be(2);
        personPage.PageSize.Should().Be(4);
    }

    [TestMethod]
    public void WhenQueryParametersAreParsedForUntypedPage_EntityPageMatchesGivenParameters()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string>
        {
            ["page"] = "2",
            ["pageSize"] = "3",
        };

        var bindingContext = CreateBindingContext<Pages.EntityPage>(queryParameters);
        var binder = new EntityPageModelBinder();

        // Act
        binder.BindModelAsync(bindingContext);

        // Assert
        using var _ = new AssertionScope();

        bindingContext.Result.IsModelSet.Should().BeTrue();

        var page = (Pages.EntityPage)bindingContext.Result.Model!;

        page.PageNumber.Should().Be(2);
        page.PageSize.Should().Be(3);
    }

    [FilterEntity(PageNumberParameter = "pageNumber", PageSizeParameter = "size")]
    private record PersonWithCustomParameters(string Name);

    [TestMethod]
    public void WhenCustomQueryParametersAreParsed_EntityPageMatchesGivenParameters()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string>
        {
            ["pageNumber"] = "2",
            ["size"] = "3",
        };

        var bindingContext = CreateBindingContext<EntityPage<PersonWithCustomParameters>>(queryParameters);
        var binder = new EntityPageModelBinder();

        // Act
        binder.BindModelAsync(bindingContext);

        // Assert
        using var _ = new AssertionScope();

        bindingContext.Result.IsModelSet.Should().BeTrue();

        var personPage = (EntityPage<PersonWithCustomParameters>)bindingContext.Result.Model!;

        personPage.PageNumber.Should().Be(2);
        personPage.PageSize.Should().Be(3);
    }

    private static DefaultModelBindingContext CreateBindingContext<TModel>(Dictionary<string, string> queryParameters)
    {
        var modelType = typeof(TModel);

        var bindingSource = new BindingSource("", "", false, false);
        var routeValueDictionary = new RouteValueDictionary(queryParameters!);
        var valueProvider = new RouteValueProvider(bindingSource, routeValueDictionary);

        var bindingContext = new DefaultModelBindingContext
        {
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(modelType),
            ModelName = modelType.Name,
            ModelState = new ModelStateDictionary(),
            ValueProvider = valueProvider,
            ActionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { QueryString = new QueryBuilder(queryParameters).ToQueryString() }
                }
            }
        };

        return bindingContext;
    }
}
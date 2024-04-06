using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Mvc.ModelBinders;
using Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.ModelBinder;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntitySortModelBinderTests
{
    [DataTestMethod]
    [DataRow(nameof(EntitySortController.SingleSort))]
    [DataRow(nameof(EntitySortController.SingleSortAtStart))]
    [DataRow(nameof(EntitySortController.SingleSortAtEnd))]
    [DataRow(nameof(EntitySortController.SingleSortBetween))]
    public void WhenSingleEntitySortIsGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = GetQueryParameters<EntitySortController>(actionName, serviceProvider);
        queryParameters["orderBy"] = "fullname, birthday";

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySort<TestPerson>>(queryParameters, serviceProvider);

        // Act
        binder.BindModelAsync(personBindingContext);

        // Assert
        using var _ = new AssertionScope();

        personBindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<TestPerson>)personBindingContext.Result.Model!;
        personSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Name", "Birthday");
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortController.MultipleSort))]
    [DataRow(nameof(EntitySortController.MultipleSortAtStart))]
    [DataRow(nameof(EntitySortController.MultipleSortAtEnd))]
    [DataRow(nameof(EntitySortController.MultipleSortBetween))]
    [DataRow(nameof(EntitySortController.MultipleSortAround))]
    [DataRow(nameof(EntitySortController.MultipleSortSpread))]
    public void WhenMultipleEntitySortWithSingleConfigurationAreGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var sortByConfiguration = new SortConfiguration { HttpQueryParameterName = "sortBy" };
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<SortConfiguration>))).Returns(Options.Create(sortByConfiguration));

        var queryParameters = GetQueryParameters<EntitySortController>(actionName, serviceProvider);
        queryParameters["sortBy"] = "fullname, birthday, addressStreet";

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySort<TestPerson>>(queryParameters, serviceProvider);
        var addressBindingContext = CreateBindingContext<EntitySort<TestAddress>>(queryParameters, serviceProvider);

        // Act
        binder.BindModelAsync(personBindingContext);
        binder.BindModelAsync(addressBindingContext);

        // Assert
        using var _ = new AssertionScope();

        personBindingContext.Result.IsModelSet.Should().BeTrue();
        addressBindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<TestPerson>)personBindingContext.Result.Model!;
        var addressSort = (EntitySort<TestAddress>)addressBindingContext.Result.Model!;
        var combinedSort = personSort.AddNested(x => x.Address, addressSort);

        combinedSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Name", "Birthday", "Address.Street");
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortController.MultipleSort))]
    [DataRow(nameof(EntitySortController.MultipleSortAtStart))]
    [DataRow(nameof(EntitySortController.MultipleSortAtEnd))]
    [DataRow(nameof(EntitySortController.MultipleSortBetween))]
    [DataRow(nameof(EntitySortController.MultipleSortAround))]
    [DataRow(nameof(EntitySortController.MultipleSortSpread))]
    public void WhenMultipleEntitySortWithMultipleConfigurationAreGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var sortByConfiguration = new SortConfiguration { HttpQueryParameterName = "sortBy" };
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestAddress>))).Returns(new EntitySort<TestAddress> { Configuration = sortByConfiguration });

        var queryParameters = GetQueryParameters<EntitySortController>(actionName, serviceProvider);
        queryParameters["orderBy"] = "fullname, birthday";
        queryParameters["sortBy"] = "addressStreet";

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySort<TestPerson>>(queryParameters, serviceProvider);
        var addressBindingContext = CreateBindingContext<EntitySort<TestAddress>>(queryParameters, serviceProvider);

        // Act
        binder.BindModelAsync(personBindingContext);
        binder.BindModelAsync(addressBindingContext);

        // Assert
        using var _ = new AssertionScope();

        personBindingContext.Result.IsModelSet.Should().BeTrue();
        addressBindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<TestPerson>)personBindingContext.Result.Model!;
        personSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Name", "Birthday");

        var addressSort = (EntitySort<TestAddress>)addressBindingContext.Result.Model!;
        addressSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Street");
    }

    private static Dictionary<string, string> GetQueryParameters<TController>(string actionName, IServiceProvider serviceProvider)
    {
        var methodInfo = typeof(TController).GetMethod(actionName)
            ?? throw new ArgumentException("Method not found", nameof(actionName));

        var queryParameters = methodInfo
            .GetParameters()
            .Select(parameter =>
            {
                if (!parameter.ParameterType.IsGenericEntitySort())
                    return parameter.Name!;

                var entitySortConfiguration = ((Sort.EntitySort?)serviceProvider.GetService(parameter.ParameterType))?.Configuration;
                var defaultConfiguration = serviceProvider.GetService<IOptions<SortConfiguration>>()?.Value;
                var configuration = entitySortConfiguration ?? defaultConfiguration ?? new SortConfiguration();
                return configuration.HttpQueryParameterName;
            })
            .Distinct()
            .ToDictionary(parameterName => parameterName, _ => "");

        return queryParameters;
    }

    private static DefaultModelBindingContext CreateBindingContext<TModel>(Dictionary<string, string> queryParameters, IServiceProvider serviceProvider)
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
                    Request = { QueryString = new QueryBuilder(queryParameters).ToQueryString() },
                    RequestServices = serviceProvider
                }
            }
        };

        return bindingContext;
    }
}
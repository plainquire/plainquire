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
using Plainquire.Sort.Mvc.ModelBinders;
using Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Plainquire.Sort.Tests.Tests.ModelBinder;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntitySortModelBinderTests
{
    [DataTestMethod]
    [DataRow(nameof(EntitySortPositionController.SingleSort))]
    [DataRow(nameof(EntitySortPositionController.SingleSortAtStart))]
    [DataRow(nameof(EntitySortPositionController.SingleSortAtEnd))]
    [DataRow(nameof(EntitySortPositionController.SingleSortBetween))]
    public async Task WhenSingleEntitySortIsGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "fullname, birthday" };

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySortPositionController>(actionName, "orderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(personBindingContext);

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
    [DataRow(nameof(EntitySortPositionController.MultipleSortSameParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtStartSameParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtEndSameParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortBetweenSameParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAroundSameParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortSpreadSameParameter))]
    public async Task WhenMultipleEntitySortWithSameParameterNameAreGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "fullname, birthday, addressStreet" };

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySortPositionController>(actionName, "orderBy", queryParameters, serviceProvider);
        var addressBindingContext = CreateBindingContext<EntitySortPositionController>(actionName, "addressOrderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(personBindingContext);
        await binder.BindModelAsync(addressBindingContext);

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
    [DataRow(nameof(EntitySortPositionController.MultipleSortSeparateParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtStartSeparateParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtEndSeparateParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortBetweenSeparateParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAroundSeparateParameter))]
    [DataRow(nameof(EntitySortPositionController.MultipleSortSpreadSeparateParameter))]
    public async Task WhenMultipleEntitySortWithSeparateParameterNamesAreGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string>
        {
            ["orderBy"] = "fullname, birthday",
            ["sortBy"] = "addressStreet"
        };

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySortPositionController>(actionName, "orderBy", queryParameters, serviceProvider);
        var addressBindingContext = CreateBindingContext<EntitySortPositionController>(actionName, "sortBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(personBindingContext);
        await binder.BindModelAsync(addressBindingContext);

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

    private static DefaultModelBindingContext CreateBindingContext<TController>(string actionName, string parameterName, Dictionary<string, string> queryParameters, IServiceProvider serviceProvider)
    {
        var parameterInfo = typeof(TController)
            .GetMethod(actionName)?
            .GetParameters()
            .FirstOrDefault(parameter => parameter.Name == parameterName)
            ?? throw new ArgumentException("Method or parameter not found", nameof(actionName));

        var bindingSource = new BindingSource("Query", "Query", false, true);
        var routeValueDictionary = new RouteValueDictionary(queryParameters!);
        var valueProvider = new RouteValueProvider(bindingSource, routeValueDictionary);

        var modelMetadata = (DefaultModelMetadata)new EmptyModelMetadataProvider().GetMetadataForParameter(parameterInfo, parameterInfo.ParameterType);
        var binderModelName = parameterInfo.GetCustomAttribute<FromQueryAttribute>()?.Name;
        modelMetadata.BindingMetadata.BinderModelName = binderModelName;

        var bindingContext = new DefaultModelBindingContext
        {
            ModelMetadata = modelMetadata,
            ModelName = parameterInfo.Name ?? throw new InvalidOperationException("Parameter name not found"),
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
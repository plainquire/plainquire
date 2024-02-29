using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Abstractions;
using Plainquire.Sort.Mvc.ModelBinders;
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
    [FilterEntity(Prefix = "", SortByParameter = "sortBy")]
    private record PersonWithSortBy(
        [property: Filter(Name = "Fullname")] string Name,
        [property: Filter] DateTime? Birthday,
        [property: Filter(Sortable = true)] Address Address
    );

    [FilterEntity(Prefix = "")]
    private record PersonWithOrderBy(
        [property: Filter(Name = "Fullname")] string Name,
        [property: Filter] DateTime? Birthday,
        [property: Filter] Address Address
    );

    [FilterEntity]
    private record Address(string Street, Country Country);

    [FilterEntity]
    private record Country(string Name);

    [TestMethod]
    public void WhenQueryParametersAreParsed_EntitySortMatchesGivenParameters()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string>
        {
            ["sortBy"] = "fullname, birthday-desc",
            ["sortBy[0]"] = "name, notExists",
            ["sortBy[1]"] = "~address.Street.length",
        };

        var bindingContext = CreateBindingContext<EntitySort<PersonWithSortBy>>(queryParameters);
        var binder = new EntitySortModelBinder();

        // Act
        binder.BindModelAsync(bindingContext);

        // Assert
        using var _ = new AssertionScope();

        bindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<PersonWithSortBy>)bindingContext.Result.Model!;

        personSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => new { propertySort.PropertyPath, propertySort.Direction })
            .Should()
            .ContainInOrder(
                new { PropertyPath = "Name", Direction = SortDirection.Ascending },
                new { PropertyPath = "Birthday", Direction = SortDirection.Descending },
                new { PropertyPath = "Address.Street.length", Direction = SortDirection.Descending }
            );
    }

    [TestMethod]
    public void WhenQueryParametersAreParsedForTwoEntities_FinalOrderMatchesQueryParams()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string>
        {
            ["orderBy"] = "fullname, birthday, addressStreet"
        };

        var binder = new EntitySortModelBinder();
        var personBindingContext = CreateBindingContext<EntitySort<PersonWithOrderBy>>(queryParameters);
        var addressBindingContext = CreateBindingContext<EntitySort<Address>>(queryParameters);

        // Act
        binder.BindModelAsync(personBindingContext);
        binder.BindModelAsync(addressBindingContext);

        // Assert
        using var _ = new AssertionScope();

        personBindingContext.Result.IsModelSet.Should().BeTrue();
        addressBindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<PersonWithOrderBy>)personBindingContext.Result.Model!;
        var addressSort = (EntitySort<Address>)addressBindingContext.Result.Model!;

        var combinedSort = personSort.AddNested(x => x.Address, addressSort);
        combinedSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Name", "Birthday", "Address.Street");
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
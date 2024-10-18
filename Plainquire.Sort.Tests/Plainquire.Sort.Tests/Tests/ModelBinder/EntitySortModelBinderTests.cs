using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Plainquire.Sort.Mvc.ModelBinders;
using Plainquire.Sort.Tests.Models;
using Plainquire.TestSupport.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Sort.Tests.Tests.ModelBinder;

[TestFixture]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntitySortModelBinderTests
{
    [Test]
    public async Task WhenGenericEntitySortParameterIsGiven_ParameterBoundAsExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "TestModelStringValue" };

        var binder = new EntitySortModelBinder();
        const string actionName = nameof(EntitySortNameController.SingleSort);
        var sortBindingContext = BindingExtensions.CreateBindingContext<EntitySortNameController>(actionName, "orderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(sortBindingContext);

        // Assert
        using var _ = new AssertionScope();

        sortBindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<TestModel<string>>)sortBindingContext.Result.Model!;
        personSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Value");
    }
    [Test]
    public async Task WhenGenericEntitySortPageIsGiven_ParameterBoundAsExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "TestModelStringValue" };

        var binder = new EntitySortModelBinder();
        var sortBindingContext = BindingExtensions.CreateBindingContext<EntitySortNamePageModel>("OrderBy", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(sortBindingContext);

        // Assert
        using var _ = new AssertionScope();

        sortBindingContext.Result.IsModelSet.Should().BeTrue();

        var personSort = (EntitySort<TestModel<string>>)sortBindingContext.Result.Model!;
        personSort.PropertySorts
            .OrderBy(propertySort => propertySort.Position)
            .Select(propertySort => propertySort.PropertyPath)
            .Should()
            .ContainInOrder("Value");
    }

    [TestCase(nameof(EntitySortPositionController.SingleSort))]
    [TestCase(nameof(EntitySortPositionController.SingleSortAtStart))]
    [TestCase(nameof(EntitySortPositionController.SingleSortAtEnd))]
    [TestCase(nameof(EntitySortPositionController.SingleSortBetween))]
    public async Task WhenSingleEntitySortIsGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "fullname, birthday" };

        var binder = new EntitySortModelBinder();
        var personBindingContext = BindingExtensions.CreateBindingContext<EntitySortPositionController>(actionName, "orderBy", queryParameters, serviceProvider);

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

    [TestCase(nameof(EntitySortPositionController.MultipleSortSameParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortAtStartSameParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortAtEndSameParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortBetweenSameParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortAroundSameParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortSpreadSameParameter))]
    public async Task WhenMultipleEntitySortWithSameParameterNameAreGiven_FinalOrderMatchesQueryParams(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["orderBy"] = "fullname, birthday, addressStreet" };

        var binder = new EntitySortModelBinder();
        var personBindingContext = BindingExtensions.CreateBindingContext<EntitySortPositionController>(actionName, "orderBy", queryParameters, serviceProvider);
        var addressBindingContext = BindingExtensions.CreateBindingContext<EntitySortPositionController>(actionName, "addressOrderBy", queryParameters, serviceProvider);

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

    [TestCase(nameof(EntitySortPositionController.MultipleSortSeparateParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortAtStartSeparateParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortAtEndSeparateParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortBetweenSeparateParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortAroundSeparateParameter))]
    [TestCase(nameof(EntitySortPositionController.MultipleSortSpreadSeparateParameter))]
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
        var personBindingContext = BindingExtensions.CreateBindingContext<EntitySortPositionController>(actionName, "orderBy", queryParameters, serviceProvider);
        var addressBindingContext = BindingExtensions.CreateBindingContext<EntitySortPositionController>(actionName, "sortBy", queryParameters, serviceProvider);

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
}
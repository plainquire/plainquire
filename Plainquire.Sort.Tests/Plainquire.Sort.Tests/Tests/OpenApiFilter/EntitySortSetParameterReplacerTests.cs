using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Tests.Models;
using Plainquire.Sort.Tests.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.OpenApiFilter;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntitySortSetParameterReplacerTests
{
    [DataTestMethod]
    [DataRow(nameof(EntitySortSetController.SingleSortSet))]
    [DataRow(nameof(EntitySortSetController.SingleSortSetAtStart))]
    [DataRow(nameof(EntitySortSetController.SingleSortSetAtEnd))]
    [DataRow(nameof(EntitySortSetController.SingleSortSetBetween))]
    public void WhenSingleEntitySortSetIsGiven_ItIsReplacedBySingleSortParameter(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        // Act
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortSetController>(actionName, serviceProvider);
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;
        var orderBy = parameters.SingleOrDefault(parameter => parameter.Name == "orderBy")!;
        orderBy.Should().NotBeNull();
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|addressStreet|addressCountry)(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortSetController.SingleSortSet))]
    [DataRow(nameof(EntitySortSetController.SingleSortSetAtStart))]
    [DataRow(nameof(EntitySortSetController.SingleSortSetAtEnd))]
    [DataRow(nameof(EntitySortSetController.SingleSortSetBetween))]
    public void WhenSingleEntitySortSetWithMultipleConfigurationsIsGiven_ItIsReplacedBySingleSortParameter(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestAddress>))).Returns(new EntitySort<TestAddress> { Configuration = new SortConfiguration { HttpQueryParameterName = "sortBy" } });

        // Act
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortSetController>(actionName, serviceProvider);
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        var orderBy = parameters.SingleOrDefault(parameter => parameter.Name == "orderBy")!;
        orderBy.Should().NotBeNull();
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday)(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        var sortBy = parameters.SingleOrDefault(parameter => parameter.Name == "sortBy")!;
        sortBy.Should().NotBeNull();
        sortBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        sortBy.Schema.Type.Should().Be("array");
        sortBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(addressStreet|addressCountry)(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSet), 0)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetAtStart), 0)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetAtEnd), 1)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetBetween), 1)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetAround), 0)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetSpread), 1)]
    public void WhenMixedEntitySortAndSetWithSingleConfigurationAreGiven_TheyAreReplacedBySingleSortParameter(string actionName, int expectedIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<SortConfiguration>))).Returns(Options.Create(new SortConfiguration()));
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestPerson>))).Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestAddress>))).Returns(null);

        // Act
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortSetController>(actionName, serviceProvider);
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters.ToList();

        var orderByIndex = parameters.FindIndex(parameter => parameter.Name == "orderBy");
        orderByIndex.Should().Be(expectedIndex);

        var orderBy = parameters[orderByIndex];
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|addressStreet|addressCountry)(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSet), 0, 1)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetAtStart), 0, 1)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetAtEnd), 1, 2)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetBetween), 1, 2)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetAround), 0, 1)]
    [DataRow(nameof(EntitySortSetController.MixedSortAndSetSpread), 1, 2)]
    public void WhenMixedEntitySortAndSetWithMultipleConfigurationAreGiven_TheyAreReplacedByMultipleSortParameter(string actionName, int expectedOrderByIndex, int expectedSortByIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(typeof(IOptions<SortConfiguration>))).Returns(Options.Create(new SortConfiguration()));
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestPerson>))).Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(EntitySort<TestAddress>))).Returns(new EntitySort<TestAddress> { Configuration = new SortConfiguration { HttpQueryParameterName = "sortBy" } });

        // Act
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortSetController>(actionName, serviceProvider);
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters.ToList();

        var orderByIndex = parameters.FindIndex(parameter => parameter.Name == "orderBy");
        orderByIndex.Should().Be(expectedOrderByIndex);

        var orderBy = parameters[orderByIndex];
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday)(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        var sortByIndex = parameters.FindIndex(parameter => parameter.Name == "sortBy");
        sortByIndex.Should().Be(expectedSortByIndex);

        var sortBy = parameters[sortByIndex];
        sortBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        sortBy.Schema.Type.Should().Be("array");
        sortBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(addressStreet|addressCountry)(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }
}
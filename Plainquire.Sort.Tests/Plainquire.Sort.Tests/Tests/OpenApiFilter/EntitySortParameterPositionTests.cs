using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Swashbuckle.Filters;
using Plainquire.Sort.Tests.Models;
using Plainquire.Swashbuckle.TestSupport.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.OpenApiFilter;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntitySortParameterPositionTests
{
    [DataTestMethod]
    [DataRow(nameof(EntitySortPositionController.SingleSort))]
    [DataRow(nameof(EntitySortPositionController.SingleSortAtStart))]
    [DataRow(nameof(EntitySortPositionController.SingleSortAtEnd))]
    [DataRow(nameof(EntitySortPositionController.SingleSortBetween))]
    public void WhenSingleEntitySortIsGiven_ItIsReplacedBySingleSortParameter(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortPositionController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;
        var orderBy = parameters.SingleOrDefault(parameter => parameter.Name == "orderBy")!;
        orderBy.Should().NotBeNull();
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|address)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortPositionController.MultipleSortSameParameter), 0)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtStartSameParameter), 0)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtEndSameParameter), 1)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortBetweenSameParameter), 1)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAroundSameParameter), 0)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortSpreadSameParameter), 1)]
    public void WhenMultipleEntitySortWithSameParameterNameAreGiven_TheyAreReplacedBySingleSortParameter(string actionName, int expectedIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortPositionController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters.ToList();

        var orderByIndex = parameters.FindIndex(parameter => parameter.Name == "orderBy");
        orderByIndex.Should().Be(expectedIndex);

        var orderBy = parameters[orderByIndex];
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|address|addressStreet|addressCountry)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortPositionController.MultipleSortSeparateParameter), 0, 1)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtStartSeparateParameter), 0, 1)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAtEndSeparateParameter), 1, 2)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortBetweenSeparateParameter), 1, 2)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortAroundSeparateParameter), 0, 2)]
    [DataRow(nameof(EntitySortPositionController.MultipleSortSpreadSeparateParameter), 1, 3)]
    public void WhenMultipleEntitySortWithSeparateParameterNamesAreGiven_TheyAreReplacedByMultipleSortParameter(string actionName, int expectedOrderByIndex, int expectedSortByIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortPositionController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters.ToList();

        var orderByIndex = parameters.FindIndex(parameter => parameter.Name == "orderBy");
        orderByIndex.Should().Be(expectedOrderByIndex);

        var orderBy = parameters[orderByIndex];
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|address)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        var sortByIndex = parameters.FindIndex(parameter => parameter.Name == "sortBy");
        sortByIndex.Should().Be(expectedSortByIndex);

        var sortBy = parameters[sortByIndex];
        sortBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        sortBy.Schema.Type.Should().Be("array");
        sortBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(addressStreet|addressCountry)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    private static List<IOperationFilter> CreateOperationFilters(IServiceProvider serviceProvider)
        =>
        [
            new EntitySortParameterReplacer(serviceProvider),
            new EntitySortSetParameterReplacer(serviceProvider)
        ];
}
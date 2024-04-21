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
public class EntitySortSetParameterPositionTests
{
    [DataTestMethod]
    [DataRow(nameof(EntitySortSetPositionController.SingleSortSet))]
    [DataRow(nameof(EntitySortSetPositionController.SingleSortSetAtStart))]
    [DataRow(nameof(EntitySortSetPositionController.SingleSortSetAtEnd))]
    [DataRow(nameof(EntitySortSetPositionController.SingleSortSetBetween))]
    public void WhenSingleEntitySortSetIsGiven_ItIsReplacedBySingleSortParameter(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortSetPositionController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;
        var orderBy = parameters.SingleOrDefault(parameter => parameter.Name == "orderBy")!;
        orderBy.Should().NotBeNull();
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|address|addressStreet|addressCountry)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntitySortSetPositionController.MixedSortAndSet), 0, 1)]
    [DataRow(nameof(EntitySortSetPositionController.MixedSortAndSetAtStart), 0, 1)]
    [DataRow(nameof(EntitySortSetPositionController.MixedSortAndSetAtEnd), 1, 2)]
    [DataRow(nameof(EntitySortSetPositionController.MixedSortAndSetBetween), 1, 2)]
    [DataRow(nameof(EntitySortSetPositionController.MixedSortAndSetAround), 0, 2)]
    [DataRow(nameof(EntitySortSetPositionController.MixedSortAndSetSpread), 1, 3)]
    public void WhenMixedEntitySortAndSetAreGiven_TheyAreReplacedBySingleSortParameter(string actionName, int expectedOrderByIndex, int expectedSortByIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortSetPositionController>(actionName, operationFilters);

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
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(fullName|birthday|address|addressStreet|addressCountry)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

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
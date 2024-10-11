using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Swashbuckle.Filters;
using Plainquire.Page.Tests.Models;
using Plainquire.Swashbuckle.TestSupport.Extensions;
using Plainquire.Swashbuckle.TestSupport.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.OpenApiFilter;

[TestClass]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityPageSetParameterPositionTests
{
    [DataTestMethod]
    [DataRow(nameof(EntityPageSetPositionController.SingleSortSet))]
    [DataRow(nameof(EntityPageSetPositionController.SingleSortSetAtStart))]
    [DataRow(nameof(EntityPageSetPositionController.SingleSortSetAtEnd))]
    [DataRow(nameof(EntityPageSetPositionController.SingleSortSetBetween))]
    public void WhenSingleEntitySortSetIsGiven_ItIsReplacedBySingleSortParameter(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageSetPositionController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        var page = parameters.SingleOrDefault(parameter => parameter.Name == "page")!;
        page.Should().BePageNumberParameter();

        var pageSize = parameters.SingleOrDefault(parameter => parameter.Name == "pageSize")!;
        pageSize.Should().BePageSizeParameter();

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [DataTestMethod]
    [DataRow(nameof(EntityPageSetPositionController.MixedSortAndSet), 0, 2)]
    [DataRow(nameof(EntityPageSetPositionController.MixedSortAndSetAtStart), 0, 2)]
    [DataRow(nameof(EntityPageSetPositionController.MixedSortAndSetAtEnd), 1, 3)]
    [DataRow(nameof(EntityPageSetPositionController.MixedSortAndSetBetween), 1, 3)]
    [DataRow(nameof(EntityPageSetPositionController.MixedSortAndSetAround), 0, 3)]
    [DataRow(nameof(EntityPageSetPositionController.MixedSortAndSetSpread), 1, 4)]
    public void WhenMixedEntitySortAndSetAreGiven_TheyAreReplacedBySingleSortParameter(string actionName, int expectedOrderByIndex, int expectedSortByIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        // Act
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageSetPositionController>(actionName, operationFilters);
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters.ToList();

        var pageIndex = parameters.FindIndex(parameter => parameter.Name == "page");
        pageIndex.Should().Be(expectedOrderByIndex);
        parameters[pageIndex].Should().BePageNumberParameter();
        parameters[pageIndex + 1].Should().BePageSizeParameter();

        var addressPageByIndex = parameters.FindIndex(parameter => parameter.Name == "addressPage");
        addressPageByIndex.Should().Be(expectedSortByIndex);
        parameters[addressPageByIndex].Should().BePageNumberParameter();
        parameters[addressPageByIndex + 1].Should().BePageSizeParameter();

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    private static List<IOperationFilter> CreateOperationFilters(IServiceProvider serviceProvider)
        =>
        [
            new EntityPageParameterReplacer(serviceProvider),
            new EntityPageSetParameterReplacer(serviceProvider)
        ];
}
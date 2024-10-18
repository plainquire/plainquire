using FakeItEasy;
using FluentAssertions.Execution;
using Microsoft.OpenApi.Models;
using NUnit.Framework;
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

[TestFixture]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityPageParameterPositionTests
{
    [TestCase(nameof(EntityPagePositionController.SinglePage))]
    [TestCase(nameof(EntityPagePositionController.SinglePageAtStart))]
    [TestCase(nameof(EntityPagePositionController.SinglePageAtEnd))]
    [TestCase(nameof(EntityPagePositionController.SinglePageBetween))]
    public void WhenSingleEntitySortIsGiven_ItIsReplacedBySingleSortParameter(string actionName)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPagePositionController>(actionName, operationFilters);

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

    [TestCase(nameof(EntityPagePositionController.MultiplePageSameParameter), 0)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageAtStartSameParameter), 0)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageAtEndSameParameter), 1)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageBetweenSameParameter), 1)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageAroundSameParameter), 0)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageSpreadSameParameter), 1)]
    public void WhenMultipleEntitySortWithSameParameterNameAreGiven_TheyAreReplacedBySingleSortParameter(string actionName, int expectedIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPagePositionController>(actionName, operationFilters);

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

    [TestCase(nameof(EntityPagePositionController.MultiplePageSeparateParameter), 0, 1)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageAtStartSeparateParameter), 0, 1)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageAtEndSeparateParameter), 1, 2)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageBetweenSeparateParameter), 1, 2)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageAroundSeparateParameter), 0, 2)]
    [TestCase(nameof(EntityPagePositionController.MultiplePageSpreadSeparateParameter), 1, 3)]
    public void WhenMultipleEntitySortWithSeparateParameterNamesAreGiven_TheyAreReplacedByMultipleSortParameter(string actionName, int expectedOrderByIndex, int expectedSortByIndex)
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPagePositionController>(actionName, operationFilters);

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

    private static List<IOperationFilter> CreateOperationFilters(IServiceProvider serviceProvider)
        =>
        [
            new EntityPageParameterReplacer(serviceProvider),
        ];
}
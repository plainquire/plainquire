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
public class EntityPageParameterNameTests
{
    [Test]
    public void WhenUnnamedPageParameterIsGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityPageNameController.ParameterUnnamed);
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageNameController>(actionName, operationFilters);

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

    [Test]
    public void WhenNumberNamedPageParameterIsGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityPageNameController.ParameterNumberNamed);
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageNameController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        var page = parameters.SingleOrDefault(parameter => parameter.Name == "defaultPage")!;
        page.Should().BePageNumberParameter();

        var pageSize = parameters.SingleOrDefault(parameter => parameter.Name == "defaultPageSize")!;
        pageSize.Should().BePageSizeParameter();

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [Test]
    public void WheSizeNamedPageParameterIsGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityPageNameController.ParameterSizeNamed);
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageNameController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        var page = parameters.SingleOrDefault(parameter => parameter.Name == "page")!;
        page.Should().BePageNumberParameter();

        var pageSize = parameters.SingleOrDefault(parameter => parameter.Name == "myPageSize")!;
        pageSize.Should().BePageSizeParameter();

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [Test]
    public void WhenBothNamedPageParameterIsGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityPageNameController.ParameterBothNamed);
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageNameController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        var page = parameters.SingleOrDefault(parameter => parameter.Name == "defaultPage")!;
        page.Should().BePageNumberParameter();

        var pageSize = parameters.SingleOrDefault(parameter => parameter.Name == "myPageSize")!;
        pageSize.Should().BePageSizeParameter();

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [Test]
    public void WhenMixedNamedPageParametersAreGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityPageNameController.ParameterMixedNamed);
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityPageNameController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        var page1 = parameters.SingleOrDefault(parameter => parameter.Name == "page1")!;
        page1.Should().BePageNumberParameter();

        var page2 = parameters.SingleOrDefault(parameter => parameter.Name == "page2")!;
        page2.Should().BePageNumberParameter();

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
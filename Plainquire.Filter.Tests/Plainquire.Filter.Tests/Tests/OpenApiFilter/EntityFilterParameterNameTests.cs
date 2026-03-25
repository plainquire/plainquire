using AwesomeAssertions;
using AwesomeAssertions.Execution;
using FakeItEasy;
using NUnit.Framework;
using Plainquire.Filter.Swashbuckle.Filters;
using Plainquire.Filter.Tests.Models;
using Plainquire.Swashbuckle.TestSupport.Services;
using Plainquire.TestSupport.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace Plainquire.Filter.Tests.Tests.OpenApiFilter;

[TestFixture]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityFilterParameterNameTests : TestContainer
{
    [Test]
    public void WhenSingleFilterIsGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityFilterController.SingleFilter);
        var operationFilters = CreateOperationFilters();
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityFilterController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations?[HttpMethod.Get].Parameters;

        parameters.Should().NotBeNull();

        parameters
            .Select(x => x.Name)
            .Should()
            .Equal(
                "testModelDateTimeId",
                "testModelDateTimeValueA",
                "testModelDateTimeValueB",
                "testModelDateTimeValueC"
            );

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    [Test]
    public void WhenMultipleFiltersAreGiven_GeneratedParametersMatchExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntityFilterController.MultiFilter);
        var operationFilters = CreateOperationFilters();
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntityFilterController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations?[HttpMethod.Get].Parameters;

        parameters.Should().NotBeNull();

        parameters
            .Select(x => x.Name)
            .Should()
            .Equal(
                "testModelDateTimeId",
                "testModelDateTimeValueA",
                "testModelDateTimeValueB",
                "testModelDateTimeValueC",
                "testModelNestedId",
                "testModelNestedParentId",
                "testModelNestedValue"
            );

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    private static List<IOperationFilter> CreateOperationFilters()
        =>
        [
            new EntityFilterParameterReplacer(null),
            new EntityFilterSetParameterReplacer(null)
        ];
}
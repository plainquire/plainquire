using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Swashbuckle.Filters;
using Plainquire.Filter.Tests.Models;
using Plainquire.Swashbuckle.TestSupport.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Filter.Tests.Tests.OpenApiFilter;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityFilterParameterNameTests
{
    [TestMethod]
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
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;

        parameters
            .Select(x => x.Name)
            .Should()
            .Equal([
                "testModelStringId",
                "testModelStringValueA",
                "testModelStringValueB",
                "testModelStringValueC"
            ]);

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    private static List<IOperationFilter> CreateOperationFilters()
        =>
        [
            new EntityFilterParameterReplacer(null),
            new EntityFilterSetParameterReplacer(null)
        ];
}
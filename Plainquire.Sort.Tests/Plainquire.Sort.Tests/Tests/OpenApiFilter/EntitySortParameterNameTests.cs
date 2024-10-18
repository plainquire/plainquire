using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.OpenApi.Models;
using NUnit.Framework;
using Plainquire.Sort.Swashbuckle.Filters;
using Plainquire.Sort.Tests.Models;
using Plainquire.Swashbuckle.TestSupport.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.OpenApiFilter;

[TestFixture]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntitySortParameterNameTests
{
    [Test]
    public void WhenGenericEntitySortIsGiven_GeneratedSchemaPatternMatchesExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        const string actionName = nameof(EntitySortNameController.SingleSort);
        var operationFilters = CreateOperationFilters(serviceProvider);
        var swaggerGenerator = SwaggerGeneratorFactory.Create<EntitySortNameController>(actionName, operationFilters);

        // Act
        var openApiDocument = swaggerGenerator.GetSwagger("v1");

        // Assert   
        using var _ = new AssertionScope();
        var parameters = openApiDocument.Paths[$"/{actionName}"].Operations[OperationType.Get].Parameters;
        var orderBy = parameters.SingleOrDefault(parameter => parameter.Name == "orderBy")!;
        orderBy.Should().NotBeNull();
        orderBy.Description.Should().Be("Sorts the result by the given property in ascending (-asc) or descending (-desc) order.");
        orderBy.Schema.Type.Should().Be("array");
        orderBy.Schema.Items.Pattern.Should().Be(@"^(asc-|asc\ |\+|desc-|desc\ |dsc-|dsc\ |-|~)?(testModelStringId|testModelStringValue|testModelStringValue2|testModelStringNestedObject)(\..+)?(-asc|\ asc|\+|-desc|\ desc|-dsc|\ dsc|-|~)?$");

        //var debugJson = openApiDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    private static List<IOperationFilter> CreateOperationFilters(IServiceProvider serviceProvider)
        =>
        [
            new EntitySortParameterReplacer(serviceProvider),
            new EntitySortSetParameterReplacer(serviceProvider)
        ];
}
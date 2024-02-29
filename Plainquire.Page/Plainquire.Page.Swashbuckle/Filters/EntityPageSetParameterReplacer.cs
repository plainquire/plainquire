using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Plainquire.Page.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="IOperationFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="EntityPage" />
public class EntityPageSetParameterReplacer : EntityPageParameterReplacer, IOperationFilter
{
    /// <inheritdoc />
    protected override List<EntityPageParameter> GetEntityPageParameters(OpenApiOperation operation, OperationFilterContext context)
        => context
            .ApiDescription
            .ParameterDescriptions
            .Where(IsEntityPageParameter)
            .Join(
                operation.Parameters,
                parameterDescription => new { parameterDescription.Name, SchemaReferenceId = GetSchemaReferenceId(parameterDescription, context) },
                parameter => new { parameter.Name, SchemaReferenceId = parameter.Schema.Reference?.Id },
                (description, parameter) => new { Parameter = parameter, description.Type }
            )
            .Select(x => new EntityPageParameter(x.Parameter, x.Type.GetGenericArguments().First()))
            .ToList();

    private static bool IsEntityPageParameter(ApiParameterDescription description)
        => description.Type.IsEntityPage();

    private static string? GetSchemaReferenceId(ApiParameterDescription parameter, OperationFilterContext context)
        => context.SchemaGenerator.GenerateSchema(parameter.Type, context.SchemaRepository).Reference?.Id;
}
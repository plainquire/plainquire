using FS.SortQueryableCreator.Extensions;
using FS.SortQueryableCreator.Sorts;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace FS.SortQueryableCreator.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="IOperationFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="EntitySort" />
public class EntitySortSetParameterReplacer : EntitySortParameterReplacer, IOperationFilter
{
    /// <inheritdoc />
    protected override List<EntitySortParameter> GetEntitySortParameters(OpenApiOperation operation, OperationFilterContext context)
        => context
            .ApiDescription
            .ParameterDescriptions
            .Where(IsEntitySortParameter)
            .Join(
                operation.Parameters,
                parameterDescription => new { parameterDescription.Name, SchemaReferenceId = GetSchemaReferenceId(parameterDescription, context) },
                parameter => new { parameter.Name, SchemaReferenceId = parameter.Schema.Reference?.Id },
                (description, parameter) => new { Parameter = parameter, description.Type }
            )
            .Select(x => new EntitySortParameter(x.Parameter, x.Type.GetGenericArguments().First()))
            .ToList();

    private static bool IsEntitySortParameter(ApiParameterDescription description)
        => description.Type.IsGenericEntitySort();

    private static string? GetSchemaReferenceId(ApiParameterDescription parameter, OperationFilterContext context)
        => context.SchemaGenerator.GenerateSchema(parameter.Type, context.SchemaRepository).Reference?.Id;
}
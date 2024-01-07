using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace FS.FilterExpressionCreator.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntityFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
public class EntityFilterSetParameterReplacer : EntityFilterParameterReplacer, IOperationFilter
{
    /// <inheritdoc />
    public EntityFilterSetParameterReplacer(IEnumerable<string>? xmlDocumentationFilePaths)
        : base(xmlDocumentationFilePaths)
    { }

    /// <inheritdoc />
    protected override List<EntityFilterParameter> GetEntityFilterParameters(OpenApiOperation operation, OperationFilterContext context)
        => context
            .ApiDescription
            .ParameterDescriptions
            .Where(IsEntityFilterParameter)
            .Join(
                operation.Parameters,
                parameterDescription => new { parameterDescription.Name, SchemaReferenceId = GetSchemaReferenceId(parameterDescription, context) },
                parameter => new { parameter.Name, SchemaReferenceId = parameter.Schema.Reference?.Id },
                (description, parameter) => new { Parameter = parameter, description.Type }
            )
            .Select(x => new EntityFilterParameter(x.Parameter, x.Type.GetGenericArguments().First()))
            .ToList();

    private static bool IsEntityFilterParameter(ApiParameterDescription description)
        => description.Type.IsGenericEntityFilter();

    private static string? GetSchemaReferenceId(ApiParameterDescription parameter, OperationFilterContext context)
        => context.SchemaGenerator.GenerateSchema(parameter.Type, context.SchemaRepository).Reference?.Id;
}
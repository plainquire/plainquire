using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Plainquire.Filter.Abstractions.Attributes;
using Plainquire.Sort.Extensions;
using Plainquire.Sort.Sorts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Sort.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntitySort"/> with sortable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
public class EntitySortParameterReplacer : IOperationFilter
{
    private const string ENTITY_EXTENSION = "x-entity-sort-order";

    /// <summary>
    /// Replaces all parameters of type <see cref="EntitySort{TEntity}"/> with their applicable sort order properties.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var entitySortParameters = GetEntitySortParameters(operation, context);

        var sortParameters = entitySortParameters
            .Select(sortParameter => new
            {
                Parameter = ReplaceOpenApiParameter(operation, sortParameter),
                PropertyNames = GetSortPropertyNames(sortParameter.SortedType)
            })
            .GroupBy(parameter => parameter.Parameter.Name)
            .Select(parameter => new
            {
                OpneApi = parameter.First().Parameter,
                PropertyNames = parameter.SelectMany(f => f.PropertyNames).ToList()
            })
            .ToList();

        var prefixRegex = SortDirectionModifiers.PrefixPattern;
        var postfixRegex = SortDirectionModifiers.PostfixPattern;

        foreach (var parameter in sortParameters)
        {
            var allowedPropertyNamePattern = $"^{prefixRegex}({string.Join("|", parameter.PropertyNames)}){postfixRegex}$";
            parameter.OpneApi.Schema.Items.Pattern = allowedPropertyNamePattern;
        }
    }

    /// <summary>
    /// Return all parameters of type <see cref="EntitySort{TEntity}"/> from the given context.
    /// </summary>
    /// <param name="operation">The API operation.</param>
    /// <param name="context">The operation filter context.</param>
    protected virtual List<EntitySortParameter> GetEntitySortParameters(OpenApiOperation operation, OperationFilterContext context)
        => context
            .ApiDescription
            .ParameterDescriptions
            .Where(IsEntitySortParameter)
            .Join(
                operation.Parameters,
                parameterDescription => parameterDescription.Name,
                parameter => parameter.Name,
                (description, parameter) => new { Parameter = parameter, description.ParameterDescriptor.ParameterType }
            )
            .Select(x =>
            {
                var sortedType = x.ParameterType.GetGenericArguments().First();
                return new EntitySortParameter(Parameter: x.Parameter, SortedType: sortedType);
            })
            .ToList();

    private static bool IsEntitySortParameter(ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.IsGenericEntitySort();

    private static OpenApiParameter ReplaceOpenApiParameter(OpenApiOperation operation, EntitySortParameter sortParameter)
    {
        var parameterIndex = operation.Parameters.IndexOf(sortParameter.Parameter);
        operation.Parameters.RemoveAt(parameterIndex);

        var entityFilterAttribute = sortParameter.SortedType.GetCustomAttribute<FilterEntityAttribute>();
        var sortByParameterName = entityFilterAttribute?.SortByParameter ?? FilterEntityAttribute.DEFAULT_SORT_BY_PARAMETER_NAME;
        var openApiParameter = operation.Parameters.FirstOrDefault(x => x.Name == sortByParameterName);
        if (openApiParameter != null)
            return openApiParameter;

        openApiParameter = new OpenApiParameter
        {
            Name = sortByParameterName,
            Description = $"Sorts the result by the given property in ascending ({SortDirectionModifiers.DefaultAscendingPostfix}) or descending $({SortDirectionModifiers.DefaultDescendingPostfix}) order.",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString(string.Empty)
                },
            },
            In = ParameterLocation.Query,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                [ENTITY_EXTENSION] = new OpenApiBoolean(true)
            }
        };

        operation.Parameters.Insert(parameterIndex, openApiParameter);

        return openApiParameter;
    }

    private static List<string> GetSortPropertyNames(Type sortedType)
    {
        var entityFilterAttribute = sortedType.GetCustomAttribute<FilterEntityAttribute>();
        var sortableProperties = sortedType
            .GetSortableProperties()
            .Select(property => property.GetSortParameterName(entityFilterAttribute?.Prefix))
            .ToList();

        return sortableProperties;
    }

    /// <summary>
    /// A single sort order parameter.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EntitySortParameterReplacer"/> class.
    /// </remarks>
    /// <param name="Parameter">Gets the OpenAPI parameter.</param>
    /// <param name="SortedType">Gets the type of the entity to sort.</param>
    protected record EntitySortParameter(OpenApiParameter Parameter, Type SortedType);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Plainquire.Filter.Abstractions.Attributes;
using Plainquire.Page.Pages;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Plainquire.Page.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntityPage"/> with page properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
public class EntityPageParameterReplacer : IOperationFilter
{
    private const string ENTITY_EXTENSION = "x-entity-page";

    /// <summary>
    /// Replaces all parameters of type <see cref="EntityPage{TEntity}"/> with their applicable page properties.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var entityPageParameters = GetEntityPageParameters(operation, context);
        ReplacePageNumberParameters(operation, entityPageParameters);
        ReplacePageSizeParameters(operation, entityPageParameters);
    }

    /// <summary>
    /// Return all parameters of type <see cref="EntityPage{TEntity}"/> from the given context.
    /// </summary>
    /// <param name="operation">The API operation.</param>
    /// <param name="context">The operation filter context.</param>
    protected virtual List<EntityPageParameter> GetEntityPageParameters(OpenApiOperation operation, OperationFilterContext context)
    {
        var operationParameters = new List<OpenApiParameter>(operation.Parameters);

        return context
            .ApiDescription
            .ParameterDescriptions
            .Where(IsEntityPageParameter)
            .Select(description =>
            {
                var operationParameter = operationParameters.First(parameter => parameter.Name == description.Name);
                operationParameters.Remove(operationParameter);

                var parameterType = description.ParameterDescriptor.ParameterType;
                var pagedType = parameterType.IsGenericType ? parameterType.GetGenericArguments().First() : null;

                return new EntityPageParameter(Parameter: operationParameter, PagedType: pagedType);
            })
            .ToList();
    }

    private static bool IsEntityPageParameter(ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.IsAssignableTo(typeof(EntityPage));

    private static void ReplacePageNumberParameters(OpenApiOperation operation, List<EntityPageParameter> entityPageParameters)
    {
        var entityPageNumberParameters = entityPageParameters
            .Where(x => x.Parameter.Name == nameof(EntityPage.PageNumber))
            .ToList();

        foreach (var entityPageNumberParameter in entityPageNumberParameters)
            ReplaceOpenApiPageNumberParameter(operation, entityPageNumberParameter);
    }

    private static void ReplaceOpenApiPageNumberParameter(OpenApiOperation operation, EntityPageParameter pageParameter)
    {
        var parameterIndex = operation.Parameters.IndexOf(pageParameter.Parameter);
        operation.Parameters.RemoveAt(parameterIndex);

        var entityFilterAttribute = pageParameter.PagedType?.GetCustomAttribute<FilterEntityAttribute>();
        var pageNumberParameterName = entityFilterAttribute?.PageNumberParameter ?? FilterEntityAttribute.DEFAULT_PAGE_NUMBER_PARAMETER_NAME;

        var openApiParameter = operation.Parameters.FirstOrDefault(x => x.Name == pageNumberParameterName);
        if (openApiParameter != null)
            return;

        openApiParameter = new OpenApiParameter
        {
            Name = pageNumberParameterName,
            Description = "Pages the result by the given page number.",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Format = "int32"
            },
            In = ParameterLocation.Query,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                [ENTITY_EXTENSION] = new OpenApiBoolean(true)
            }
        };

        operation.Parameters.Insert(parameterIndex, openApiParameter);
    }

    private static void ReplacePageSizeParameters(OpenApiOperation operation, List<EntityPageParameter> entityPageParameters)
    {
        var entityPageSizeParameters = entityPageParameters
            .Where(x => x.Parameter.Name == nameof(EntityPage.PageSize))
            .ToList();

        foreach (var entityPageSizeParameter in entityPageSizeParameters)
            ReplaceOpenApiPageSizeParameter(operation, entityPageSizeParameter);
    }

    private static void ReplaceOpenApiPageSizeParameter(OpenApiOperation operation, EntityPageParameter pageParameter)
    {
        var parameterIndex = operation.Parameters.IndexOf(pageParameter.Parameter);
        operation.Parameters.RemoveAt(parameterIndex);

        var entityFilterAttribute = pageParameter.PagedType?.GetCustomAttribute<FilterEntityAttribute>();
        var pageSizeParameterName = entityFilterAttribute?.PageSizeParameter ?? FilterEntityAttribute.DEFAULT_PAGE_SIZE_PARAMETER_NAME;

        var openApiParameter = operation.Parameters.FirstOrDefault(x => x.Name == pageSizeParameterName);
        if (openApiParameter != null)
            return;

        openApiParameter = new OpenApiParameter
        {
            Name = pageSizeParameterName,
            Description = "Pages the result by the given page size.",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Format = "int32"
            },
            In = ParameterLocation.Query,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                [ENTITY_EXTENSION] = new OpenApiBoolean(true)
            }
        };

        operation.Parameters.Insert(parameterIndex, openApiParameter);
    }

    /// <summary>
    /// A single page parameter.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EntityPageParameterReplacer"/> class.
    /// </remarks>
    /// <param name="Parameter">Gets the OpenAPI parameter.</param>
    /// <param name="PagedType">Gets the type of the entity to page.</param>
    protected record EntityPageParameter(OpenApiParameter Parameter, Type? PagedType);
}
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi;
using Plainquire.Page.Swashbuckle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Plainquire.Page.Swashbuckle;

/// <summary>
/// Extension methods for <see cref="OpenApiOperation"/>.
/// </summary>
public static class OpenApiOperationExtensions
{
    private const string ENTITY_PAGE_EXTENSION = "x-entity-page";
    private const string ENTITY_DELETE_EXTENSION = "x-entity-page-delete";


    /// <summary>
    /// Replaces <see cref="EntityPage{TEntity}"/> and with the sort parameters.
    /// </summary>
    /// <param name="operation">The <see cref="OpenApiOperation"/> to operate on.</param>
    /// <param name="parametersToReplace">The parameters to replace.</param>
    public static void ReplacePageParameters(this OpenApiOperation operation, IList<PageParameterReplacement> parametersToReplace)
    {
        MarkExistingParametersForDeletion(parametersToReplace);
        ReplacePageNumberParameters(operation, parametersToReplace);
        ReplacePageSizeParameters(operation, parametersToReplace);
        RemoveParametersMarkedForDeletion(operation);
    }

    private static void ReplacePageNumberParameters(OpenApiOperation operation, IList<PageParameterReplacement> parametersToReplace)
    {
        var httpQueryParameterGroup = GroupByPageNumberHttpQueryParameterName(parametersToReplace);
        foreach (var (queryParameter, parameters) in httpQueryParameterGroup)
        {
            var openApiParameter = new OpenApiParameter
            {
                Name = queryParameter,
                Description = "Pages the result by the given page number.",
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Integer,
                    Format = "int32"
                },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                {
                    [ENTITY_PAGE_EXTENSION] = new JsonNodeExtension(JsonValue.Create(true))
                }
            };

            operation.Parameters ??= new List<IOpenApiParameter>();
            var insertionIndex = operation.Parameters.IndexOf(parameters[0].OpenApiParameter);
            operation.Parameters.Insert(insertionIndex, openApiParameter);
        }
    }

    private static void ReplacePageSizeParameters(OpenApiOperation operation, IList<PageParameterReplacement> parametersToReplace)
    {
        var httpQueryParameterGroup = GroupByPageSizeHttpQueryParameterName(parametersToReplace);
        foreach (var (queryParameter, parameters) in httpQueryParameterGroup)
        {
            var openApiParameter = new OpenApiParameter
            {
                Name = queryParameter,
                Description = "Pages the result by the given page size.",
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Integer,
                    Format = "int32"
                },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                {
                    [ENTITY_PAGE_EXTENSION] = new JsonNodeExtension(JsonValue.Create(true))
                }
            };

            operation.Parameters ??= new List<IOpenApiParameter>();
            var insertionIndex = operation.Parameters.IndexOf(parameters[0].OpenApiParameter);
            operation.Parameters.Insert(insertionIndex, openApiParameter);
        }
    }

    private static Dictionary<string, List<PageParameterReplacement>> GroupByPageNumberHttpQueryParameterName(IList<PageParameterReplacement> parametersToReplace)
        => parametersToReplace
            .GroupBy(parameter => GetPageNumberParameterName(parameter.OpenApiDescription.ParameterDescriptor), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToList(),
                StringComparer.Ordinal
            );

    private static Dictionary<string, List<PageParameterReplacement>> GroupByPageSizeHttpQueryParameterName(IList<PageParameterReplacement> parametersToReplace)
        => parametersToReplace
            .GroupBy(parameter => GetPageSizeParameterName(parameter.OpenApiDescription.ParameterDescriptor), StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.ToList(),
                StringComparer.Ordinal
            );

    private static void MarkExistingParametersForDeletion(IList<PageParameterReplacement> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.OpenApiParameter is not IOpenApiExtensible extensibleParameter)
                throw new InvalidOperationException("The OpenApiParameter must implement IOpenApiExtensible to be replaceable.");

            extensibleParameter.Extensions ??= new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase);
            extensibleParameter.Extensions.TryAdd(ENTITY_DELETE_EXTENSION, new JsonNodeExtension(JsonValue.Create(true)));
        }
    }

    private static void RemoveParametersMarkedForDeletion(OpenApiOperation operation)
    {
        operation.Parameters ??= new List<IOpenApiParameter>();
        var parametersToRemove = operation.Parameters
            .Where(parameter =>
            {
                if (parameter is not IOpenApiExtensible extensibleParameter)
                    throw new InvalidOperationException("The OpenApiParameter must implement IOpenApiExtensible to be replaceable.");

                extensibleParameter.Extensions ??= new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase);
                return extensibleParameter.Extensions.ContainsKey(ENTITY_DELETE_EXTENSION);
            })
            .ToList();

        foreach (var parameter in parametersToRemove)
            operation.Parameters.Remove(parameter);
    }

    private static string GetPageNumberParameterName(ParameterDescriptor parameterDescriptor)
    {
        var actionParameterName = parameterDescriptor.Name;
        var bindingParameterName = parameterDescriptor.BindingInfo?.BinderModelName;
        var (pageNumberName, _) = ParameterExtensions.GetPageParameterNames(actionParameterName, bindingParameterName);
        return pageNumberName;
    }

    private static string GetPageSizeParameterName(ParameterDescriptor parameterDescriptor)
    {
        var actionParameterName = parameterDescriptor.Name;
        var bindingParameterName = parameterDescriptor.BindingInfo?.BinderModelName;
        var (_, pageSizeName) = ParameterExtensions.GetPageParameterNames(actionParameterName, bindingParameterName);
        return pageSizeName;
    }
}
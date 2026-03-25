using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi;
using Plainquire.Page.Swashbuckle.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Plainquire.Page.Swashbuckle;

/// <summary>
/// Extension methods for <see cref="OpenApiOperation"/>.
/// </summary>
internal static class OpenApiOperationExtensions
{
    private const string PARAMETER_INDEX_EXTENSION = "x-original-parameter-index";
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

    public static void AddOriginalIndexExtensionIfMissing(this OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            return;

        var indexExtensionsAlreadyAdded = operation.Parameters.Any(p => p.Extensions?.ContainsKey(PARAMETER_INDEX_EXTENSION) == true);
        if (indexExtensionsAlreadyAdded)
            return;

        for (var index = 0; index < operation.Parameters.Count; index++)
        {
            var parameter = operation.Parameters[index];
            if (parameter is not IOpenApiExtensible openApiParameter)
                throw new InvalidOperationException("The OpenApiParameter must implement IOpenApiExtensible to be replaceable.");

            openApiParameter.Extensions ??= new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase);
            openApiParameter.Extensions.Add(PARAMETER_INDEX_EXTENSION, new JsonNodeExtension(JsonValue.Create(index)));
        }
    }

    public static int GetOriginalIndex(this IOpenApiParameter parameter)
    {
        if (parameter.Extensions == null)
            return -1;

        if (!parameter.Extensions.TryGetValue(PARAMETER_INDEX_EXTENSION, out var extension))
            return -1;

        if (extension is not JsonNodeExtension nodeExtension)
            throw new InvalidOperationException($"Extension '{PARAMETER_INDEX_EXTENSION}' must be of type {nameof(JsonNodeExtension)}.");

        if (nodeExtension.Node is not JsonValue jsonValue)
            throw new InvalidOperationException($"Value of extension '{PARAMETER_INDEX_EXTENSION}' must be of type {nameof(JsonValue)}.");

        return jsonValue.GetValue<int>();
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
                Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase)
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
                Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase)
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
            .GroupBy(parameter => GetPageNumberParameterName(parameter.OpenApiDescription.ParameterDescriptor), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.ToList(),
                StringComparer.OrdinalIgnoreCase
            );

    private static Dictionary<string, List<PageParameterReplacement>> GroupByPageSizeHttpQueryParameterName(IList<PageParameterReplacement> parametersToReplace)
        => parametersToReplace
            .GroupBy(parameter => GetPageSizeParameterName(parameter.OpenApiDescription.ParameterDescriptor), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.ToList(),
                StringComparer.OrdinalIgnoreCase
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
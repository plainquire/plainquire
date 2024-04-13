using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Plainquire.Page.Swashbuckle.Models;
using System.Collections.Generic;
using System.Linq;

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
    public static void ReplacePageParameters(this OpenApiOperation operation, List<PageParameterReplacement> parametersToReplace)
    {
        MarkExistingParametersForDeletion(parametersToReplace);
        ReplacePageNumberParameters(operation, parametersToReplace);
        ReplacePageSizeParameters(operation, parametersToReplace);
        RemoveParametersMarkedForDeletion(operation);
    }

    private static void ReplacePageNumberParameters(OpenApiOperation operation, List<PageParameterReplacement> parametersToReplace)
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
                    Type = "integer",
                    Format = "int32"
                },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    [ENTITY_PAGE_EXTENSION] = new OpenApiBoolean(true)
                }
            };

            var insertionIndex = operation.Parameters.IndexOf(parameters.First().OpenApiParameter);
            operation.Parameters.Insert(insertionIndex, openApiParameter);
        }
    }

    private static void ReplacePageSizeParameters(OpenApiOperation operation, List<PageParameterReplacement> parametersToReplace)
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
                    Type = "integer",
                    Format = "int32"
                },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    [ENTITY_PAGE_EXTENSION] = new OpenApiBoolean(true)
                }
            };

            var insertionIndex = operation.Parameters.IndexOf(parameters.First().OpenApiParameter);
            operation.Parameters.Insert(insertionIndex, openApiParameter);
        }
    }

    private static Dictionary<string, List<PageParameterReplacement>> GroupByPageNumberHttpQueryParameterName(List<PageParameterReplacement> parametersToReplace)
        => parametersToReplace
            .GroupBy(parameter => GetPageNumberParameterName(parameter.OpenApiDescription.ParameterDescriptor))
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );

    private static Dictionary<string, List<PageParameterReplacement>> GroupByPageSizeHttpQueryParameterName(List<PageParameterReplacement> parametersToReplace)
        => parametersToReplace
            .GroupBy(parameter => GetPageSizeParameterName(parameter.OpenApiDescription.ParameterDescriptor))
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );

    private static void MarkExistingParametersForDeletion(List<PageParameterReplacement> parameters)
    {
        foreach (var parameter in parameters)
            parameter.OpenApiParameter.Extensions.TryAdd(ENTITY_DELETE_EXTENSION, new OpenApiBoolean(true));
    }

    private static void RemoveParametersMarkedForDeletion(OpenApiOperation operation)
    {
        var parametersToRemove = operation.Parameters
            .Where(parameter => parameter.Extensions.ContainsKey(ENTITY_DELETE_EXTENSION))
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
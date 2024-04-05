using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Plainquire.Sort.Swashbuckle.Filters;
using Plainquire.Sort.Swashbuckle.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plainquire.Sort.Swashbuckle;

public static class OpenApiOperationExtensions
{
    private const string ENTITY_SORT_EXTENSION = "x-entity-sort";
    private const string ENTITY_DELETE_EXTENSION = "x-entity-sort-delete";

    public static void ReplaceSortParameters(this OpenApiOperation operation, List<SortParameterReplacement> parametersToReplace)
    {
        MarkExistingParametersForDeletion(parametersToReplace);

        var httpQueryParameterGroup = GroupByHttpQueryParameterName(parametersToReplace);
        foreach (var (queryParameter, parameters) in httpQueryParameterGroup)
        {
            var (prefixes, postfixes, primaryAscendingPostfix, primaryDescendingPostfix) = GetSortPrefixes(parameters);
            var allowedPropertyNamePattern = CreatePropertyNamePattern(parameters, prefixes, postfixes);

            var openApiParameter = new OpenApiParameter
            {
                Name = queryParameter,
                Description = $"Sorts the result by the given property in ascending ({primaryAscendingPostfix}) or descending ({primaryDescendingPostfix}) order.",
                Schema = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string",
                        Example = new OpenApiString(string.Empty),
                        Pattern = allowedPropertyNamePattern
                    },
                },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    [ENTITY_SORT_EXTENSION] = new OpenApiBoolean(true)
                }
            };

            var insertionIndex = operation.Parameters.IndexOf(parameters.First().OpenApiParameter);
            operation.Parameters.Insert(insertionIndex, openApiParameter);
        }

        RemoveParametersMarkedForDeletion(operation);
    }

    private static Dictionary<string, List<SortParameterReplacement>> GroupByHttpQueryParameterName(List<SortParameterReplacement> parametersToReplace)
        => parametersToReplace
            .GroupBy(parameter => parameter.Configuration.HttpQueryParameterName)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );

    private static void MarkExistingParametersForDeletion(List<SortParameterReplacement> parameters)
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

    private static (List<string>, List<string>, string, string) GetSortPrefixes(IReadOnlyCollection<SortParameterReplacement> parameters)
    {
        var ascendingPrefixes = parameters.Select(parameter => parameter.Configuration.AscendingPrefixes).SelectMany(x => x).Distinct().ToList();
        var descendingPrefixes = parameters.Select(parameter => parameter.Configuration.DescendingPrefixes).SelectMany(x => x).Distinct().ToList();
        var ascendingPostfixes = parameters.Select(parameter => parameter.Configuration.AscendingPostfixes).SelectMany(x => x).Distinct().ToList();
        var descendingPostfixes = parameters.Select(parameter => parameter.Configuration.DescendingPostfixes).SelectMany(x => x).Distinct().ToList();

        var prefixes = ascendingPrefixes.Concat(descendingPrefixes).Distinct().Select(Regex.Escape).ToList();
        var postfixes = ascendingPostfixes.Concat(descendingPostfixes).Distinct().Select(Regex.Escape).ToList();

        var primaryAscendingPostfix = ascendingPostfixes.First();
        var primaryDescendingPostfix = descendingPostfixes.First();

        return (prefixes, postfixes, primaryAscendingPostfix, primaryDescendingPostfix);
    }

    private static string CreatePropertyNamePattern(IEnumerable<SortParameterReplacement> parameters, IEnumerable<string> prefixes, IEnumerable<string> postfixes)
    {
        var sortablePropertyNames = GetSortablePropertyNames(parameters);

        var allowedPropertyNamePattern = $"^({string.Join("|", prefixes)})?({string.Join("|", sortablePropertyNames)})({string.Join("|", postfixes)})?$";
        return allowedPropertyNamePattern;
    }

    private static List<string> GetSortablePropertyNames(IEnumerable<SortParameterReplacement> parameters)
        => parameters
            .Select(parameter => parameter.SortedType)
            .SelectMany(sortedType => sortedType.GetSortPropertyNames())
            .Distinct()
            .Select(Regex.Escape)
            .ToList();
}
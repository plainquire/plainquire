using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Swashbuckle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Filter.Swashbuckle;

internal static class OpenApiParameterExtensions
{
    internal const string ENTITY_EXTENSION_PREFIX = "x-entity-filter-";

    public static bool IsEntityFilterParameter(this ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.IsGenericEntityFilter();

    public static bool IsEntityFilterSetParameter(this ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.GetCustomAttribute<EntityFilterSetAttribute>() != null;

    public static void ReplaceFilterParameters(this IList<OpenApiParameter> parameters, List<FilterParameterReplaceInfo> parameterReplacements, IReadOnlyCollection<DocXmlReader> docXmlReaders)
    {
        foreach (var replacement in parameterReplacements)
        {
            if (!replacement.Parameters.Any())
                continue;

            var parameterIndex = parameters.IndexOf(replacement.Parameters.First());

            foreach (var parameter in replacement.Parameters)
                parameters.Remove(parameter);

            var propertyParameters = replacement
                .EntityFilters
                .SelectMany(entityFilterType => entityFilterType
                    .GenericTypeArguments[0]
                    .ExpandToPropertyParameters(docXmlReaders)
                )
                .ToList();

            foreach (var parameter in propertyParameters)
                parameters.Insert(parameterIndex++, parameter);
        }
    }

    private static List<OpenApiParameter> ExpandToPropertyParameters(this Type filteredType, IReadOnlyCollection<DocXmlReader> docXmlReaders)
    {
        var filterableProperties = filteredType.GetFilterableProperties();
        var entityFilterAttribute = filteredType.GetCustomAttribute<EntityFilterAttribute>();

        return filterableProperties
            .Select(property => new OpenApiParameter
            {
                Name = property.GetFilterParameterName(entityFilterAttribute?.Prefix),
                Description = docXmlReaders.GetXmlDocumentationSummary(property),
                Schema = new OpenApiSchema { Type = "string" },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    [ENTITY_EXTENSION_PREFIX + "property-type"] = new OpenApiString(property.PropertyType.GetUnderlyingType().Name)
                }
            })
            .ToList();
    }

    private static string? GetXmlDocumentationSummary(this IEnumerable<DocXmlReader> docXmlReaders, MemberInfo member)
        => docXmlReaders.Select(x => x.GetMemberComment(member)).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
}
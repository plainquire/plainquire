using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Swashbuckle.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;

namespace Plainquire.Filter.Swashbuckle;

internal static class OpenApiParameterExtensions
{
    private const string PARAMETER_INDEX_EXTENSION = "x-original-parameter-index";
    internal const string ENTITY_EXTENSION_PREFIX = "x-entity-filter-";

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract", Justification = "ParameterDescriptor can be null")]
    public static bool IsEntityFilterParameter(this ApiParameterDescription description)
        => description.ParameterDescriptor != null && description.ParameterDescriptor.ParameterType.IsGenericEntityFilter();

    [SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract", Justification = "ParameterDescriptor can be null")]
    public static bool IsEntityFilterSetParameter(this ApiParameterDescription description)
        => description.ParameterDescriptor?.ParameterType.GetCustomAttribute<EntityFilterSetAttribute>() != null;

    public static void ReplaceFilterParameters(this IList<IOpenApiParameter> parameters, List<FilterParameterReplaceInfo> parameterReplacements, IReadOnlyCollection<DocXmlReader> docXmlReaders)
    {
        foreach (var replacement in parameterReplacements)
        {
            if (!replacement.Parameters.Any())
                continue;

            var parameterIndex = parameters.IndexOf(replacement.Parameters[0]);

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

    private static List<OpenApiParameter> ExpandToPropertyParameters(this Type filteredType, IReadOnlyCollection<DocXmlReader> docXmlReaders)
    {
        var filterableProperties = filteredType.GetFilterableProperties();
        var entityFilterAttribute = filteredType.GetCustomAttribute<EntityFilterAttribute>();

        return filterableProperties
            .Select(property => new OpenApiParameter
            {
                Name = property.GetFilterParameterName(entityFilterAttribute?.Prefix),
                Description = docXmlReaders.GetXmlDocumentationSummary(property),
                Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase)
                {
                    [ENTITY_EXTENSION_PREFIX + "property-type"] = new JsonNodeExtension(JsonValue.Create(property.PropertyType.GetUnderlyingType().Name))
                }
            })
            .ToList();
    }

    private static string? GetXmlDocumentationSummary(this IEnumerable<DocXmlReader> docXmlReaders, MemberInfo member)
        => docXmlReaders.Select(x => x.GetMemberComment(member)).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
}
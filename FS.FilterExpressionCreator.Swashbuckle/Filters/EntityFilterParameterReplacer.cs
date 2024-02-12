using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace FS.FilterExpressionCreator.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntityFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Instantiated via reflection.")]
[Obsolete("Use 'Plainquire.Filter.Swashbuckle.Filters.EntityFilterParameterReplacer' instead.")]
public class EntityFilterParameterReplacer : IOperationFilter
{
    private const string ENTITY_EXTENSION_PREFIX = "x-entity-filter-";

    private readonly List<DocXmlReader> _docXmlReaders;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFilterParameterReplacer"/> class.
    /// </summary>
    /// <param name="xmlDocumentationFilePaths">Paths to XML documentation files. Used to provide parameter descriptions.</param>
    public EntityFilterParameterReplacer(IEnumerable<string>? xmlDocumentationFilePaths)
        => _docXmlReaders = xmlDocumentationFilePaths?.Select(x => new DocXmlReader(x)).ToList() ?? [];

    /// <summary>
    /// Replaces all parameters of type <see cref="EntityFilter{TEntity}"/> with their applicable filter properties.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var entityFilterParameters = GetEntityFilterParameters(operation, context);
        foreach (var filterParameter in entityFilterParameters)
        {
            var parameterIndex = operation.Parameters.IndexOf(filterParameter.Parameter);
            operation.Parameters.Remove(filterParameter.Parameter);

            var propertyParameters = ExpandToPropertyParameters(filterParameter.FilteredType);
            foreach (var parameter in propertyParameters)
                operation.Parameters.Insert(parameterIndex++, parameter);
        }

        var hasParametersFromEntityFilter = entityFilterParameters.Any();
        operation.Extensions[ENTITY_EXTENSION_PREFIX + "has-filter-parameters"] = new OpenApiBoolean(hasParametersFromEntityFilter);
    }

    /// <summary>
    /// Return all parameters of type <see cref="EntityFilter{TEntity}"/> from the given context.
    /// </summary>
    /// <param name="operation">The API operation.</param>
    /// <param name="context">The operation filter context.</param>
    protected virtual List<EntityFilterParameter> GetEntityFilterParameters(OpenApiOperation operation, OperationFilterContext context)
        => context
            .ApiDescription
            .ParameterDescriptions
            .Where(IsEntityFilterParameter)
            .Join(
                operation.Parameters,
                parameterDescription => parameterDescription.Name,
                parameter => parameter.Name,
                (description, parameter) => new { Parameter = parameter, description.ParameterDescriptor.ParameterType }
            )
            .Select(x => new EntityFilterParameter(x.Parameter, x.ParameterType.GetGenericArguments().First()))
            .ToList();

    private static bool IsEntityFilterParameter(ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.IsGenericEntityFilter();

    private List<OpenApiParameter> ExpandToPropertyParameters(Type filteredType)
    {
        var filterableProperties = filteredType.GetFilterableProperties();
        var entityFilterAttribute = filteredType.GetCustomAttribute<FilterEntityAttribute>();

        return filterableProperties
            .Select(property => new OpenApiParameter
            {
                Name = property.GetFilterParameterName(entityFilterAttribute?.Prefix),
                Description = GetXmlDocumentationSummary(property),
                Schema = new OpenApiSchema { Type = "string" },
                In = ParameterLocation.Query,
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    [ENTITY_EXTENSION_PREFIX + "property-type"] = new OpenApiString(property.PropertyType.GetUnderlyingType().Name)
                }
            })
            .ToList();
    }

    private string? GetXmlDocumentationSummary(MemberInfo member)
        => _docXmlReaders.Select(x => x.GetMemberComment(member)).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

    /// <summary>
    /// A single entity filter parameter.
    /// </summary>
    protected readonly struct EntityFilterParameter
    {
        /// <summary>
        /// Gets the OpenAPI parameter.
        /// </summary>
        public OpenApiParameter Parameter { get; }

        /// <summary>
        /// Gets the type of the filtered entity.
        /// </summary>
        public Type FilteredType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilterParameterReplacer"/> class.
        /// </summary>
        /// <param name="parameter">Gets the OpenAPI parameter.</param>
        /// <param name="filteredType">Gets the type of the filtered entity.</param>
        public EntityFilterParameter(OpenApiParameter parameter, Type filteredType)
        {
            Parameter = parameter;
            FilteredType = filteredType;
        }
    }
}
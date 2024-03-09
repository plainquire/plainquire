using LoxSmoke.DocXml;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Plainquire.Filter.Swashbuckle.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Filter.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntityFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated via reflection.")]
public class EntityFilterParameterReplacer : IOperationFilter
{
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
        var parameterReplacements = GetEntityFilterReplacements(operation, context);
        operation.Parameters.ReplaceFilterParameters(parameterReplacements, _docXmlReaders);

        var hasParametersFromEntityFilter = parameterReplacements.Any();
        operation.Extensions[OpenApiParameterExtensions.ENTITY_EXTENSION_PREFIX + "has-filter-parameters"] = new OpenApiBoolean(hasParametersFromEntityFilter);
    }

    private static List<FilterParameterReplaceInfo> GetEntityFilterReplacements(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameterReplacements = operation.Parameters
            .Zip(
                context.ApiDescription.ParameterDescriptions,
                (parameter, description) => new { Parameter = parameter, Description = description }
             )
            .Where(openApi => openApi.Description.IsEntityFilterParameter())
            .GroupBy(openApi => openApi.Description.ParameterDescriptor.ParameterType)
            .Select(parameterGroup =>
            {
                var parametersToRemove = parameterGroup.Select(x => x.Parameter).ToList();
                var filteredTypesToAdd = new[] { parameterGroup.Key }.ToList();
                return new FilterParameterReplaceInfo(parametersToRemove, filteredTypesToAdd);
            })
            .ToList();

        return parameterReplacements;
    }
}
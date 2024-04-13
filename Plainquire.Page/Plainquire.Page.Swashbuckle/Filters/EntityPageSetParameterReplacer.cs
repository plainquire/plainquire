using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Plainquire.Filter.Abstractions;
using Plainquire.Page.Abstractions;
using Plainquire.Page.Swashbuckle.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Plainquire.Page.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="IOperationFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="EntityPage" />
public class EntityPageSetParameterReplacer : IOperationFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PageConfiguration _defaultConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPageSetParameterReplacer"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public EntityPageSetParameterReplacer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _defaultConfiguration = _serviceProvider.GetService<IOptions<PageConfiguration>>()?.Value ?? new PageConfiguration();
    }

    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parametersToReplace = operation.Parameters
            .Zip(
                context.ApiDescription.ParameterDescriptions,
                (parameter, description) => (Parameter: parameter, Description: description)
            )
            .Where(openApi => IsEntityPageSetParameter(openApi.Description))
            .SelectMany(openApi =>
                openApi.Description.ParameterDescriptor
                    .ParameterType
                    .GetProperties()
                    .Select(x => x.PropertyType)
                    .Where(type => type.IsEntityPage())
                    .Select(entityPageType => new PageParameterReplacement(
                        OpenApiParameter: openApi.Parameter,
                        OpenApiDescription: openApi.Description,
                        PagedType: entityPageType.IsGenericType ? entityPageType.GenericTypeArguments[0] : null,
                        Configuration: GetConfiguration(entityPageType))
                    )
            )
            .ToList();

        operation.ReplacePageParameters(parametersToReplace);
    }

    private static bool IsEntityPageSetParameter(ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.GetCustomAttribute<EntityPageSetAttribute>() != null;

    private PageConfiguration GetConfiguration(Type entityPageType)
    {
        if (!entityPageType.IsEntityPage())
            throw new ArgumentException("Type is not an EntityPage", nameof(entityPageType));

        var entityTypeConfiguration = ((EntityPage?)_serviceProvider.GetService(entityPageType))?.Configuration;
        return entityTypeConfiguration ?? _defaultConfiguration;
    }
}
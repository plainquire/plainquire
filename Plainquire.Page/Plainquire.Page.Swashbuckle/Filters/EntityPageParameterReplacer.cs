using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Plainquire.Page.Abstractions;
using Plainquire.Page.Swashbuckle.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntityPage"/> with page properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
public class EntityPageParameterReplacer : IOperationFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly PageConfiguration _defaultConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPageParameterReplacer"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public EntityPageParameterReplacer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _defaultConfiguration = _serviceProvider.GetService<IOptions<PageConfiguration>>()?.Value ?? PageConfiguration.Default ?? new PageConfiguration();
    }

    /// <summary>
    /// Replaces all parameters of type <see cref="EntityPage{TEntity}"/> with their applicable page properties.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parametersToReplace = operation.Parameters
            .Join(
                context.ApiDescription.ParameterDescriptions,
                parameter => parameter.Name,
                description => description.Name,
                (parameter, description) => (Parameter: parameter, Description: description),
                StringComparer.Ordinal
            )
            .Where(openApi => IsEntityPageParameter(openApi.Description))
            .Select(openApi =>
            {
                var entityPageType = openApi.Description.ParameterDescriptor.ParameterType;
                var configuration = GetConfiguration(entityPageType);
                return new PageParameterReplacement(
                    OpenApiParameter: openApi.Parameter,
                    OpenApiDescription: openApi.Description,
                    PagedType: entityPageType.IsGenericType ? entityPageType.GenericTypeArguments[0] : null,
                    Configuration: configuration);
            })
            .ToList();

        operation.ReplacePageParameters(parametersToReplace);
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract", Justification = "ParameterDescriptor can be null")]
    private static bool IsEntityPageParameter(ApiParameterDescription description)
        => description.ParameterDescriptor != null && description.ParameterDescriptor.ParameterType.IsAssignableTo(typeof(EntityPage));

    private PageConfiguration GetConfiguration(Type entityPageType)
    {
        if (!entityPageType.IsEntityPage())
            throw new ArgumentException("Type is not an EntityPage", nameof(entityPageType));

        var entityTypeConfiguration = ((EntityPage?)_serviceProvider.GetService(entityPageType))?.Configuration;
        return entityTypeConfiguration ?? _defaultConfiguration;
    }
}
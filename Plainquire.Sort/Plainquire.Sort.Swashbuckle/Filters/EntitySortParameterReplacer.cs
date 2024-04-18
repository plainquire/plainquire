using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Swashbuckle.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Swashbuckle.Filters;

/// <summary>
/// Replaces action parameters of type <see cref="EntitySort"/> with sortable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="IOperationFilter" />
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated via DI.")]
public class EntitySortParameterReplacer : IOperationFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SortConfiguration _defaultConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySortParameterReplacer"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public EntitySortParameterReplacer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _defaultConfiguration = _serviceProvider.GetService<IOptions<SortConfiguration>>()?.Value ?? SortConfiguration.Default ?? new SortConfiguration();
    }

    /// <summary>
    /// Replaces all parameters of type <see cref="EntitySort{TEntity}"/> with their applicable sort order properties.
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
                (parameter, description) => (Parameter: parameter, Description: description)
            )
            .Where(openApi => IsEntitySortParameter(openApi.Description))
            .Select(openApi =>
            {
                var entitySortType = openApi.Description.ParameterDescriptor.ParameterType;
                var configuration = GetConfiguration(entitySortType);
                return new SortParameterReplacement(
                    OpenApiParameter: openApi.Parameter,
                    OpenApiDescription: openApi.Description,
                    SortedType: entitySortType.GenericTypeArguments[0],
                    Configuration: configuration);
            })
            .ToList();

        operation.ReplaceSortParameters(parametersToReplace);
    }

    private static bool IsEntitySortParameter(ApiParameterDescription description)
        => description.ParameterDescriptor.ParameterType.IsGenericEntitySort();

    private SortConfiguration GetConfiguration(Type entitySortType)
    {
        if (!entitySortType.IsGenericEntitySort())
            throw new ArgumentException("Type is not an EntitySort<>", nameof(entitySortType));

        var entityTypeConfiguration = ((EntitySort?)_serviceProvider.GetService(entitySortType))?.Configuration;
        return entityTypeConfiguration ?? _defaultConfiguration;
    }
}
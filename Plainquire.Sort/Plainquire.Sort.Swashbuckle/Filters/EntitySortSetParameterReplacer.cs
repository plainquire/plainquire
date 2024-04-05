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
/// Replaces action parameters of type <see cref="IOperationFilter"/> with filterable properties of type <c>TEntity</c>.
/// Implements <see cref="IOperationFilter" />
/// </summary>
/// <seealso cref="EntitySort" />
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by reflection")]
public class EntitySortSetParameterReplacer : IOperationFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SortConfiguration _defaultConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySortSetParameterReplacer"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public EntitySortSetParameterReplacer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _defaultConfiguration = _serviceProvider.GetService<IOptions<SortConfiguration>>()?.Value ?? new SortConfiguration();
    }

    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parametersToReplace = operation.Parameters
            .Zip(
                context.ApiDescription.ParameterDescriptions,
                (parameter, description) => (Parameter: parameter, Description: description)
            )
            .Where(openApi => openApi.Description.IsEntitySortSetParameter())
            .SelectMany(openApi =>
                openApi.Description.ParameterDescriptor
                    .ParameterType
                    .GetProperties()
                    .Select(x => x.PropertyType)
                    .Where(type => type.IsGenericEntitySort())
                    .Select(entitySortType => new SortParameterReplacement
                    {
                        OpenApiParameter = openApi.Parameter,
                        OpenApiDescription = openApi.Description,
                        SortedType = entitySortType.GenericTypeArguments[0],
                        Configuration = GetConfiguration(entitySortType)
                    })
            )
            .ToList();

        operation.ReplaceSortParameters(parametersToReplace);
    }

    private SortConfiguration GetConfiguration(Type entitySortType)
    {
        if (!entitySortType.IsGenericEntitySort())
            throw new ArgumentException("Type is not an EntitySort<>", nameof(entitySortType));

        var entityTypeConfiguration = ((EntitySort?)_serviceProvider.GetService(entitySortType))?.Configuration;
        return entityTypeConfiguration ?? _defaultConfiguration;
    }
}
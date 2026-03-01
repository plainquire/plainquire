using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Filter.Mvc.ModelBinders;

/// <summary>
/// ModelBinder for <see cref="EntityFilter{TEntity}"/>
/// Implements <see cref="IModelBinder" />
/// </summary>
/// <seealso cref="IModelBinder" />
public class EntityFilterModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var request = bindingContext.ActionContext.HttpContext.Request;

        var filteredType = bindingContext.ModelType.GetGenericArguments()[0];
        var serviceProvider = bindingContext.ActionContext.HttpContext.RequestServices;
        var configuration = GetFilterConfiguration(filteredType, serviceProvider);
        var entityFilter = EntityFilterFactory.Create(filteredType, configuration);

        var filters = request.Query
            .SelectMany(kvp => kvp.Value.Select(value => new KeyValuePair<string, string?>(kvp.Key, value)))
            .ToArray();

        entityFilter.ApplyFromSyntax(filteredType, filters);

        bindingContext.Result = ModelBindingResult.Success(entityFilter);

        return Task.CompletedTask;
    }

    private static FilterConfiguration? GetFilterConfiguration(Type? entityType, IServiceProvider serviceProvider)
    {
        if (entityType == null)
            return null;

        var entityFilterType = typeof(EntityFilter<>).MakeGenericType(entityType);
        var prototypeConfiguration = ((EntityFilter?)serviceProvider.GetService(entityFilterType))?.Configuration;
        var injectedConfiguration = serviceProvider.GetService<IOptions<FilterConfiguration>>()?.Value;

        var configuration = prototypeConfiguration ?? injectedConfiguration;
        return configuration;
    }
}
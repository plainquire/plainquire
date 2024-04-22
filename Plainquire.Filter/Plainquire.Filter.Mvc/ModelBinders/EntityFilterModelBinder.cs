using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plainquire.Filter.Abstractions;
using System;
using System.Reflection;
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

        var serviceProvider = bindingContext.ActionContext.HttpContext.RequestServices;
        var filteredType = bindingContext.ModelType.GetGenericArguments()[0];

        var filterableProperties = filteredType.GetFilterableProperties();
        var entityFilterAttribute = filteredType.GetCustomAttribute<FilterEntityAttribute>();
        var entityFilter = CreateEntityFilter(filteredType, serviceProvider);

        foreach (var property in filterableProperties)
        {
            var parameterName = property.GetFilterParameterName(entityFilterAttribute?.Prefix);
            var parameterValues = bindingContext.ValueProvider.GetValue(parameterName);
            foreach (var filterSyntax in parameterValues)
                entityFilter.PropertyFilters.Add(new PropertyFilter(property.Name, ValueFiltersFactory.Create(filterSyntax, entityFilter.Configuration)));
        }

        bindingContext.Result = ModelBindingResult.Success(entityFilter);
        return Task.CompletedTask;
    }

    private static EntityFilter CreateEntityFilter(Type? type, IServiceProvider serviceProvider)
    {
        if (type == null)
            return new EntityFilter();

        var entityPageType = typeof(EntityFilter<>).MakeGenericType(type);
        var entityPage = (EntityFilter)Activator.CreateInstance(entityPageType)!;

        var prototypeConfiguration = ((EntityFilter?)serviceProvider.GetService(entityPageType))?.Configuration;
        var injectedConfiguration = serviceProvider.GetService<IOptions<FilterConfiguration>>()?.Value;
        entityPage.Configuration = prototypeConfiguration ?? injectedConfiguration;

        return entityPage;
    }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Schick.Plainquire.Filter.Abstractions.Attributes;
using Schick.Plainquire.Filter.Extensions;
using Schick.Plainquire.Filter.Filters;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Schick.Plainquire.Filter.Mvc.ModelBinders;

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

        var filteredType = bindingContext.ModelType.GetGenericArguments()[0];
        var entityFilterType = typeof(EntityFilter<>).MakeGenericType(filteredType);

        var filterableProperties = filteredType.GetFilterableProperties();
        var entityFilterAttribute = filteredType.GetCustomAttribute<FilterEntityAttribute>();
        var entityFilter = (EntityFilter)Activator.CreateInstance(entityFilterType)!;

        foreach (var property in filterableProperties)
        {
            var parameterName = property.GetFilterParameterName(entityFilterAttribute?.Prefix);
            var parameterValues = bindingContext.ValueProvider.GetValue(parameterName);
            foreach (var filterSyntax in parameterValues)
                entityFilter.PropertyFilters.Add(new PropertyFilter(property.Name, ValueFiltersFactory.Create(filterSyntax)));
        }

        bindingContext.Result = ModelBindingResult.Success(entityFilter);
        return Task.CompletedTask;
    }
}
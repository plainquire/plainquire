using FS.FilterExpressionCreator.Abstractions.Attributes;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace FS.FilterExpressionCreator.Mvc.ModelBinders;

/// <summary>
/// ModelBinder for <see cref="EntityFilter{TEntity}"/>
/// Implements <see cref="IModelBinder" />
/// </summary>
/// <seealso cref="IModelBinder" />
[SuppressMessage("ReSharper", "InconsistentNaming")]
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
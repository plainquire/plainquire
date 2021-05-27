using FilterExpressionCreator.Models;
using FilterExpressionCreator.Mvc.Attributes;
using FilterExpressionCreator.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FilterExpressionCreator.Mvc.ModelBinders
{
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
            var entityFilter = (EntityFilter)Activator.CreateInstance(entityFilterType);

            foreach (var property in filterableProperties)
            {
                var parameterName = property.GetFilterParameterName(entityFilterAttribute?.Prefix);
                var parameterValues = bindingContext.ValueProvider.GetValue(parameterName);
                foreach (var filterSyntax in parameterValues)
                    entityFilter!.PropertyFilters.Add(new EntityFilter.PropertyFilter(property.Name, ValueFilter.Create(filterSyntax)));
            }

            bindingContext.Result = ModelBindingResult.Success(entityFilter);
            return Task.CompletedTask;
        }
    }
}

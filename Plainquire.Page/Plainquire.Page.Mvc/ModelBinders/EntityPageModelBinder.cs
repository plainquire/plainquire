using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Plainquire.Filter.Abstractions.Attributes;
using Plainquire.Page.Pages;

namespace Plainquire.Page.Mvc.ModelBinders;

/// <summary>
/// ModelBinder for <see cref="EntityPage{TEntity}"/>
/// Implements <see cref="IModelBinder" />
/// </summary>
/// <seealso cref="IModelBinder" />
public class EntityPageModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var pageType = (Type?)null;
        var pageParameterName = FilterEntityAttribute.DEFAULT_PAGE_NUMBER_PARAMETER_NAME;
        var pageSizeParameterName = FilterEntityAttribute.DEFAULT_PAGE_SIZE_PARAMETER_NAME;

        var isGenericPage = bindingContext.ModelType.IsGenericType;
        if (isGenericPage)
        {
            pageType = bindingContext.ModelType.GetGenericArguments()[0];
            var entityFilterAttribute = pageType.GetCustomAttribute<FilterEntityAttribute>();
            pageParameterName = entityFilterAttribute?.PageNumberParameter ?? FilterEntityAttribute.DEFAULT_PAGE_NUMBER_PARAMETER_NAME;
            pageSizeParameterName = entityFilterAttribute?.PageSizeParameter ?? FilterEntityAttribute.DEFAULT_PAGE_SIZE_PARAMETER_NAME;
        }

        var pageParameterValue = bindingContext.HttpContext.Request.Query.Keys
            .Where(queryParameter => queryParameter == pageParameterName)
            .Select(queryParameter => GetParameterValue(queryParameter, bindingContext))
            .FirstOrDefault();

        var pageSizeParameterValue = bindingContext.HttpContext.Request.Query.Keys
            .Where(queryParameter => queryParameter == pageSizeParameterName)
            .Select(queryParameter => GetParameterValue(queryParameter, bindingContext))
            .FirstOrDefault();

        var entityPage = CreateEntityPage(pageType, pageParameterValue, pageSizeParameterValue);
        bindingContext.Result = ModelBindingResult.Success(entityPage);
        return Task.CompletedTask;
    }

    private static string? GetParameterValue(string queryParameter, ModelBindingContext bindingContext)
        => bindingContext.ValueProvider.GetValue(queryParameter).FirstValue;

    private static EntityPage CreateEntityPage(Type? pageType, string? page, string? pageSize)
    {
        if (pageType == null)
        {
            return new EntityPage
            {
                PageNumberValue = page ?? string.Empty,
                PageSizeValue = pageSize ?? string.Empty
            };
        }

        if (string.IsNullOrEmpty(pageSize))
        {
            var entityFilterAttribute = pageType.GetCustomAttribute<FilterEntityAttribute>();
            if (entityFilterAttribute?.PageSize != 0)
                pageSize = entityFilterAttribute?.PageSize.ToString();
        }

        var entityPageType = typeof(EntityPage<>).MakeGenericType(pageType);
        var entityPage = (EntityPage)Activator.CreateInstance(entityPageType)!;
        entityPage.PageNumberValue = page ?? string.Empty;
        entityPage.PageSizeValue = pageSize ?? string.Empty;

        return entityPage;
    }
}

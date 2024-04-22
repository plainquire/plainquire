

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plainquire.Filter.Abstractions;
using Plainquire.Page.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        var serviceProvider = bindingContext.ActionContext.HttpContext.RequestServices;

        var isGenericPage = bindingContext.ModelType.IsGenericType;
        var pageType = isGenericPage ? bindingContext.ModelType.GenericTypeArguments[0] : null;
        var entityPage = CreateEntityPage(pageType, serviceProvider);

        if (bindingContext.ModelMetadata.ParameterName == null)
            throw new InvalidOperationException("Unable to get original parameter name.");

        var (pageParameterName, pageSizeParameterName) = ParameterExtensions
            .GetPageParameterNames(bindingContext.ModelMetadata.ParameterName, bindingContext.OriginalModelName);

        var pageParameterValue = bindingContext.HttpContext.Request.Query.Keys
            .Where(queryParameter => queryParameter == pageParameterName)
            .Select(queryParameter => GetParameterValue(queryParameter, bindingContext))
            .FirstOrDefault();

        var pageSizeParameterValue = bindingContext.HttpContext.Request.Query.Keys
            .Where(queryParameter => queryParameter == pageSizeParameterName)
            .Select(queryParameter => GetParameterValue(queryParameter, bindingContext))
            .FirstOrDefault();

        var pageSizeFromFilterAttribute = pageType?
            .GetCustomAttribute<FilterEntityAttribute>()?
            .PageSize
            .ToString();

        entityPage.PageNumberValue = pageParameterValue ?? string.Empty;
        entityPage.PageSizeValue = pageSizeParameterValue ?? pageSizeFromFilterAttribute ?? string.Empty;

        bindingContext.Result = ModelBindingResult.Success(entityPage);

        return Task.CompletedTask;
    }

    private static string? GetParameterValue(string queryParameter, ModelBindingContext bindingContext)
        => bindingContext.ValueProvider.GetValue(queryParameter).FirstValue;

    private static EntityPage CreateEntityPage(Type? pageType, IServiceProvider serviceProvider)
    {
        if (pageType == null)
            return new EntityPage();

        var entityPageType = typeof(EntityPage<>).MakeGenericType(pageType);
        var entityPage = (EntityPage)Activator.CreateInstance(entityPageType)!;

        var prototypeConfiguration = ((EntityPage?)serviceProvider.GetService(entityPageType))?.Configuration;
        var injectedConfiguration = serviceProvider.GetService<IOptions<PageConfiguration>>()?.Value;
        entityPage.Configuration = prototypeConfiguration ?? injectedConfiguration;

        return entityPage;
    }
}

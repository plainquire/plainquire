using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plainquire.Filter.Abstractions;
using Plainquire.Sort.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Plainquire.Sort.Mvc.ModelBinders;

/// <summary>
/// ModelBinder for <see cref="EntitySort{TEntity}"/>
/// Implements <see cref="IModelBinder" />
/// </summary>
/// <seealso cref="IModelBinder" />
public class EntitySortModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var serviceProvider = bindingContext.ActionContext.HttpContext.RequestServices;

        var sortedType = bindingContext.ModelType.GetGenericArguments()[0];
        var entitySort = CreateEntitySort(sortedType, serviceProvider);

        var entitySortConfiguration = entitySort.Configuration;
        var configuration = entitySortConfiguration ?? SortConfiguration.Default ?? new SortConfiguration();

        var sortByParameterName = bindingContext.OriginalModelName;
        var sortByParameterValues = bindingContext.HttpContext.Request.Query.Keys
            .Where(queryParameter => IsSortByParameter(queryParameter, sortByParameterName))
            .SelectMany(queryParameter => GetParameterValues(queryParameter, bindingContext))
            .SelectMany(value => value.SplitCommaSeparatedValues())
            .ToList();

        var entityEntitySort = entitySort.Apply(sortByParameterValues, configuration);
        bindingContext.Result = ModelBindingResult.Success(entityEntitySort);
        return Task.CompletedTask;
    }

    private static bool IsSortByParameter(string queryParameterName, string sortByParameterName)
        => Regex.IsMatch(queryParameterName, @$"{sortByParameterName}(\[\d*\])?", RegexOptions.IgnoreCase, RegexDefaults.Timeout);

    private static ValueProviderResult GetParameterValues(string queryParameter, ModelBindingContext bindingContext)
        => bindingContext.ValueProvider.GetValue(queryParameter);

    private static EntitySort CreateEntitySort(Type sortedType, IServiceProvider serviceProvider)
    {
        var entitySortType = typeof(EntitySort<>).MakeGenericType(sortedType);
        var entitySortInstance = Activator.CreateInstance(entitySortType)
            ?? throw new InvalidOperationException($"Unable to create instance of type {entitySortType.Name}");

        var entitySort = (EntitySort)entitySortInstance;

        var prototypeConfiguration = ((EntitySort?)serviceProvider.GetService(entitySortType))?.Configuration;
        var injectedConfiguration = serviceProvider.GetService<IOptions<SortConfiguration>>()?.Value;
        entitySort.Configuration = prototypeConfiguration ?? injectedConfiguration;

        return entitySort;
    }
}

file static class Extensions
{
    public static EntitySort Apply(this EntitySort entitySort, IEnumerable<string> sortParameters, SortConfiguration configuration)
    {
        var sortedType = entitySort.GetType().GenericTypeArguments[0];
        var entityFilterAttribute = sortedType.GetCustomAttribute<EntityFilterAttribute>();

        var sortablePropertyNameToParameterMap = sortedType
            .GetSortableProperties()
            .Select(property => new PropertyNameToParameterMap(
                PropertyName: property.Name,
                ParameterName: property.GetSortParameterName(entityFilterAttribute?.Prefix)
            ))
            .ToList();

        var propertySorts = sortParameters
            .Select(parameter => MapToPropertyPath(parameter, sortablePropertyNameToParameterMap, configuration))
            .Select((propertyPath, index) => propertyPath != null ? PropertySort.Create(propertyPath, index, configuration) : null)
            .WhereNotNull()
            .ToList();

        foreach (var propertySort in propertySorts)
            entitySort.PropertySorts.Add(propertySort);

        return entitySort;
    }

    private static string? MapToPropertyPath(string parameter, IReadOnlyCollection<PropertyNameToParameterMap> sortableProperties, SortConfiguration configuration)
    {
        var sortSyntaxMatch = Regex.Match(parameter, configuration.SortDirectionPattern, RegexOptions.IgnoreCase, RegexDefaults.Timeout);
        if (!sortSyntaxMatch.Success)
            return null;

        var prefix = sortSyntaxMatch.Groups["prefix"].Value;
        var propertyPath = sortSyntaxMatch.Groups["propertyPath"].Value;
        var postfix = sortSyntaxMatch.Groups["postfix"].Value;

        var propertyPathSegments = propertyPath
            .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var primaryParameterName = propertyPathSegments.FirstOrDefault();

        var property = sortableProperties.FirstOrDefault(x => x.ParameterName.EqualsOrdinal(primaryParameterName)) ??
                       sortableProperties.FirstOrDefault(x => x.ParameterName.Equals(primaryParameterName, StringComparison.OrdinalIgnoreCase));

        if (property == null)
            return null;

        propertyPathSegments[0] = property.PropertyName;

        return prefix + string.Join('.', propertyPathSegments) + postfix;
    }

    [ExcludeFromCodeCoverage]
    private record PropertyNameToParameterMap(string PropertyName, string ParameterName);
}

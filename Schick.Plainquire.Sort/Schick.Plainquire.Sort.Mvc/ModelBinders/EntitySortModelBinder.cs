using Schick.Plainquire.Filter.Abstractions.Attributes;
using Schick.Plainquire.Filter.Abstractions.Extensions;
using Schick.Plainquire.Sort.Extensions;
using Schick.Plainquire.Sort.Mvc.Extensions;
using Schick.Plainquire.Sort.Sorts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Schick.Plainquire.Sort.Mvc.ModelBinders;

/// <summary>
/// ModelBinder for <see cref="EntitySort{TEntity}"/>
/// Implements <see cref="IModelBinder" />
/// </summary>
/// <seealso cref="IModelBinder" />
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class EntitySortModelBinder : IModelBinder
{
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var sortedType = bindingContext.ModelType.GetGenericArguments()[0];
        var entityFilterAttribute = sortedType.GetCustomAttribute<FilterEntityAttribute>();
        var sortByParameterName = entityFilterAttribute?.SortByParameter ?? FilterEntityAttribute.DEFAULT_SORT_BY_PARAMETER_NAME;

        var sortByParameterValues = bindingContext.HttpContext.Request.Query.Keys
            .Where(queryParameter => IsSortByParameter(queryParameter, sortByParameterName))
            .SelectMany(queryParameter => GetParameterValues(queryParameter, bindingContext))
            .SelectMany(value => value.SplitCommaSeparatedValues())
            .ToList();

        var entityEntitySort = CreateEntitySort(sortedType, sortByParameterValues);
        bindingContext.Result = ModelBindingResult.Success(entityEntitySort);
        return Task.CompletedTask;
    }

    private static bool IsSortByParameter(string queryParameterName, string sortByParameterName)
        => Regex.IsMatch(queryParameterName, @$"{sortByParameterName}(\[\d*\])?");

    private static ValueProviderResult GetParameterValues(string queryParameter, ModelBindingContext bindingContext)
        => bindingContext.ValueProvider.GetValue(queryParameter);

    private static EntitySort CreateEntitySort(Type sortedType, IEnumerable<string> sortParameters)
    {
        var entityFilterAttribute = sortedType.GetCustomAttribute<FilterEntityAttribute>();

        var sortablePropertyNameToParameterMap = sortedType
            .GetSortableProperties()
            .Select(property => new PropertyNameToParameterMap(
                PropertyName: property.Name,
                ParameterName: property.GetSortQueryableParameterName(entityFilterAttribute?.Prefix)
            ))
            .ToList();

        var propertySorts = sortParameters
            .Select(parameter => MapToPropertyPath(parameter, sortablePropertyNameToParameterMap))
            .Select((propertyPath, index) => propertyPath != null ? PropertySort.Create(propertyPath, index) : null)
            .WhereNotNull()
            .ToList();

        var entitySortType = typeof(EntitySort<>).MakeGenericType(sortedType);
        var entityEntitySort = (EntitySort)Activator.CreateInstance(entitySortType)!;
        foreach (var propertySort in propertySorts)
            entityEntitySort._propertySorts.Add(propertySort);

        return entityEntitySort;
    }

    private static string? MapToPropertyPath(string parameter, IReadOnlyCollection<PropertyNameToParameterMap> sortableProperties)
    {
        var sortSyntaxMatch = Regex.Match(parameter, PropertySort._sortSyntaxPattern, RegexOptions.IgnoreCase);
        if (!sortSyntaxMatch.Success)
            return null;

        var prefix = sortSyntaxMatch.Groups["prefix"].Value;
        var propertyPath = sortSyntaxMatch.Groups["propertyPath"].Value;
        var postfix = sortSyntaxMatch.Groups["postfix"].Value;

        var propertyPathSegments = propertyPath
            .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var primaryParameterName = propertyPathSegments.FirstOrDefault();

        var property = sortableProperties.FirstOrDefault(x => x.ParameterName.Equals(primaryParameterName)) ??
                       sortableProperties.FirstOrDefault(x => x.ParameterName.Equals(primaryParameterName, StringComparison.OrdinalIgnoreCase));

        if (property == null)
            return null;

        propertyPathSegments[0] = property.PropertyName;

        return prefix + string.Join('.', propertyPathSegments) + postfix;
    }

    [ExcludeFromCodeCoverage]
    private record PropertyNameToParameterMap(string PropertyName, string ParameterName);
}

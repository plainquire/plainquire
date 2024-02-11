using Schick.Plainquire.Filter.Abstractions.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Schick.Plainquire.Sort.Mvc.ModelBinders;

/// <summary>
/// Model binding provider for <see cref="EntitySortSetModelBinder"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class EntitySortSetModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var entitySortAttribute = context.Metadata.ModelType.GetCustomAttribute<EntitySortSetAttribute>();
        if (entitySortAttribute == null)
            return null;

        var entityFilterBinders = context.Metadata.ModelType
            .GetProperties()
            .Select(property => GetModelBinder(property, context))
            .ToDictionary(x => x.Type, x => (x.Metadata, x.Binder));

        return new EntitySortSetModelBinder(entityFilterBinders);
    }

    private static (Type Type, ModelMetadata Metadata, IModelBinder Binder) GetModelBinder(PropertyInfo property, ModelBinderProviderContext context)
    {
        var modelMetadata = context.MetadataProvider.GetMetadataForType(property.PropertyType);
        var modelBinder = context.CreateBinder(modelMetadata);
        return (property.PropertyType, modelMetadata, modelBinder);
    }
}
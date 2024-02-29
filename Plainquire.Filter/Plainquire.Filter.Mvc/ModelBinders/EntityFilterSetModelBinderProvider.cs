using Microsoft.AspNetCore.Mvc.ModelBinding;
using Plainquire.Filter.Abstractions;
using System;
using System.Linq;
using System.Reflection;

namespace Plainquire.Filter.Mvc.ModelBinders;

/// <summary>
/// Model binding provider for <see cref="EntityFilterSetModelBinder"/>
/// </summary>
public class EntityFilterSetModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var entityFilterSetAttribute = context.Metadata.ModelType.GetCustomAttribute<EntityFilterSetAttribute>();
        if (entityFilterSetAttribute == null)
            return null;

        var entityFilterBinders = context.Metadata.ModelType
            .GetProperties()
            .Select(property => GetModelBinder(property, context))
            .ToDictionary(x => x.Type, x => (x.Metadata, x.Binder));

        return new EntityFilterSetModelBinder(entityFilterBinders);
    }

    private static (Type Type, ModelMetadata Metadata, IModelBinder Binder) GetModelBinder(PropertyInfo property, ModelBinderProviderContext context)
    {
        var modelMetadata = context.MetadataProvider.GetMetadataForType(property.PropertyType);
        var modelBinder = context.CreateBinder(modelMetadata);
        return (property.PropertyType, modelMetadata, modelBinder);
    }
}
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Plainquire.Page.Mvc.ModelBinders;

/// <summary>
/// Model binding provider for <see cref="EntityPageSetModelBinder"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class EntityPageSetModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var entityPageAttribute = context.Metadata.ModelType.GetCustomAttribute<EntityPageSetAttribute>();
        if (entityPageAttribute == null)
            return null;

        var entityFilterBinders = context.Metadata.ModelType
            .GetProperties()
            .Select(property => GetModelBinder(property, context))
            .ToDictionary(x => x.Type, x => (x.Metadata, x.Binder));

        return new EntityPageSetModelBinder(entityFilterBinders);
    }

    private static (Type Type, ModelMetadata Metadata, IModelBinder Binder) GetModelBinder(PropertyInfo property, ModelBinderProviderContext context)
    {
        var modelMetadata = context.MetadataProvider.GetMetadataForType(property.PropertyType);
        var modelBinder = context.CreateBinder(modelMetadata);
        return (property.PropertyType, modelMetadata, modelBinder);
    }
}
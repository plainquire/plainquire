using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Mvc.ModelBinders;

/// <summary>
/// Model binding provider for <see cref="EntitySortModelBinder"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class EntitySortModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.Metadata.ModelType.IsGenericEntitySort()
            ? new EntitySortModelBinder()
            : null;
    }
}
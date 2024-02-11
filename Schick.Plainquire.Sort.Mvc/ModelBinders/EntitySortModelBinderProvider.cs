using Schick.Plainquire.Sort.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Sort.Mvc.ModelBinders;

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
            ? new BinderTypeModelBinder(typeof(EntitySortModelBinder))
            : null;
    }
}
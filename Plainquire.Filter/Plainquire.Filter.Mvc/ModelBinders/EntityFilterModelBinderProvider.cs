using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Plainquire.Filter.Extensions;
using System;

namespace Plainquire.Filter.Mvc.ModelBinders;

/// <summary>
/// Model binding provider for <see cref="EntityFilterModelBinder"/>
/// </summary>
public class EntityFilterModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.Metadata.ModelType.IsGenericEntityFilter()
            ? new BinderTypeModelBinder(typeof(EntityFilterModelBinder))
            : null;
    }
}
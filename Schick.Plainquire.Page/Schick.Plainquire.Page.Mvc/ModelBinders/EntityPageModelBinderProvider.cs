using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Schick.Plainquire.Page.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Page.Mvc.ModelBinders;

/// <summary>
/// Model binding provider for <see cref="EntityPageModelBinder"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class EntityPageModelBinderProvider : IModelBinderProvider
{
    /// <inheritdoc />
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.Metadata.ModelType.IsEntityPage()
            ? new BinderTypeModelBinder(typeof(EntityPageModelBinder))
            : null;
    }
}
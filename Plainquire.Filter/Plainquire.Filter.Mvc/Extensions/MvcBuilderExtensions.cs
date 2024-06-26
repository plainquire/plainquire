﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Filter.Mvc.ModelBinders;

namespace Plainquire.Filter.Mvc;

/// <summary>
/// Extensions to register entity filter extensions to MVC
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Registers filter expression specific model binders.
    /// </summary>
    /// <param name="mvcBuilder">The MVC builder.</param>
    /// <autogeneratedoc />
    public static IMvcBuilder AddFilterSupport(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new EntityFilterModelBinderProvider());
            options.ModelBinderProviders.Insert(0, new EntityFilterSetModelBinderProvider());
        });

        return mvcBuilder;
    }
}
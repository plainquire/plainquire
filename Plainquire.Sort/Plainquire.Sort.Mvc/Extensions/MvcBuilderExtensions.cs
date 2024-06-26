﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Sort.Mvc.ModelBinders;

namespace Plainquire.Sort.Mvc;

/// <summary>
/// Extensions to register entity filter extensions to MVC
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Registers sort queryable specific model binders.
    /// </summary>
    /// <param name="mvcBuilder">The MVC builder.</param>
    public static IMvcBuilder AddSortSupport(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new EntitySortModelBinderProvider());
            options.ModelBinderProviders.Insert(0, new EntitySortSetModelBinderProvider());
        });

        return mvcBuilder;
    }
}
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Page.Mvc.ModelBinders;

namespace Plainquire.Page.Mvc;

/// <summary>
/// Extensions to register entity filter extensions to MVC
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Registers page queryable specific model binders.
    /// </summary>
    /// <param name="mvcBuilder">The MVC builder.</param>
    /// <autogeneratedoc />
    public static IMvcBuilder AddPageSupport(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new EntityPageModelBinderProvider());
        });

        return mvcBuilder;
    }
}
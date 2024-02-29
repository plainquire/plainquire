﻿using Microsoft.Extensions.DependencyInjection;
using Plainquire.Sort.Swashbuckle.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Plainquire.Sort.Swashbuckle;

/// <summary>
/// Extensions to register entity filter extensions to Swashbuckle.AspNetCore (https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
/// </summary>
/// <autogeneratedoc />
public static class SwaggerGenOptionsExtensions
{
    /// <summary>
    /// Replaces action parameters of type <see cref="EntitySort{TEntity}"/> with filterable properties of type <c>TEntity</c>.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="xmlDocumentationFilePaths">Paths to XML documentation files. Used to provide parameter descriptions.</param>
    public static SwaggerGenOptions AddSortSupport(this SwaggerGenOptions options, params string[] xmlDocumentationFilePaths)
    {
        options.OperationFilter<EntitySortParameterReplacer>();
        options.OperationFilter<EntitySortSetParameterReplacer>();
        return options;
    }
}
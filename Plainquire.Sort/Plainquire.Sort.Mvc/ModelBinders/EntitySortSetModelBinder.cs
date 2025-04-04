﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Plainquire.Sort.Mvc.ModelBinders;

/// <summary>
/// ModelBinder for <see cref="EntitySort{TEntity}"/>
/// Implements <see cref="IModelBinder" />
/// </summary>
/// <seealso cref="IModelBinder" />
[ExcludeFromCodeCoverage]
public class EntitySortSetModelBinder : IModelBinder
{
    private readonly IDictionary<Type, (ModelMetadata, IModelBinder)> _entitySortBinders;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntitySortSetModelBinder"/> class.
    /// </summary>
    /// <param name="entitySortBinders">The entity sort binders.</param>
    /// <autogeneratedoc />
    public EntitySortSetModelBinder(IDictionary<Type, (ModelMetadata, IModelBinder)> entitySortBinders)
        => this._entitySortBinders = entitySortBinders;

    /// <inheritdoc />
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var entitySortSet = Activator.CreateInstance(bindingContext.ModelType)
            ?? throw new InvalidOperationException($"Cannot create instance of sort set '{bindingContext.ModelType.Name}'");

        var entitySortSetProperties = entitySortSet.GetType().GetProperties();
        foreach (var entitySort in entitySortSetProperties)
        {
            var (modelMetadata, modelBinder) = _entitySortBinders[entitySort.PropertyType];

            var entitySortBindingContext = DefaultModelBindingContext
                .CreateBindingContext(
                    bindingContext.ActionContext,
                    bindingContext.ValueProvider,
                    modelMetadata,
                    bindingInfo: null,
                    bindingContext.OriginalModelName
                );

            await modelBinder.BindModelAsync(entitySortBindingContext);

            entitySort.SetValue(entitySortSet, entitySortBindingContext.Result.Model);
        }

        bindingContext.Result = ModelBindingResult.Success(entitySortSet);
    }
}
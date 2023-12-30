using FS.FilterExpressionCreator.Abstractions.Configurations;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using System.Collections.Generic;

namespace FS.FilterExpressionCreator.Tests.Models;

// TODO: Compare with FS.SortQueryableCreator.Tests and check if this is still needed.
public delegate List<TEntity> EntityFilterFunc<TEntity>(ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration? configuration = null, IPropertyFilterInterceptor? interceptor = null);

public delegate List<TestModel<TModelValue>> TestModelFilterFunc<TModelValue>(ICollection<TestModel<TModelValue>> testItems, EntityFilter<TestModel<TModelValue>> filter, FilterConfiguration? configuration = null, IPropertyFilterInterceptor? interceptor = null);
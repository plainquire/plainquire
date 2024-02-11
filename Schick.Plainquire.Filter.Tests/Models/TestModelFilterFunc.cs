using Schick.Plainquire.Filter.Abstractions.Configurations;
using Schick.Plainquire.Filter.Filters;
using Schick.Plainquire.Filter.Interfaces;
using System.Collections.Generic;

namespace Schick.Plainquire.Filter.Tests.Models;

// TODO: Compare with Schick.Plainquire.Sort.Tests and check if this is still needed.
public delegate List<TEntity> EntityFilterFunc<TEntity>(ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration? configuration = null, IPropertyFilterInterceptor? interceptor = null);

public delegate List<TestModel<TModelValue>> TestModelFilterFunc<TModelValue>(ICollection<TestModel<TModelValue>> testItems, EntityFilter<TestModel<TModelValue>> filter, FilterConfiguration? configuration = null, IPropertyFilterInterceptor? interceptor = null);
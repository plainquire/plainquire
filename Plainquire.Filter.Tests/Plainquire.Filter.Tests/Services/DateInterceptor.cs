using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Plainquire.Filter.Tests.Services;

[ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global", Justification = "Provided as library, can be used from outside")]
internal class DateInterceptor : IFilterInterceptor
{
    public Func<DateTimeOffset> Now { get; set; } = () => DateTimeOffset.Now;

    public Expression<Func<TEntity, bool>>? CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, IEnumerable<ValueFilter> filters, FilterConfiguration configuration)
        => null;
}
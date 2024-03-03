using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Plainquire.Filter.Tests.Services;

[ExcludeFromCodeCoverage]
internal class DateInterceptor : IFilterInterceptor
{
    public Func<DateTimeOffset> Now { get; set; } = () => DateTimeOffset.Now;

    public Expression<Func<TEntity, bool>>? CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, ValueFilter[] filters, FilterConfiguration configuration)
        => null;
}
using Plainquire.Filter.Abstractions;
using System;

namespace Plainquire.Filter.Tests.Models;

public class FilterTestCase<TFilterValue, TModelValue> : FilterTestCase
{
    public FilterOperator FilterOperator { get; private init; }
    public TFilterValue[]? FilterValues { get; private init; }
    public string? FilterSyntax { get; private init; }
    public Exception? ExpectedException { get; private init; }
    public Func<TModelValue?, bool>? ExpectedTestItemsExpression { get; private init; }
    public FilterConfiguration? FilterConfiguration { get; private init; }
    public IFilterInterceptor? Interceptor { get; private init; }

    private FilterTestCase(int id, FilterConfiguration? syntaxConfiguration)
        : base(id)
        => FilterConfiguration = syntaxConfiguration;

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, FilterOperator filterOperator, TFilterValue[] values, Func<TModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => new(id, syntaxConfiguration)
        {
            FilterOperator = filterOperator,
            FilterValues = values,
            ExpectedTestItemsExpression = expectedTestItemsExpression,
            Interceptor = interceptor
        };

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, FilterOperator filterOperator, TFilterValue[] values, Exception? expectedException = null, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => new(id, syntaxConfiguration)
        {
            FilterOperator = filterOperator,
            FilterValues = values,
            ExpectedException = expectedException,
            Interceptor = interceptor
        };

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, string filterSyntax, Func<TModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => new(id, syntaxConfiguration)
        {
            FilterSyntax = filterSyntax,
            ExpectedTestItemsExpression = expectedTestItemsExpression,
            Interceptor = interceptor
        };

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, string filterSyntax, Exception? expectedException = null, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => new(id, syntaxConfiguration)
        {
            FilterSyntax = filterSyntax,
            ExpectedException = expectedException,
            Interceptor = interceptor
        };
}

public class FilterTestCase
{
    public int Id { get; }

    protected FilterTestCase(int id)
        => Id = id;

    public static FilterTestCase<TFilterValue?, TModelValue?> Create<TFilterValue, TModelValue>(int id, FilterOperator filterOperator, TFilterValue[] values, Func<TModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => FilterTestCase<TFilterValue?, TModelValue?>.Create(id, filterOperator, values, expectedTestItemsExpression, syntaxConfiguration, interceptor);

    public static FilterTestCase<TFilterAndModelValue?, TFilterAndModelValue?> Create<TFilterAndModelValue>(int id, FilterOperator filterOperator, TFilterAndModelValue[] values, Exception? expectedException = null, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => FilterTestCase<TFilterAndModelValue?, TFilterAndModelValue?>.Create(id, filterOperator, values, expectedException, syntaxConfiguration, interceptor);

    public static FilterTestCase<TFilterAndModelValue?, TFilterAndModelValue?> Create<TFilterAndModelValue>(int id, string filterSyntax, Func<TFilterAndModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => FilterTestCase<TFilterAndModelValue?, TFilterAndModelValue?>.Create(id, filterSyntax, expectedTestItemsExpression, syntaxConfiguration, interceptor);

    public static FilterTestCase<TFilterAndModelValue?, TFilterAndModelValue?> Create<TFilterAndModelValue>(int id, string filterSyntax, Exception? expectedException = null, FilterConfiguration? syntaxConfiguration = null, IFilterInterceptor? interceptor = null)
        => FilterTestCase<TFilterAndModelValue?, TFilterAndModelValue?>.Create(id, filterSyntax, expectedException, syntaxConfiguration, interceptor);
}
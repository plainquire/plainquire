using Plainquire.Filter.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Filter.Tests.Models;

[ExcludeFromCodeCoverage]
public class FilterTestCase<TFilterValue, TModelValue> : FilterTestCase
{
    public FilterOperator FilterOperator { get; private init; }
    public TFilterValue[]? FilterValues { get; private init; }
    public string? FilterSyntax { get; private init; }
    public Exception? ExpectedException { get; private init; }
    public Func<TModelValue?, bool>? ExpectedTestItemsExpression { get; private init; }
    public FilterConfiguration? FilterConfiguration { get; }
    public SyntaxConfiguration? SyntaxConfiguration { get; }

    private FilterTestCase(int id, FilterConfiguration? filterConfiguration, SyntaxConfiguration? syntaxConfiguration)
        : base(id)
    {
        FilterConfiguration = filterConfiguration;
        SyntaxConfiguration = syntaxConfiguration;
    }

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, FilterOperator filterOperator, TFilterValue[] values, Func<TModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => new(id, filterConfiguration, syntaxConfiguration)
        {
            FilterOperator = filterOperator,
            FilterValues = values,
            ExpectedTestItemsExpression = expectedTestItemsExpression,
        };

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, FilterOperator filterOperator, TFilterValue[] values, Exception? expectedException = null, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => new(id, filterConfiguration, syntaxConfiguration)
        {
            FilterOperator = filterOperator,
            FilterValues = values,
            ExpectedException = expectedException
        };

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, string filterSyntax, Func<TModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => new(id, filterConfiguration, syntaxConfiguration)
        {
            FilterSyntax = filterSyntax,
            ExpectedTestItemsExpression = expectedTestItemsExpression
        };

    public static FilterTestCase<TFilterValue, TModelValue> Create(int id, string filterSyntax, Exception? expectedException = null, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => new(id, filterConfiguration, syntaxConfiguration)
        {
            FilterSyntax = filterSyntax,
            ExpectedException = expectedException,
        };
}

[ExcludeFromCodeCoverage]
public class FilterTestCase
{
    public int Id { get; }

    protected FilterTestCase(int id)
        => Id = id;

    public static FilterTestCase<TFilterValue, TModelValue?> Create<TFilterValue, TModelValue>(int id, FilterOperator filterOperator, TFilterValue[] values, Func<TModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => FilterTestCase<TFilterValue, TModelValue?>.Create(id, filterOperator, values, expectedTestItemsExpression, filterConfiguration, syntaxConfiguration);

    public static FilterTestCase<TFilterAndModelValue, TFilterAndModelValue> Create<TFilterAndModelValue>(int id, FilterOperator filterOperator, TFilterAndModelValue[] values, Exception? expectedException = null, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => FilterTestCase<TFilterAndModelValue, TFilterAndModelValue>.Create(id, filterOperator, values, expectedException, filterConfiguration, syntaxConfiguration);

    public static FilterTestCase<TFilterAndModelValue, TFilterAndModelValue> Create<TFilterAndModelValue>(int id, string filterSyntax, Func<TFilterAndModelValue?, bool>? expectedTestItemsExpression, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => FilterTestCase<TFilterAndModelValue, TFilterAndModelValue>.Create(id, filterSyntax, expectedTestItemsExpression, filterConfiguration, syntaxConfiguration);

    public static FilterTestCase<TFilterAndModelValue, TFilterAndModelValue> Create<TFilterAndModelValue>(int id, string filterSyntax, Exception? expectedException = null, FilterConfiguration? filterConfiguration = null, SyntaxConfiguration? syntaxConfiguration = null)
        => FilterTestCase<TFilterAndModelValue, TFilterAndModelValue>.Create(id, filterSyntax, expectedException, filterConfiguration, syntaxConfiguration);
}
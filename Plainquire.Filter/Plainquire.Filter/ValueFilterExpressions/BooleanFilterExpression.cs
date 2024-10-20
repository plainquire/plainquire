﻿using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Plainquire.Filter.ValueFilterExpressions;

/// <inheritdoc cref="IBooleanFilterExpression"/>
public class BooleanFilterExpression : DefaultFilterExpression, IBooleanFilterExpression
{
    /// <inheritdoc />
    public override ICollection<FilterOperator> SupportedFilterOperators
        =>
        [
            FilterOperator.Default,
            FilterOperator.EqualCaseSensitive,
            FilterOperator.EqualCaseInsensitive,
            FilterOperator.NotEqual,
            FilterOperator.IsNull,
            FilterOperator.NotNull
        ];

    /// <inheritdoc />
    public override bool CanCreateExpressionFor(Type type)
        => type.GetUnderlyingType() == typeof(bool);

    /// <inheritdoc />
    protected internal override Expression? CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration, IFilterInterceptor? interceptor)
    {
        if (bool.TryParse(value, out var boolValue))
            return CreateBoolExpressionByFilterOperator(propertySelector, filterOperator, boolValue);

        var boolSyntax = configuration.BooleanMap.FirstOrDefault(x => x.Key.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        if (boolSyntax.Key != null)
            return CreateBoolExpressionByFilterOperator(propertySelector, filterOperator, boolSyntax.Value);

        if (configuration.IgnoreParseExceptions)
            return null;

        throw CreateFilterExpressionCreationException("Unable to parse given filter value", propertySelector, filterOperator, value);
    }

    private Expression CreateBoolExpressionByFilterOperator<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, bool value)
    {
        switch (filterOperator)
        {
            case FilterOperator.Default:
            case FilterOperator.EqualCaseSensitive:
            case FilterOperator.EqualCaseInsensitive:
                return CreateEqualExpression(propertySelector, value);
            case FilterOperator.NotEqual:
                return CreateNotEqualExpression(propertySelector, value);
            default:
                throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
        }
    }
}
using Schick.Plainquire.Filter.Abstractions.Configurations;
using Schick.Plainquire.Filter.Enums;
using Schick.Plainquire.Filter.Extensions;
using Schick.Plainquire.Filter.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Schick.Plainquire.Filter.ValueFilterExpression;

/// <inheritdoc cref="IBooleanFilterExpression"/>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BooleanFilterExpression : DefaultFilterExpression, IBooleanFilterExpression
{
    /// <inheritdoc />
    public override ICollection<FilterOperator> SupportedFilterOperators
        => new[]
        {
            FilterOperator.Default,
            FilterOperator.EqualCaseSensitive,
            FilterOperator.EqualCaseInsensitive,
            FilterOperator.NotEqual,
            FilterOperator.IsNull,
            FilterOperator.NotNull
        };

    /// <inheritdoc />
    public override bool CanCreateExpressionFor(Type type)
        => type.GetUnderlyingType() == typeof(bool);

    /// <inheritdoc />
    protected internal override Expression? CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration)
    {
        if (bool.TryParse(value, out var boolValue))
            return CreateBoolExpressionByFilterOperator(propertySelector, filterOperator, boolValue);

        if (configuration.BoolTrueStrings.Contains(value?.ToUpper()))
            return CreateBoolExpressionByFilterOperator(propertySelector, filterOperator, true);

        if (configuration.BoolFalseStrings.Contains(value?.ToUpper()))
            return CreateBoolExpressionByFilterOperator(propertySelector, filterOperator, false);

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
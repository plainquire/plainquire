using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Plainquire.Filter.ValueFilterExpression;

/// <inheritdoc cref="IGuidFilterExpression"/>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Provided as library, can be used from outside")]
public class GuidFilterExpression : DefaultFilterExpression, IGuidFilterExpression
{
    /// <inheritdoc />
    public override ICollection<FilterOperator> SupportedFilterOperators
        => new[]
        {
            FilterOperator.Default,
            FilterOperator.Contains,
            FilterOperator.EqualCaseSensitive,
            FilterOperator.EqualCaseInsensitive,
            FilterOperator.NotEqual,
            FilterOperator.IsNull,
            FilterOperator.NotNull
        };

    /// <inheritdoc />
    public override bool CanCreateExpressionFor(Type type)
        => type.GetUnderlyingType() == typeof(Guid);

    /// <inheritdoc />
    protected internal override Expression? CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration)
    {
        if (Guid.TryParse(value, out var guidValue))
            return CreateGuidExpressionByFilterOperator(propertySelector, filterOperator, guidValue);
        if (filterOperator == FilterOperator.Contains)
            return CreateGuidContainsExpression(propertySelector, value);

        if (configuration.IgnoreParseExceptions)
            return null;

        throw CreateFilterExpressionCreationException("Unable to parse given filter value", propertySelector, filterOperator, value);
    }

    private Expression CreateGuidExpressionByFilterOperator<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, Guid value)
    {
        switch (filterOperator)
        {
            case FilterOperator.Default:
            case FilterOperator.EqualCaseSensitive:
            case FilterOperator.EqualCaseInsensitive:
                return CreateEqualExpression(propertySelector, value);
            case FilterOperator.NotEqual:
                return CreateNotEqualExpression(propertySelector, value);
            case FilterOperator.Contains:
                return CreateGuidContainsExpression(propertySelector, value);
            // TODO: Implement LessThan/LessThanOrEqual/GreaterThan/GreaterThanOrEqual
            default:
                throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
        }
    }

    /// <summary>
    /// Creates unique identifier contains expression.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="value">The value to check for.</param>
    /// <returns>
    /// The new unique identifier contains expression.
    /// </returns>
    public static Expression CreateGuidContainsExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, object? value)
    {
        var valueToUpper = Expression.Constant(value?.ToString().ToUpper(), typeof(string));
        var propertyToString = propertySelector.Body.ObjectToString();
        var propertyToUpper = propertyToString.StringToUpper();
        var propertyContainsValue = propertyToUpper.StringContains(valueToUpper);
        return propertyContainsValue;
    }
}
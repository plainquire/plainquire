using FS.FilterExpressionCreator.Abstractions.Configurations;
using FS.FilterExpressionCreator.Abstractions.Extensions;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace FS.FilterExpressionCreator.ValueFilterExpressionCreators;

/// <inheritdoc cref="IStringFilterExpressionCreator"/>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Provided as library, can be used from outside")]
public class StringFilterExpressionCreator : DefaultFilterExpressionCreator, IStringFilterExpressionCreator
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
        => type.GetUnderlyingType() == typeof(string);

    /// <inheritdoc />
    protected internal override Expression CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration)
    {
        var strFilter = value?.Trim();
        switch (filterOperator)
        {
            case FilterOperator.Default:
            case FilterOperator.Contains:
                return CreateStringContainsExpression(propertySelector, strFilter);
            case FilterOperator.EqualCaseSensitive:
                return CreateStringCaseSensitiveEqualExpression(propertySelector, strFilter);
            case FilterOperator.EqualCaseInsensitive:
                return CreateStringCaseInsensitiveEqualExpression(propertySelector, strFilter);
            case FilterOperator.NotEqual:
                return CreateStringNotContainsExpression(propertySelector, strFilter);
            // TODO: Implement LessThan/LessThanOrEqual/GreaterThan/GreaterThanOrEqual
            default:
                throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
        }
    }

    /// <summary>
    /// Creates a string contains expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">A property selector.</param>
    /// <param name="value">The value.</param>
    public static Expression CreateStringContainsExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, string? value)
    {
        var valueToUpper = Expression.Constant(value?.ToUpper(), typeof(TProperty));
        var propertyToUpper = propertySelector.Body.StringToUpper();
        var propertyContainsValue = propertyToUpper.StringContains(valueToUpper);
        var propertyIsNotNull = propertySelector.IsNotNull();
        var propertyIsNotNullAndContainsValue = Expression.AndAlso(propertyIsNotNull, propertyContainsValue);
        return propertyIsNotNullAndContainsValue;
    }

    /// <summary>
    /// Creates a string case-sensitive equal expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="value">The value.</param>
    public static Expression CreateStringCaseSensitiveEqualExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, string? value)
    {
        if (value == null)
            return propertySelector.IsNull();
        if (value == string.Empty)
            return propertySelector.StringIsEmpty();

        var valueExpression = Expression.Constant(value, typeof(TProperty));
        var propertyEqualsValue = Expression.Equal(propertySelector.Body, valueExpression);
        return propertyEqualsValue;
    }

    /// <summary>
    /// Creates a string case-insensitive equal expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="value">The value.</param>
    public static Expression CreateStringCaseInsensitiveEqualExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, string? value)
    {
        if (value == null)
            return propertySelector.IsNull();
        if (value == string.Empty)
            return propertySelector.StringIsEmpty();

        var valueToUpper = Expression.Constant(value.ToUpper(), typeof(TProperty));
        var propertyToUpper = propertySelector.Body.StringToUpper();
        var propertyEqualsValue = Expression.Equal(propertyToUpper, valueToUpper);
        var propertyIsNotNull = propertySelector.IsNotNull();
        var propertyIsNotNullAndEqualsValue = Expression.AndAlso(propertyIsNotNull, propertyEqualsValue);
        return propertyIsNotNullAndEqualsValue;
    }

    /// <summary>
    /// Creates a negated string contains expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TProperty">The type of the t property.</typeparam>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="value">The value.</param>
    /// <autogeneratedoc />
    public static Expression CreateStringNotContainsExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, string? value)
        => Expression.Not(CreateStringContainsExpression(propertySelector, value));
}
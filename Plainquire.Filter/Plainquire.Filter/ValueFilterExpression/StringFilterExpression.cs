using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Plainquire.Filter.ValueFilterExpression;

/// <inheritdoc cref="IStringFilterExpression"/>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Provided as library, can be used from outside")]
public class StringFilterExpression : DefaultFilterExpression, IStringFilterExpression
{
    /// <inheritdoc />
    public override ICollection<FilterOperator> SupportedFilterOperators
        =>
        [
            FilterOperator.Default,
            FilterOperator.Contains,
            FilterOperator.StartsWith,
            FilterOperator.EndsWith,
            FilterOperator.EqualCaseSensitive,
            FilterOperator.EqualCaseInsensitive,
            FilterOperator.NotEqual,
            FilterOperator.IsNull,
            FilterOperator.NotNull
        ];

    /// <inheritdoc />
    public override bool CanCreateExpressionFor(Type type)
        => type.GetUnderlyingType() == typeof(string);

    /// <inheritdoc />
    protected internal override Expression? CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration, IFilterInterceptor? interceptor)
    {
        var strFilter = value?.Trim();
        switch (filterOperator)
        {
            case FilterOperator.Default:
            case FilterOperator.Contains:
                return CreateStringContainsExpression(propertySelector, strFilter);
            case FilterOperator.StartsWith:
                return CreateStringStartsWithExpression(propertySelector, strFilter);
            case FilterOperator.EndsWith:
                return CreateStringEndsWithExpression(propertySelector, strFilter);
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
    /// Creates a string starts with expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="value">The value.</param>
    public static Expression? CreateStringStartsWithExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, string? value)
    {
        var valueToUpper = Expression.Constant(value?.ToUpper(), typeof(TProperty));
        var propertyToUpper = propertySelector.Body.StringToUpper();
        var propertyStartsWithValue = propertyToUpper.StringStartsWith(valueToUpper);
        var propertyIsNotNull = propertySelector.IsNotNull();
        var propertyIsNotNullAndStartsWithValue = Expression.AndAlso(propertyIsNotNull, propertyStartsWithValue);
        return propertyIsNotNullAndStartsWithValue;
    }

    /// <summary>
    /// Creates a string ends with expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property selector.</param>
    /// <param name="value">The value.</param>
    public static Expression? CreateStringEndsWithExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, string? value)
    {
        var valueToUpper = Expression.Constant(value?.ToUpper(), typeof(TProperty));
        var propertyToUpper = propertySelector.Body.StringToUpper();
        var propertyEndsWithValue = propertyToUpper.StringEndsWith(valueToUpper);
        var propertyIsNotNull = propertySelector.IsNotNull();
        var propertyIsNotNullAndEndsWithValue = Expression.AndAlso(propertyIsNotNull, propertyEndsWithValue);
        return propertyIsNotNullAndEndsWithValue;
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
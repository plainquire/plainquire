using FS.FilterExpressionCreator.Abstractions.Configurations;
using FS.FilterExpressionCreator.Abstractions.Extensions;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Exceptions;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace FS.FilterExpressionCreator.ValueFilterExpressionCreators;

/// <inheritdoc />
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Provided as library, can be used from outside")]
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Provided as library, can be used from outside")]
[Obsolete("Use 'Plainquire.Filter.ValueFilterExpression.DateTimeFilterExpression' instead.")]
public class DefaultFilterExpressionCreator : IValueFilterExpressionCreator
{
    /// <summary>
    /// Gets the supported filter operators.
    /// </summary>
    public virtual ICollection<FilterOperator> SupportedFilterOperators
        => new[]
        {
            FilterOperator.Default,
            FilterOperator.EqualCaseSensitive,
            FilterOperator.EqualCaseInsensitive,
            FilterOperator.NotEqual,
            FilterOperator.LessThanOrEqual,
            FilterOperator.LessThan,
            FilterOperator.GreaterThanOrEqual,
            FilterOperator.GreaterThan,
            FilterOperator.IsNull,
            FilterOperator.NotNull
        };

    /// <inheritdoc />
    public bool CanCreateExpressionFor<TType>()
        => CanCreateExpressionFor(typeof(TType));

    /// <inheritdoc />
    public virtual bool CanCreateExpressionFor(Type type)
        => true;

    /// <inheritdoc />
    public Expression? CreateExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, ValueFilter[]? filters, FilterConfiguration configuration)
    {
        if (filters == null || filters.Length == 0)
            return null;

        var propertyType = typeof(TProperty);
        var propertyCanBeNull = !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null;

        var filterExpressions = filters
            .Select(filter =>
            {
                if (!SupportedFilterOperators.Contains(filter.Operator))
                    throw CreateFilterExpressionCreationException($"Filter operator '{filter.Operator}' not allowed for property type '{propertyType}'", propertySelector, filter.Operator, filter.Value);

                if (filter.IsEmpty)
                    return null;

                switch (filter.Operator)
                {
                    case FilterOperator.IsNull:
                        if (!propertyCanBeNull)
                            throw CreateFilterExpressionCreationException($"Filter operator '{filter.Operator}' not allowed for property type '{propertyType}'", propertySelector, filter.Operator, filter.Value);
                        return Expression.Equal(propertySelector.Body, Expression.Constant(null));
                    case FilterOperator.NotNull:
                        if (!propertyCanBeNull)
                            throw CreateFilterExpressionCreationException($"Filter operator '{filter.Operator}' not allowed for property type '{propertyType}'", propertySelector, filter.Operator, filter.Value);
                        return Expression.NotEqual(propertySelector.Body, Expression.Constant(null));
                    default:
                        return CreateExpressionForValue(propertySelector, filter.Operator, filter.Value, configuration);
                }
            })
            .ToList();

        var filterExpression = filterExpressions.Aggregate(Expression.OrElse);

        return filterExpression;
    }

    /// <summary>
    /// Creates the body of a filter expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to create the expression for.</param>
    /// <param name="filterOperator">The filter operator to use.</param>
    /// <param name="value">The value to create the expression for.</param>
    /// <param name="configuration">The filter configuration to use.</param>
    protected internal virtual Expression? CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration)
    {
        switch (filterOperator)
        {
            case FilterOperator.NotEqual:
                return CreateNotEqualExpression(propertySelector, value);
            case FilterOperator.GreaterThan:
                return CreateGreaterThanExpression(propertySelector, value);
            case FilterOperator.GreaterThanOrEqual:
                return CreateGreaterThanOrEqualExpression(propertySelector, value);
            case FilterOperator.LessThan:
                return CreateLessThanExpression(propertySelector, value);
            case FilterOperator.LessThanOrEqual:
                return CreateLessThanOrEqualExpression(propertySelector, value);
            case FilterOperator.Default:
            case FilterOperator.EqualCaseInsensitive:
            case FilterOperator.EqualCaseSensitive:
                return CreateEqualExpression(propertySelector, value);
            default:
                throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
        }
    }

    /// <summary>
    /// Creates a binary 'equals' ('==') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="value">The value to use.</param>
    public static Expression CreateEqualExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue? value)
        => CreateEqualExpression(propertySelector, typeof(TValue), value);

    /// <summary>
    /// Creates a binary 'equals' ('==') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="valueType">The type of the value.</param>
    /// <param name="value">The value to use.</param>
    protected static Expression CreateEqualExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var (propertyExpression, valueExpression) = CreateAndCastExpressionParts(propertySelector, valueType, value);
        return Expression.Equal(propertyExpression, valueExpression);
    }

    /// <summary>
    /// Creates a binary 'not equals' ('!=') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="value">The value to use.</param>
    public static Expression CreateNotEqualExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue? value)
        => CreateNotEqualExpression(propertySelector, typeof(TValue), value);

    /// <summary>
    /// Creates a binary 'not equals' ('!=') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="valueType">The type of the value.</param>
    /// <param name="value">The value to use.</param>
    protected static Expression CreateNotEqualExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var (propertyExpression, valueExpression) = CreateAndCastExpressionParts(propertySelector, valueType, value);
        return Expression.NotEqual(propertyExpression, valueExpression);
    }

    /// <summary>
    /// Creates a binary 'less than' ('&lt;') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="value">The value to use.</param>
    public static Expression CreateLessThanExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue? value)
        => CreateLessThanExpression(propertySelector, typeof(TValue), value);

    /// <summary>
    /// Creates a binary 'less than' ('&lt;') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="valueType">The type of the value.</param>
    /// <param name="value">The value to use.</param>
    protected static Expression CreateLessThanExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var (propertyExpression, valueExpression) = CreateAndCastExpressionParts(propertySelector, valueType, value);
        return Expression.LessThan(propertyExpression, valueExpression);
    }

    /// <summary>
    /// Creates a binary 'less than or equal' ('&lt;=') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="value">The value to use.</param>
    public static Expression CreateLessThanOrEqualExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue? value)
        => CreateLessThanOrEqualExpression(propertySelector, typeof(TValue), value);

    /// <summary>
    /// Creates a binary 'less than or equal' ('&lt;=') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="valueType">The type of the value.</param>
    /// <param name="value">The value to use.</param>
    protected static Expression CreateLessThanOrEqualExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var (propertyExpression, valueExpression) = CreateAndCastExpressionParts(propertySelector, valueType, value);
        return Expression.LessThanOrEqual(propertyExpression, valueExpression);
    }

    /// <summary>
    /// Creates a binary 'greater than' ('&gt;') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="value">The value to use.</param>
    public static Expression CreateGreaterThanExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue? value)
        => CreateGreaterThanExpression(propertySelector, typeof(TValue), value);

    /// <summary>
    /// Creates a binary 'greater than' ('&gt;') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="valueType">The type of the value.</param>
    /// <param name="value">The value to use.</param>
    protected static Expression CreateGreaterThanExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var (propertyExpression, valueExpression) = CreateAndCastExpressionParts(propertySelector, valueType, value);
        return Expression.GreaterThan(propertyExpression, valueExpression);
    }

    /// <summary>
    /// Creates a binary 'greater than or equal' ('&gt;=') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="value">The value to use.</param>
    public static Expression CreateGreaterThanOrEqualExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue? value)
        => CreateGreaterThanOrEqualExpression(propertySelector, typeof(TValue), value);

    /// <summary>
    /// Creates a binary 'greater than or equal' ('&gt;=') expression for the given property/value pair.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">The property to use.</param>
    /// <param name="valueType">The type of the value.</param>
    /// <param name="value">The value to use.</param>
    protected static Expression CreateGreaterThanOrEqualExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var (propertyExpression, valueExpression) = CreateAndCastExpressionParts(propertySelector, valueType, value);
        return Expression.GreaterThanOrEqual(propertyExpression, valueExpression);
    }

    private static (Expression propertyExpression, Expression valueExpression) CreateAndCastExpressionParts<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, Type valueType, object? value)
    {
        var underlingValueType = valueType.GetUnderlyingType();
        var partsHaveSameUnderlyingTypes = typeof(TProperty).GetUnderlyingType() == underlingValueType;
        var propertyIsNullable = Nullable.GetUnderlyingType(typeof(TProperty)) != null;

        var destinationType = propertyIsNullable && underlingValueType.IsValueType
            ? typeof(Nullable<>).MakeGenericType(underlingValueType)
            : valueType;

        var valueExpression = partsHaveSameUnderlyingTypes
            ? Expression.Constant(value, typeof(TProperty))
            : Expression.Constant(value, destinationType);

        var propertyExpression = partsHaveSameUnderlyingTypes
            ? propertySelector.Body
            : propertySelector.Body.Cast(typeof(TProperty), destinationType);

        return (propertyExpression, valueExpression);
    }

    /// <summary>
    /// Creates a filter expression creation exception.
    /// </summary>
    /// <typeparam name="TEntity">The type of the class that declares <typeparamref name="TProperty"/>.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="message">The exception message.</param>
    /// <param name="property">The property the filter is created for.</param>
    /// <param name="filterOperator">The filter operator.</param>
    /// <param name="value">The value to filter.</param>
    protected FilterExpressionCreationException CreateFilterExpressionCreationException<TEntity, TProperty, TValue>(string message, Expression<Func<TEntity, TProperty>> property, FilterOperator filterOperator, TValue? value)
    {
        return new FilterExpressionCreationException(message)
        {
            FilteredEntity = typeof(TEntity),
            FilteredProperty = ((MemberExpression)property.Body).Member.Name,
            FilteredPropertyType = typeof(TProperty),
            FilterOperator = filterOperator,
            Value = value,
            ValueType = typeof(TValue),
            SupportedFilterOperators = SupportedFilterOperators
        };
    }
}
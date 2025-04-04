﻿using Plainquire.Filter.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Plainquire.Filter.ValueFilterExpressions;

/// <inheritdoc cref="IEnumFilterExpression"/>
public class EnumFilterExpression : DefaultFilterExpression, IEnumFilterExpression
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
            FilterOperator.LessThan,
            FilterOperator.LessThanOrEqual,
            FilterOperator.GreaterThan,
            FilterOperator.GreaterThanOrEqual,
            FilterOperator.IsNull,
            FilterOperator.NotNull
        ];

    /// <inheritdoc />
    public override bool CanCreateExpressionFor(Type type)
        => type.GetUnderlyingType().IsEnum;

    /// <inheritdoc />
    protected internal override Expression CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration, IFilterInterceptor? interceptor)
    {
        var operatorImpliesMatchByName = filterOperator is FilterOperator.Contains or FilterOperator.StartsWith or FilterOperator.EndsWith;
        if (operatorImpliesMatchByName)
            return CreateEnumFromStringExpression(propertySelector, filterOperator, value, configuration, interceptor);

        var valueIsNumeric = long.TryParse(value, NumberStyles.Any, new CultureInfo(configuration.CultureName), out var numericValue);
        if (!valueIsNumeric)
            return CreateEnumFromStringExpression(propertySelector, filterOperator, value, configuration, interceptor);

        var enumValue = (TProperty)Enum.ToObject(typeof(TProperty).GetUnderlyingType(), numericValue);
        return CreateEnumExpressionByFilterOperator(propertySelector, filterOperator, enumValue);
    }

    private Expression CreateEnumFromStringExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string? value, FilterConfiguration configuration, IFilterInterceptor? interceptor)
    {
        var enumValues = GetEnumValuesMatchByStringFilter<TProperty>(filterOperator, value, configuration, interceptor).ToList();
        if (!enumValues.Any())
            return Expression.Constant(false);

        var result = enumValues
            .Select(x => CreateEnumExpressionByFilterOperator(propertySelector, filterOperator, x))
            .Aggregate(Expression.OrElse);

        return result;
    }

    private Expression CreateEnumExpressionByFilterOperator<TEntity, TProperty, TEnum>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, TEnum value)
    {
        var underlyingEnumType = Enum.GetUnderlyingType(typeof(TProperty).GetUnderlyingType());
        var numericEnumValue = Convert.ChangeType(value, underlyingEnumType, CultureInfo.InvariantCulture);

        switch (filterOperator)
        {
            case FilterOperator.Default:
            case FilterOperator.Contains:
            case FilterOperator.StartsWith:
            case FilterOperator.EndsWith:
            case FilterOperator.EqualCaseInsensitive:
            case FilterOperator.EqualCaseSensitive:
                return CreateEqualExpression(propertySelector, value);
            case FilterOperator.NotEqual:
                return CreateNotEqualExpression(propertySelector, value);
            case FilterOperator.LessThan:
                return CreateLessThanExpression(propertySelector, underlyingEnumType, numericEnumValue);
            case FilterOperator.LessThanOrEqual:
                return CreateLessThanOrEqualExpression(propertySelector, underlyingEnumType, numericEnumValue);
            case FilterOperator.GreaterThan:
                return CreateGreaterThanExpression(propertySelector, underlyingEnumType, numericEnumValue);
            case FilterOperator.GreaterThanOrEqual:
                return CreateGreaterThanOrEqualExpression(propertySelector, underlyingEnumType, numericEnumValue);
            default:
                throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
        }
    }

    private static IEnumerable<TProperty> GetEnumValuesMatchByStringFilter<TProperty>(FilterOperator filterOperator, string? value, FilterConfiguration configuration, IFilterInterceptor? interceptor)
    {
        var stringFilterExpressionCreator = new StringFilterExpression();
        Expression<Func<KeyValuePair<TProperty, string>, string>> dictionaryValueSelector = x => x.Value;

        var stringFilterOperator = filterOperator switch
        {
            FilterOperator.Contains => FilterOperator.Contains,
            FilterOperator.StartsWith => FilterOperator.StartsWith,
            FilterOperator.EndsWith => FilterOperator.EndsWith,
            FilterOperator.EqualCaseInsensitive => FilterOperator.EqualCaseInsensitive,
            FilterOperator.EqualCaseSensitive => FilterOperator.EqualCaseSensitive,
            _ => FilterOperator.EqualCaseInsensitive
        };

        var underlyingEnumType = typeof(TProperty).GetUnderlyingType();

        var stringFilterExpression = stringFilterExpressionCreator
            .CreateExpressionForValue(dictionaryValueSelector, stringFilterOperator, value, configuration, interceptor);

        var stringFilter = Expression
            .Lambda<Func<KeyValuePair<TProperty, string>, bool>>(stringFilterExpression, dictionaryValueSelector.Parameters)
            .Compile();

        var filteredValues = Enum
            .GetValues(underlyingEnumType)
            .Cast<TProperty>()
            .Select(enumValue => KeyValuePair.Create(enumValue, EnumGetName(underlyingEnumType, enumValue)))
            .Where(stringFilter)
            .Select(x => x.Key);

        return filteredValues;
    }

    private static string EnumGetName<TEnum>(Type underlyingEnumType, TEnum value)
        => Enum.GetName(underlyingEnumType, value!)
           ?? throw new InvalidOperationException($"Unable to get value of {value} for enum {underlyingEnumType.Name}");
}
using FilterExpressionCreator.Enums;
using FilterExpressionCreator.Extensions;
using FilterExpressionCreator.Interfaces;
using FilterExpressionCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FilterExpressionCreator.ValueFilterExpressionCreators
{
    /// <inheritdoc cref="IDateTimeFilterExpressionCreator"/>
    public class DateTimeFilterExpressionCreator : DefaultFilterExpressionCreator, IDateTimeFilterExpressionCreator
    {
        /// <inheritdoc />
        public override ICollection<FilterOperator> SupportedFilterOperators
            => new[]
            {
                FilterOperator.Default,
                FilterOperator.Contains,
                FilterOperator.EqualCaseInsensitive,
                FilterOperator.EqualCaseSensitive,
                FilterOperator.NotEqual,
                FilterOperator.LessThan,
                FilterOperator.LessThanOrEqual,
                FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqual,
                FilterOperator.IsNull,
                FilterOperator.NotNull,
            };

        /// <inheritdoc />
        public override bool CanCreateExpressionFor(Type type)
            => type.GetUnderlyingType() == typeof(DateTime);

        /// <inheritdoc />
        protected internal override Expression CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string value, FilterConfiguration filterConfiguration)
        {
            if (value.TryConvertStringToDateTimeSpan(filterConfiguration.Now(), out var dateTimeSpan, filterConfiguration.CultureInfo))
                return CreateDateTimeExpressionByFilterOperator(propertySelector, filterOperator, dateTimeSpan);

            if (filterConfiguration.IgnoreParseExceptions)
                return null;

            throw CreateFilterExpressionCreationException("Unable to parse given filter value", propertySelector, filterOperator, value);
        }

        private Expression CreateDateTimeExpressionByFilterOperator<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, DateTimeSpan value)
        {
            switch (filterOperator)
            {
                case FilterOperator.Default:
                case FilterOperator.Contains:
                    return CreateDateTimeSpanContainsExpression(propertySelector, value);
                case FilterOperator.EqualCaseInsensitive:
                case FilterOperator.EqualCaseSensitive:
                    return CreateEqualExpression(propertySelector, value.Start);
                case FilterOperator.NotEqual:
                    return CreateNotEqualExpression(propertySelector, value.Start);
                case FilterOperator.LessThan:
                    return CreateLessThanExpression(propertySelector, value.Start);
                case FilterOperator.LessThanOrEqual:
                    return CreateLessThanOrEqualExpression(propertySelector, value.Start);
                case FilterOperator.GreaterThan:
                    return CreateGreaterThanExpression(propertySelector, value.Start);
                case FilterOperator.GreaterThanOrEqual:
                    return CreateGreaterThanOrEqualExpression(propertySelector, value.Start);
                default:
                    throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
            }
        }

        private static Expression CreateDateTimeSpanContainsExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, DateTimeSpan value)
        {
            var startExpression = Expression.Constant(value.Start, typeof(TProperty));
            var endExpression = Expression.Constant(value.End, typeof(TProperty));
            var startGreaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertySelector.Body, startExpression);
            var endLessThanExpression = Expression.LessThan(propertySelector.Body, endExpression);
            var result = Expression.AndAlso(startGreaterThanOrEqualExpression, endLessThanExpression);
            return result;
        }
    }
}

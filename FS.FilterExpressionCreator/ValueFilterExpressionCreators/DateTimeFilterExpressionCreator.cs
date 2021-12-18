using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FS.FilterExpressionCreator.ValueFilterExpressionCreators
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
            => new[] { typeof(Section<DateTimeOffset>), typeof(DateTime), typeof(DateTimeOffset) }.Contains(type.GetUnderlyingType());

        /// <inheritdoc />
        protected internal override Expression CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string value, FilterConfiguration configuration)
        {
            if (value.TryConvertStringToDateTimeSection(configuration.Now(), out var dateTimeSpan, configuration.CultureInfo))
                return CreateDateTimeExpressionByFilterOperator(propertySelector, filterOperator, dateTimeSpan);

            if (configuration.IgnoreParseExceptions)
                return null;

            throw CreateFilterExpressionCreationException("Unable to parse given filter value", propertySelector, filterOperator, value);
        }

        private Expression CreateDateTimeExpressionByFilterOperator<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, Section<DateTimeOffset> value)
        {
            TProperty valueStart;
            TProperty valueEnd;

            var underlyingFilterPropertyType = typeof(TProperty).GetUnderlyingType();
            if (underlyingFilterPropertyType == typeof(DateTime))
            {
                valueStart = (TProperty)(object)value.Start.DateTime;
                valueEnd = (TProperty)(object)value.End.DateTime;
            }
            else
            {
                valueStart = (TProperty)(object)value.Start;
                valueEnd = (TProperty)(object)value.End;
            }

            switch (filterOperator)
            {
                case FilterOperator.Default:
                case FilterOperator.Contains:
                    return CreateDateTimeSpanContainsExpression(propertySelector, valueStart, valueEnd);
                case FilterOperator.EqualCaseInsensitive:
                case FilterOperator.EqualCaseSensitive:
                    return CreateEqualExpression(propertySelector, valueStart);
                case FilterOperator.NotEqual:
                    return CreateNotEqualExpression(propertySelector, valueStart);
                case FilterOperator.LessThan:
                    return CreateLessThanExpression(propertySelector, valueStart);
                case FilterOperator.LessThanOrEqual:
                    return CreateLessThanOrEqualExpression(propertySelector, valueStart);
                case FilterOperator.GreaterThan:
                    return CreateGreaterThanExpression(propertySelector, valueStart);
                case FilterOperator.GreaterThanOrEqual:
                    return CreateGreaterThanOrEqualExpression(propertySelector, valueStart);
                default:
                    throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
            }
        }

        /// <summary>
        /// Creates a date time span contains expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertySelector">The property selector.</param>
        /// <param name="start">The start <see cref="DateTime"/> or <see cref="DateTimeOffset"/>.</param>
        /// <param name="end">The end <see cref="DateTime"/> or <see cref="DateTimeOffset"/>.</param>
        public static Expression CreateDateTimeSpanContainsExpression<TEntity, TProperty, TValue>(Expression<Func<TEntity, TProperty>> propertySelector, TValue start, TValue end)
        {
            var startExpression = Expression.Constant(start, typeof(TProperty));
            var endExpression = Expression.Constant(end, typeof(TProperty));
            var startGreaterThanOrEqualExpression = Expression.GreaterThanOrEqual(propertySelector.Body, startExpression);
            var endLessThanExpression = Expression.LessThan(propertySelector.Body, endExpression);
            var result = Expression.AndAlso(startGreaterThanOrEqualExpression, endLessThanExpression);
            return result;
        }
    }
}

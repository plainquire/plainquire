using FS.FilterExpressionCreator.Abstractions.Extensions;
using FS.FilterExpressionCreator.Abstractions.Models;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace FS.FilterExpressionCreator.ValueFilterExpressionCreators
{
    /// <inheritdoc cref="INumericFilterExpressionCreator"/>
    public class NumericFilterExpressionCreator : DefaultFilterExpressionCreator, INumericFilterExpressionCreator
    {
        private static readonly List<Type> _primitiveNumberTypes = new List<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) };

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
            => _primitiveNumberTypes.Contains(type.GetUnderlyingType());

        /// <inheritdoc />
        protected internal override Expression CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string value, FilterConfiguration configuration)
        {
            if (decimal.TryParse(value, NumberStyles.Any, configuration.CultureInfo, out var decimalValue))
                return CreateNumberExpressionByFilterOperator(propertySelector, filterOperator, decimalValue);

            if (configuration.IgnoreParseExceptions)
                return null;

            throw CreateFilterExpressionCreationException("Unable to parse given filter value", propertySelector, filterOperator, value);
        }

        private Expression CreateNumberExpressionByFilterOperator<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, object value)
        {
            var underlyingFilterPropertyType = typeof(TProperty).GetUnderlyingType();
            var typedValue = (TProperty)Convert.ChangeType(value, underlyingFilterPropertyType);

            switch (filterOperator)
            {
                case FilterOperator.Default:
                case FilterOperator.EqualCaseInsensitive:
                case FilterOperator.EqualCaseSensitive:
                    return CreateEqualExpression(propertySelector, typedValue);
                case FilterOperator.NotEqual:
                    return CreateNotEqualExpression(propertySelector, typedValue);
                case FilterOperator.Contains:
                    return CreateNumericContainsExpression(propertySelector, typedValue);
                case FilterOperator.LessThan:
                    return CreateLessThanExpression(propertySelector, typedValue);
                case FilterOperator.LessThanOrEqual:
                    return CreateLessThanOrEqualExpression(propertySelector, typedValue);
                case FilterOperator.GreaterThan:
                    return CreateGreaterThanExpression(propertySelector, typedValue);
                case FilterOperator.GreaterThanOrEqual:
                    return CreateGreaterThanOrEqualExpression(propertySelector, typedValue);
                default:
                    throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, typedValue);
            }
        }

        private static Expression CreateNumericContainsExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, TProperty typedValue)
        {
            var valueToUpper = Expression.Constant(typedValue.ToString().ToUpper(), typeof(string));
            var propertyToString = propertySelector.Body.ObjectToString();
            var propertyToUpper = propertyToString.StringToUpper();
            var propertyContainsValue = propertyToUpper.StringContains(valueToUpper);
            return propertyContainsValue;
        }
    }
}

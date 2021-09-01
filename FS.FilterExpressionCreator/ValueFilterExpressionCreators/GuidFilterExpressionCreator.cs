using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.FilterExpressionCreator.ValueFilterExpressionCreators
{
    /// <inheritdoc cref="IGuidFilterExpressionCreator"/>
    public class GuidFilterExpressionCreator : DefaultFilterExpressionCreator, IGuidFilterExpressionCreator
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
                FilterOperator.NotNull,
            };

        /// <inheritdoc />
        public override bool CanCreateExpressionFor(Type type)
            => type.GetUnderlyingType() == typeof(Guid);

        /// <inheritdoc />
        protected internal override Expression CreateExpressionForValue<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, FilterOperator filterOperator, string value, FilterConfiguration configuration)
        {
            if (Guid.TryParse(value, out var guidValue))
                return CreateGuidExpressionByFilterOperator(propertySelector, filterOperator, guidValue);

            // TODO: Check how partial GUID handling could be implemented.
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
                // TODO: Implement LessThan/LessThanOrEqual/GreaterThan/GreaterThanOrEqual
                default:
                    throw CreateFilterExpressionCreationException($"Filter operator '{filterOperator}' not allowed for property type '{typeof(TProperty)}'", propertySelector, filterOperator, value);
            }
        }
    }
}

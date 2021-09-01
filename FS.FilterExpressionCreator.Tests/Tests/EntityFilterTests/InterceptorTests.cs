using FluentAssertions;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.Models;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Models;
using FS.FilterExpressionCreator.ValueFilterExpressionCreators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilterTests
{
    [TestClass]
    public class InterceptorTests : TestBase
    {
        [DataTestMethod]
        [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
        public void WhenFilterInterceptorIsUsed_ValuesAreFilteredAsExpected(EntityFilterFunc<TestModel<string>> filterFunc)
        {
            var filter = new EntityFilter<TestModel<string>>()
                .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, "TestA")
                .Replace(x => x.ValueB, FilterOperator.EqualCaseSensitive, "TestB");

            var testItems = new List<TestModel<string>>
            {
                new() { ValueA = "TestA", ValueB = "TestA" },
                new() { ValueA = "TestA", ValueB = "TestB" },
                new() { ValueA = "TESTA", ValueB = "TestB" },
                new() { ValueA = "TestB", ValueB = "TestB" },
            };

            var interceptor = new FilterStringsCaseInsensitiveInterceptor();
            var filteredEntities = filterFunc(testItems, filter, null, interceptor);

            filteredEntities.Should().BeEquivalentTo(new[] { testItems[1], testItems[2] });
        }

        public class FilterStringsCaseInsensitiveInterceptor : IPropertyFilterInterceptor
        {
            public Expression<Func<TEntity, bool>> CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, ValueFilter filter, FilterConfiguration configuration)
            {
                var stringPropertyIsFiltered = propertyInfo.PropertyType == typeof(string);
                var operatorIsEqualCaseSensitive = filter.Operator == Enums.FilterOperator.EqualCaseSensitive;
                if (!stringPropertyIsFiltered || !operatorIsEqualCaseSensitive)
                    return null;

                var propertySelector = typeof(TEntity).CreatePropertySelector<TEntity, string>(propertyInfo.Name);
                var filterExpression = filter
                    .Values
                    .Select(value => StringFilterExpressionCreator
                        .CreateStringCaseInsensitiveEqualExpression(propertySelector, value)
                    )
                    .Aggregate(Expression.OrElse);

                return propertySelector.CreateLambda<TEntity, string, bool>(filterExpression);
            }
        }
    }
}

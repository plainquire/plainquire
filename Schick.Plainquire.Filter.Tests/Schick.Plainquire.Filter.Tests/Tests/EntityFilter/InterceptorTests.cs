using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Filter.Abstractions.Configurations;
using Schick.Plainquire.Filter.Abstractions.Extensions;
using Schick.Plainquire.Filter.Enums;
using Schick.Plainquire.Filter.Extensions;
using Schick.Plainquire.Filter.Filters;
using Schick.Plainquire.Filter.Interfaces;
using Schick.Plainquire.Filter.Tests.Models;
using Schick.Plainquire.Filter.Tests.Services;
using Schick.Plainquire.Filter.ValueFilterExpression;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Schick.Plainquire.Filter.Tests.Tests.EntityFilter;

[TestClass, ExcludeFromCodeCoverage]
public class InterceptorTests
{
    [DataTestMethod]
    [FilterFuncDataSource<TestModel<string>>]
    public void WhenFilterInterceptorIsUsed_ValuesAreFilteredAsExpected(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var filter = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, "TestA")
            .Replace(x => x.ValueB, FilterOperator.Contains, "TestB");

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "TestA", ValueB = "TestA" },
            new() { ValueA = "TestA", ValueB = "TestB" },
            // ReSharper disable once StringLiteralTypo
            new() { ValueA = "TESTA", ValueB = "TestB" },
            new() { ValueA = "TestB", ValueB = "TestB" }
        };

        var interceptor = new FilterStringsCaseInsensitiveInterceptor();
        var filteredEntities = filterFunc(testItems, filter, null, interceptor);

        filteredEntities.Should().BeEquivalentTo(new[] { testItems[1], testItems[2] });
    }

    [DataTestMethod, DoNotParallelize]
    [FilterFuncDataSource<TestModel<string>>]
    public void WhenDefaultFilterInterceptorIsUsed_ValuesAreFilteredAsExpected(EntityFilterFunc<TestModel<string>> filterFunc)
    {
        var filter = new EntityFilter<TestModel<string>>()
            .Replace(x => x.ValueA, FilterOperator.EqualCaseSensitive, "TestA")
            .Replace(x => x.ValueB, FilterOperator.Contains, "TestB");

        var testItems = new List<TestModel<string>>
        {
            new() { ValueA = "TestA", ValueB = "TestA" },
            new() { ValueA = "TestA", ValueB = "TestB" },
            // ReSharper disable once StringLiteralTypo
            new() { ValueA = "TESTA", ValueB = "TestB" },
            new() { ValueA = "TestB", ValueB = "TestB" }
        };

        var interceptor = new FilterStringsCaseInsensitiveInterceptor();
        Filters.EntityFilter.DefaultInterceptor = interceptor;
        var filteredEntities = filterFunc(testItems, filter);

        filteredEntities.Should().BeEquivalentTo(new[] { testItems[1], testItems[2] });

        // Cleanup
        Filters.EntityFilter.DefaultInterceptor = null;
    }

    private class FilterStringsCaseInsensitiveInterceptor : IFilterInterceptor
    {
        public Expression<Func<TEntity, bool>>? CreatePropertyFilter<TEntity>(PropertyInfo propertyInfo, Filters.ValueFilter[] filters, FilterConfiguration configuration)
        {
            var filteredPropertyIsTypeOfString = propertyInfo.PropertyType == typeof(string);
            if (!filteredPropertyIsTypeOfString)
                return null;

            var filterToModify = filters
                .Where(x => x.Operator == FilterOperator.EqualCaseSensitive)
                .ToArray();

            if (!filterToModify.Any())
                return null;

            var propertySelector = typeof(TEntity).CreatePropertySelector<TEntity, string>(propertyInfo.Name);

            var modifiedFilterExpr = filterToModify
                .Select(filter => StringFilterExpression.CreateStringCaseInsensitiveEqualExpression(propertySelector, filter.Value))
                .Aggregate(Expression.OrElse);

            var modifiedFilterLambda = propertySelector
                .CreateLambda<TEntity, string, bool>(modifiedFilterExpr);

            var unmodifiedFilters = filters
                .Where(x => x.Operator != FilterOperator.EqualCaseSensitive)
                .ToArray();

            var unmodifiedFilterLambda = PropertyFilterExpression.PropertyFilterExpression
                .CreateFilter<TEntity>(propertyInfo.PropertyType, propertySelector, unmodifiedFilters, configuration);

            var filterExpression = new[] { modifiedFilterLambda, unmodifiedFilterLambda }.CombineWithConditionalOr();

            return filterExpression;
        }
    }
}
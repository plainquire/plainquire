using FluentAssertions;
using FS.FilterExpressionCreator.Abstractions.Configurations;
using FS.FilterExpressionCreator.Abstractions.Extensions;
using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.PropertyFilterExpressionCreators;
using FS.FilterExpressionCreator.Tests.Attributes;
using FS.FilterExpressionCreator.Tests.Models;
using FS.FilterExpressionCreator.ValueFilterExpressionCreators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.FilterExpressionCreator.Tests.Tests.EntityFilter;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[TestClass, ExcludeFromCodeCoverage]
public class InterceptorTests : TestBase
{
    [DataTestMethod]
    [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
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
    [FilterFuncDataSource(nameof(GetEntityFilterFunctions), typeof(TestModel<string>))]
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

    private class FilterStringsCaseInsensitiveInterceptor : IPropertyFilterInterceptor
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
                .Select(filter => StringFilterExpressionCreator.CreateStringCaseInsensitiveEqualExpression(propertySelector, filter.Value))
                .Aggregate(Expression.OrElse);

            var modifiedFilterLambda = propertySelector
                .CreateLambda<TEntity, string, bool>(modifiedFilterExpr);

            var unmodifiedFilters = filters
                .Where(x => x.Operator != FilterOperator.EqualCaseSensitive)
                .ToArray();

            var unmodifiedFilterLambda = PropertyFilterExpressionCreator
                .CreateFilter<TEntity>(propertyInfo.PropertyType, propertySelector, unmodifiedFilters, configuration);

            var filterExpression = new[] { modifiedFilterLambda, unmodifiedFilterLambda }.CombineWithConditionalOr();

            return filterExpression;
        }
    }
}
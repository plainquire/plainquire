using FluentAssertions;
using Schick.Plainquire.Sort.Enums;
using Schick.Plainquire.Sort.Sorts;
using Schick.Plainquire.Sort.Tests.Models;
using Schick.Plainquire.Sort.Tests.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Schick.Plainquire.Sort.Tests.Tests.PropertySort;

[TestClass, ExcludeFromCodeCoverage]
public class PropertySortExpressionTests
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private readonly TestModel<string>[] _testItems =
    [
        new TestModel<string> { Value = "Sorting", NestedObject = new TestModelNested<string> { Value = "Sorting" } },
        new TestModel<string> { Value = "Třídění", NestedObject = new TestModelNested<string> { Value = "Třídění" } },
        new TestModel<string> { Value = "Razvrščanje", NestedObject = new TestModelNested<string> { Value = "Razvrščanje" } },
        new TestModel<string> { Value = "Sortierung", NestedObject = new TestModelNested<string> { Value = "Sortierung" } },
        new TestModel<string> { Value = "100", NestedObject = new TestModelNested<string> { Value = "100" } },
        new TestModel<string> { Value = "10", NestedObject = new TestModelNested<string> { Value = "10" } },
        new TestModel<string> { Value = "20", NestedObject = new TestModelNested<string> { Value = "20" } },
        new TestModel<string> { Value = "ソート", NestedObject = new TestModelNested<string> { Value = "ソート" } },
        new TestModel<string> { Value = "정렬", NestedObject = new TestModelNested<string> { Value = "정렬" } },
        new TestModel<string> { Value = "分类", NestedObject = new TestModelNested<string> { Value = "分类" } },
        new TestModel<string> { Value = "Über", NestedObject = new TestModelNested<string> { Value = "Über" } },
        new TestModel<string> { Value = "Unter", NestedObject = new TestModelNested<string> { Value = "Unter" } },
    ];

    private static readonly SortTestcase<TestModel<string>>[] _ownPropertyTestCases = CreateOwnPropertyTestcases<string>();
    private static readonly SortTestcase<TestModel<string>>[] _navigationPropertyTestCases = CreateNavigationPropertyTestcases<string>();

    [DataTestMethod]
    [SortTestCaseDataSource(nameof(_ownPropertyTestCases))]
    public void WhenPropertySortIsCreatedByOwnPropertyExpression_EntitiesSortedAsExpected(SortTestcase<TestModel<string>> testCase, EntitySortFunction<TestModel<string>> sortFunc)
    {
        var expectedTestItems = testCase.ExpectedSortFunc(_testItems.AsQueryable()).ToList();

        var entitySort = new EntitySort<TestModel<string>>();
        entitySort.Add(testCase.PropertySelector, testCase.SortDirection);
        var sortedTestItems = sortFunc(_testItems, entitySort);

        sortedTestItems.Should().Equal(expectedTestItems);
    }

    [DataTestMethod]
    [SortTestCaseDataSource(nameof(_navigationPropertyTestCases))]
    public void WhenPropertySortIsCreatedByNavigationPropertyExpression_EntitiesSortedAsExpected(SortTestcase<TestModel<string>> testCase, EntitySortFunction<TestModel<string>> sortFunc)
    {
        var expectedTestItems = testCase.ExpectedSortFunc(_testItems.AsQueryable()).ToList();

        var entitySort = new EntitySort<TestModel<string>>();
        entitySort.Add(testCase.PropertySelector, testCase.SortDirection);
        var sortedTestItems = sortFunc(_testItems, entitySort);

        sortedTestItems.Should().Equal(expectedTestItems);
    }

    private static SortTestcase<TestModel<TValue>>[] CreateOwnPropertyTestcases<TValue>()
        => CreateTestcases<TValue>(model => model.Value);

    private static SortTestcase<TestModel<TValue>>[] CreateNavigationPropertyTestcases<TValue>()
        => CreateTestcases<TValue>(model => model.NestedObject!.Value);

    private static SortTestcase<TestModel<TValue>>[] CreateTestcases<TValue>(Expression<Func<TestModel<TValue>, object?>> propertySelector)
    {
        var ascendingPrefixTestcase = SortTestcase<TestModel<TValue>>
            .Create(
                propertySelector: propertySelector,
                sortDirection: SortDirection.Ascending,
                expectedSortFunc: query => query.OrderBy(propertySelector)
            );

        var descendingTestcase = SortTestcase<TestModel<TValue>>
            .Create(
                propertySelector: propertySelector,
                sortDirection: SortDirection.Descending,
                expectedSortFunc: query => query.OrderByDescending(propertySelector)
            );

        return [ascendingPrefixTestcase, descendingTestcase];
    }
}
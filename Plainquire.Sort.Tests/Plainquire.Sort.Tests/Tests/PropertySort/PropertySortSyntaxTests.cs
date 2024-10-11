using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Tests.Models;
using Plainquire.Sort.Tests.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.PropertySort;

[TestClass]
public class PropertySortSyntaxTests
{
    private static readonly SortConfiguration _defaultConfiguration = new();

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private readonly TestModel<string>[] _testItems =
    [
        new() { Value = "Sorting", NestedObject = new TestModelNested<string> { Value = "Sorting" } },
        new() { Value = "Třídění", NestedObject = new TestModelNested<string> { Value = "Třídění" } },
        new() { Value = "Razvrščanje", NestedObject = new TestModelNested<string> { Value = "Razvrščanje" } },
        new() { Value = "Sortierung", NestedObject = new TestModelNested<string> { Value = "Sortierung" } },
        new() { Value = "100", NestedObject = new TestModelNested<string> { Value = "100" } },
        new() { Value = "10", NestedObject = new TestModelNested<string> { Value = "10" } },
        new() { Value = "20", NestedObject = new TestModelNested<string> { Value = "20" } },
        new() { Value = "ソート", NestedObject = new TestModelNested<string> { Value = "ソート" } },
        new() { Value = "정렬", NestedObject = new TestModelNested<string> { Value = "정렬" } },
        new() { Value = "分类", NestedObject = new TestModelNested<string> { Value = "分类" } },
        new() { Value = "Über", NestedObject = new TestModelNested<string> { Value = "Über" } },
        new() { Value = "Unter", NestedObject = new TestModelNested<string> { Value = "Unter" } },
    ];

    private static readonly SortTestcase<TestModel<string>>[] _ownPropertyTestCases = CreateOwnPropertyTestcases<string>();
    private static readonly SortTestcase<TestModel<string>>[] _navigationPropertyTestCases = CreateNavigationPropertyTestcases<string>();

    [DataTestMethod]
    [SortTestCaseDataSource(nameof(_ownPropertyTestCases))]
    public void WhenPropertySortIsCreatedByOwnPropertySyntax_EntitiesSortedAsExpected(SortTestcase<TestModel<string>> testCase, EntitySortFunction<TestModel<string>> sortFunc)
    {
        var expectedTestItems = testCase.ExpectedSortFunc(_testItems.AsQueryable()).ToList();

        var entitySort = new EntitySort<TestModel<string>>()
            .Add(testCase.Syntax);

        var sortedTestItems = sortFunc(_testItems, entitySort);

        sortedTestItems.Should().Equal(expectedTestItems);
    }

    [DataTestMethod]
    [SortTestCaseDataSource(nameof(_navigationPropertyTestCases))]
    public void WhenPropertySortIsCreatedByNavigationPropertySyntax_EntitiesSortedAsExpected(SortTestcase<TestModel<string>> testCase, EntitySortFunction<TestModel<string>> sortFunc)
    {
        var expectedTestItems = testCase.ExpectedSortFunc(_testItems.AsQueryable()).ToList();

        var entitySort = new EntitySort<TestModel<string>>()
            .Add(testCase.Syntax);

        var sortedTestItems = sortFunc(_testItems, entitySort);

        sortedTestItems.Should().Equal(expectedTestItems);
    }

    private static SortTestcase<TestModel<TValue>>[] CreateOwnPropertyTestcases<TValue>()
    {
        var ascendingPrefixTestcases = _defaultConfiguration.AscendingPrefixes
            .Select(selector: ascPrefix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"{ascPrefix}Value",
                expectedSortFunc: query => query.OrderBy(keySelector: model => model.Value)
            ));

        var descendingPrefixTestcases = _defaultConfiguration.DescendingPrefixes
            .Select(selector: descPrefix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"{descPrefix}Value",
                expectedSortFunc: query => query.OrderByDescending(keySelector: model => model.Value)
            ));

        var ascendingPostfixTestcases = _defaultConfiguration.AscendingPostfixes
            .Select(selector: ascPostfix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"Value{ascPostfix}",
                expectedSortFunc: query => query.OrderBy(keySelector: model => model.Value)
            ));

        var descendingPostfixTestcases = _defaultConfiguration.DescendingPostfixes
            .Select(selector: descPostfix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"Value{descPostfix}",
                expectedSortFunc: query => query.OrderByDescending(keySelector: model => model.Value)
            ));

        return ascendingPrefixTestcases
            .Concat(descendingPrefixTestcases)
            .Concat(ascendingPostfixTestcases)
            .Concat(descendingPostfixTestcases)
            .ToArray();
    }

    private static SortTestcase<TestModel<TValue>>[] CreateNavigationPropertyTestcases<TValue>()
    {
        var ascendingPrefixTestcases = _defaultConfiguration.AscendingPrefixes
            .Select(selector: ascPrefix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"{ascPrefix}NestedObject.Value",
                expectedSortFunc: query => query.OrderBy(keySelector: model => model.NestedObject!.Value)
            ));

        var descendingPrefixTestcases = _defaultConfiguration.DescendingPrefixes
            .Select(selector: descPrefix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"{descPrefix}NestedObject.Value",
                expectedSortFunc: query => query.OrderByDescending(keySelector: model => model.NestedObject!.Value)
            ));

        var ascendingPostfixTestcases = _defaultConfiguration.AscendingPostfixes
            .Select(selector: ascPostfix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"NestedObject.Value{ascPostfix}",
                expectedSortFunc: query => query.OrderBy(keySelector: model => model.NestedObject!.Value)
            ));

        var descendingPostfixTestcases = _defaultConfiguration.DescendingPostfixes
            .Select(selector: descPostfix => SortTestcase<TestModel<TValue>>.Create(
                syntax: $"NestedObject.Value{descPostfix}",
                expectedSortFunc: query => query.OrderByDescending(keySelector: model => model.NestedObject!.Value)
            ));

        return ascendingPrefixTestcases
            .Concat(descendingPrefixTestcases)
            .Concat(ascendingPostfixTestcases)
            .Concat(descendingPostfixTestcases)
            .ToArray();
    }
}
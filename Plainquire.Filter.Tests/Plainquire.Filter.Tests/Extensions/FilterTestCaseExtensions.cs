using FluentAssertions;
using FluentAssertions.Execution;
using Plainquire.Filter.Tests.Models;
using Plainquire.Filter.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainquire.Filter.Tests.Extensions;

public static class FilterTestCaseExtensions
{
    public static void Run<TFilterValue, TModelValue>(this FilterTestCase<TFilterValue, TModelValue> testCase, ICollection<TestModel<TModelValue>> testItems, EntityFilterFunc<TestModel<TModelValue>> filterFunc)
    {
        if (testCase.ExpectedException == null)
            runAndCheckExpectedItems();
        else
            RunAndCheckExpectedException(runAndCheckExpectedItems, testCase.ExpectedException);
        return;

        void runAndCheckExpectedItems() => RunAndCheckExpectedItems(testCase, testItems, filterFunc);
    }

    private static void RunAndCheckExpectedItems<TFilterValue, TModelValue>(FilterTestCase<TFilterValue, TModelValue> testCase, ICollection<TestModel<TModelValue>> testItems, EntityFilterFunc<TestModel<TModelValue>> filterFunc)
    {
        var entityFilter = CreateEntityFilter(testCase);
        var filteredItems = filterFunc(testItems, entityFilter, testCase.Interceptor);
        var expectedItems = testItems.Select(x => x.ValueA).Where(testCase.ExpectedTestItemsExpression ?? (_ => true)).ToList();

        using (new AssertionScope($"items filtered by '{entityFilter.CreateFilter()}'"))
            filteredItems.Select(x => x.ValueA).Should().Equal(expectedItems);
    }

    private static void RunAndCheckExpectedException<TExpectedException>(Action testRunner, TExpectedException expectedException)
        where TExpectedException : Exception
        => testRunner.Should().Throw<TExpectedException>().WithMessage(expectedException.Message);

    private static EntityFilter<TestModel<TModelValue>> CreateEntityFilter<TFilterValue, TModelValue>(FilterTestCase<TFilterValue, TModelValue> testCase)
    {
        var filter = new EntityFilter<TestModel<TModelValue>>(testCase.FilterConfiguration!);
        if (testCase.FilterSyntax != null)
            filter.Replace(x => x.ValueA, testCase.FilterSyntax);
        else
            filter.Replace(x => x.ValueA, testCase.FilterOperator, testCase.FilterValues);

        return filter;
    }
}
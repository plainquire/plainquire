using FluentAssertions;
using FluentAssertions.Execution;
using FS.FilterExpressionCreator.Extensions;
using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.FilterExpressionCreator.Tests.Extensions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [ExcludeFromCodeCoverage]
    public static class FilterTestCaseExtensions
    {
        public static void Run<TFilterValue, TModelValue>(this FilterTestCase<TFilterValue, TModelValue> testCase, ICollection<TestModel<TModelValue>> testItems, TestModelFilterFunc<TModelValue> filterFunc)
        {
            void testRunner() => RunAndCheckExpectedItems(testCase, testItems, filterFunc);
            RunInternal(testRunner, testCase.ExpectedException);
        }

        private static void RunInternal(Action testRunner, Exception expectedException)
        {
            if (expectedException == null)
                testRunner();
            else
                // Calls RunAndCheckExpectedException<**typeof(expectedException)**>(testRunner, expectedException)
                typeof(FilterTestCaseExtensions)
                    .GetMethod(nameof(RunAndCheckExpectedException), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
                    .MakeGenericMethod(expectedException.GetType())
                    .Invoke(null, new object[] { testRunner, expectedException });
        }

        private static void RunAndCheckExpectedItems<TFilterValue, TModelValue>(FilterTestCase<TFilterValue, TModelValue> testCase, ICollection<TestModel<TModelValue>> testItems, TestModelFilterFunc<TModelValue> filterFunc)
        {
            var entityFilter = CreateEntityFilter(testCase);
            var filteredItems = filterFunc(testItems, entityFilter, testCase.Configuration);
            var expectedItems = testItems.Select(x => x.ValueA).Where(testCase.ExpectedTestItemsExpression ?? (_ => true)).ToList();

            using (new AssertionScope($"items filtered by '{entityFilter.CreateFilter(testCase.Configuration)}'"))
                filteredItems.Select(x => x.ValueA).Should().Equal(expectedItems);
        }

        private static void RunAndCheckExpectedException<TExpectedException>(Action testRunner, TExpectedException expectedException)
            where TExpectedException : Exception
            => testRunner.Should().Throw<TExpectedException>().WithMessage(expectedException.Message);

        private static EntityFilter<TestModel<TModelValue>> CreateEntityFilter<TFilterValue, TModelValue>(FilterTestCase<TFilterValue, TModelValue> testCase)
        {
            var filter = new EntityFilter<TestModel<TModelValue>>();
            if (testCase.FilterSyntax != null)
                filter.Replace(x => x.ValueA, testCase.FilterSyntax);
            else
                filter.Replace(x => x.ValueA, testCase.FilterOperator, testCase.FilterValues);

            return filter;
        }
    }
}

using FluentAssertions;
using NUnit.Framework;
using Plainquire.Sort.Tests.Models;
using Plainquire.Sort.Tests.Services;
using Plainquire.TestSupport.Services;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.EntitySort;

[TestFixture]
public class InterceptorTests : TestContainer
{
    [SortFuncDataSource<TestModel<string>>]
    public void WhenFirstSortUsingInterceptorViaParameter_InterceptorIsCalledToCreateSort(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "222" } },
            new() { Value = "even", NestedObject = new() { Value = "1111" } },
            new() { Value = "odd", NestedObject = new() { Value = "4" } },
            new() { Value = "even", NestedObject = new() { Value = "33" } }
        ];

        var entitySort = new EntitySort<TestModel<string>>()
            .Add(x => x.NestedObject, SortDirection.Ascending);

        var interceptor = new NestedModelByValueInterceptor();

        var sortedItems = sortFunc(testItems, entitySort, interceptor);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[0], testItems[3], testItems[2]);
    }

    [SortFuncDataSource<TestModel<string>>]
    public void WhenSecondarySortUsingInterceptorViaParameter_InterceptorIsCalledToCreateSort(EntitySortFunction<TestModel<string>> sortFunc)
    {
        TestModel<string>[] testItems =
        [
            new() { Value = "odd", NestedObject = new() { Value = "222" } },
            new() { Value = "even", NestedObject = new() { Value = "1111" } },
            new() { Value = "odd", NestedObject = new() { Value = "4" } },
            new() { Value = "even", NestedObject = new() { Value = "33" } }
        ];

        var entitySort = new EntitySort<TestModel<string>>()
            .Add(x => x.Value)
            .Add(x => x.NestedObject, SortDirection.Ascending);

        var interceptor = new NestedModelByValueInterceptor();

        var sortedItems = sortFunc(testItems, entitySort, interceptor);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[3], testItems[0], testItems[2]);
    }

    private class NestedModelByValueInterceptor : ISortInterceptor
    {
        public IOrderedQueryable<TEntity>? OrderBy<TEntity>(IQueryable<TEntity> source, Sort.PropertySort sort)
        {
            if (source is IQueryable<TestModel<string>> testModelSource && sort.PropertyPath == "NestedObject")
                return (IOrderedQueryable<TEntity>)testModelSource.OrderBy(x => x.NestedObject!.Value);

            return null;
        }

        public IOrderedQueryable<TEntity>? ThenBy<TEntity>(IOrderedQueryable<TEntity> source, Sort.PropertySort sort)
        {
            if (source is IOrderedQueryable<TestModel<string>> testModelSource && sort.PropertyPath == "NestedObject")
                return (IOrderedQueryable<TEntity>)testModelSource.ThenBy(x => x.NestedObject!.Value);

            return null;
        }
    }
}
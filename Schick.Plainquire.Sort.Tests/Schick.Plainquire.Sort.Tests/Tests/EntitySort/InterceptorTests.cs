using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Sort.Abstractions.Configurations;
using Schick.Plainquire.Sort.Enums;
using Schick.Plainquire.Sort.Interfaces;
using Schick.Plainquire.Sort.Sorts;
using Schick.Plainquire.Sort.Tests.Models;
using Schick.Plainquire.Sort.Tests.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Schick.Plainquire.Sort.Tests.Tests.EntitySort;

[TestClass, ExcludeFromCodeCoverage]
public class InterceptorTests
{
    [DataTestMethod]
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

        var sortedItems = sortFunc(testItems, entitySort, null, interceptor);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[0], testItems[3], testItems[2]);
    }

    [DataTestMethod]
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

        var sortedItems = sortFunc(testItems, entitySort, null, interceptor);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[3], testItems[0], testItems[2]);
    }

    [DataTestMethod]
    [SortFuncDataSource<TestModel<string>>]
    public void WhenInterceptorIsSetViaStaticDefault_ConfigurationIsUsed(EntitySortFunction<TestModel<string>> sortFunc)
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

        Sorts.EntitySort.DefaultInterceptor = new NestedModelByValueInterceptor();
        var sortedItems = sortFunc(testItems, entitySort);
        sortedItems.Should().ContainInOrder(testItems[1], testItems[0], testItems[3], testItems[2]);

        // Cleanup
        Sorts.EntitySort.DefaultInterceptor = null;
    }

    private class NestedModelByValueInterceptor : ISortInterceptor
    {
        public IOrderedQueryable<TEntity>? OrderBy<TEntity>(IQueryable<TEntity> source, Sorts.PropertySort sort, SortConfiguration? configuration = null)
        {
            if (source is IQueryable<TestModel<string>> testModelSource && sort.PropertyPath == "NestedObject")
                return (IOrderedQueryable<TEntity>)testModelSource.OrderBy(x => x.NestedObject!.Value);

            return null;
        }

        public IOrderedQueryable<TEntity>? ThenBy<TEntity>(IOrderedQueryable<TEntity> source, Sorts.PropertySort sort, SortConfiguration? configuration = null)
        {
            if (source is IOrderedQueryable<TestModel<string>> testModelSource && sort.PropertyPath == "NestedObject")
                return (IOrderedQueryable<TEntity>)testModelSource.ThenBy(x => x.NestedObject!.Value);

            return null;
        }
    }
}
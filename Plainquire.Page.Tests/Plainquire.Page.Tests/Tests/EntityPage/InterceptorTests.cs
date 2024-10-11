using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Page.Tests.Models;
using Plainquire.Page.Tests.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.EntityPage;

[TestClass]
public class InterceptorTests
{
    [DataTestMethod]
    [PageFuncDataSource<TestModel<string>>]
    public void WhenPagedUsingInterceptorViaParameter_InterceptorIsCalledToCreatePage(EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var entityPage = new EntityPage<TestModel<string>>(1, 3);

        var interceptor = new FixedPageSizeInterceptor();

        var pagedItems = pageFunc(testItems, entityPage, interceptor);
        pagedItems.Should().Equal(testItems[0], testItems[1]);
    }

    private class FixedPageSizeInterceptor : IPageInterceptor
    {
        public IQueryable<TEntity> Page<TEntity>(IQueryable<TEntity> source, Page.EntityPage page)
        {
            var fixedPageSizePage = new Page.EntityPage
            {
                PageNumber = page.PageNumber,
                PageSize = 2
            };

            var (skip, take) = fixedPageSizePage.GetSkipAndTake();

            if (skip is > 0)
                source = source.Skip(skip.Value);

            if (take is > 0)
                source = source.Take(take.Value);

            return source;
        }
    }
}
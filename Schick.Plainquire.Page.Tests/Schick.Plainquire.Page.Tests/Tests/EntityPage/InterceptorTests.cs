using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schick.Plainquire.Page.Abstractions.Configurations;
using Schick.Plainquire.Page.Extensions;
using Schick.Plainquire.Page.Interfaces;
using Schick.Plainquire.Page.Tests.Models;
using Schick.Plainquire.Page.Tests.Services;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Schick.Plainquire.Page.Tests.Tests.EntityPage;

[TestClass, ExcludeFromCodeCoverage]
public class InterceptorTests
{
    [DataTestMethod]
    [PageFuncDataSource<TestModel<string>>]
    public void WhenPagedUsingInterceptorViaParameter_InterceptorIsCalledToCreatePage(EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var entityPage = new Pages.EntityPage<TestModel<string>>(1, 3);

        var interceptor = new FixedPageSizeInterceptor();

        var pagedItems = pageFunc(testItems, entityPage, null, interceptor);
        pagedItems.Should().Equal(testItems[0], testItems[1]);
    }

    [DataTestMethod]
    [PageFuncDataSource<TestModel<string>>]
    public void WhenPagedUsingDefaultInterceptor_InterceptorIsCalledToCreatePage(EntityPageFunction<TestModel<string>> pageFunc)
    {
        var testItems = new TestModel<string>[] { new("a"), new("b"), new("c"), new("d") };

        var entityPage = new Pages.EntityPage<TestModel<string>>(1, 3);

        Pages.EntityPage.DefaultInterceptor = new FixedPageSizeInterceptor();
        var pagedItems = pageFunc(testItems, entityPage);
        pagedItems.Should().Equal(testItems[0], testItems[1]);

        // Cleanup
        Pages.EntityPage.DefaultInterceptor = null;
    }

    private class FixedPageSizeInterceptor : IPageInterceptor
    {
        public IQueryable<TEntity> Page<TEntity>(IQueryable<TEntity> source, Pages.EntityPage page, PageConfiguration configuration)
        {
            var fixedPageSizePage = new Pages.EntityPage
            {
                PageNumber = page.PageNumber,
                PageSize = 2
            };

            var (skip, take) = fixedPageSizePage.GetSkipAndTake(configuration);

            if (skip is > 0)
                source = source.Skip(skip.Value);

            if (take is > 0)
                source = source.Take(take.Value);

            return source;
        }
    }
}
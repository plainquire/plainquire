using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Plainquire.Page.Mvc;
using Plainquire.Page.Mvc.ModelBinders;
using Plainquire.TestSupport.Services;
using System;
using System.Linq;

namespace Plainquire.Page.Tests.Tests.ModelBinder;

[TestFixture]
public class MvcBuilderExtensionsTests : TestContainer
{
    [Test]
    public void WhenPageSupportIsAdded_AllRequiredModelBindersAreRegistered()
    {
        var servCollection = new ServiceCollection();
        var mvcBuilderType = typeof(IMvcBuilder).Assembly.DefinedTypes.FirstOrDefault(x => x.Name == "MvcBuilder");
        var mvcBuilder = (IMvcBuilder)Activator.CreateInstance(mvcBuilderType!, servCollection, new ApplicationPartManager())!;

        mvcBuilder.AddPageSupport();

        using var serviceProvider = servCollection.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        using var _ = new AssertionScope();
        mvcOptions.ModelBinderProviders.Should().ContainSingle();
        mvcOptions.ModelBinderProviders.Should().Contain(x => x.GetType().Name == nameof(EntityPageModelBinderProvider));
    }
}
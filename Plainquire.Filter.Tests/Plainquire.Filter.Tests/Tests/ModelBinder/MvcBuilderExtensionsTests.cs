using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Mvc.ModelBinders;
using Plainquire.TestSupport.Services;
using System;
using System.Linq;

namespace Plainquire.Filter.Tests.Tests.ModelBinder;

[TestFixture]
public class MvcBuilderExtensionsTests : TestContainer
{
    [Test]
    public void WhenPageSupportIsAdded_AllRequiredModelBindersAreRegistered()
    {
        var servCollection = new ServiceCollection();
        var mvcBuilderType = typeof(IMvcBuilder).Assembly.DefinedTypes.FirstOrDefault(x => x.Name == "MvcBuilder");
        var mvcBuilder = (IMvcBuilder)Activator.CreateInstance(mvcBuilderType!, servCollection, new ApplicationPartManager())!;

        mvcBuilder.AddFilterSupport();

        using var serviceProvider = servCollection.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        using var _ = new AssertionScope();
        mvcOptions.ModelBinderProviders.Should().HaveCount(2);
        mvcOptions.ModelBinderProviders.Should().Contain(x => x.GetType().Name == nameof(EntityFilterModelBinderProvider));
        mvcOptions.ModelBinderProviders.Should().Contain(x => x.GetType().Name == nameof(EntityFilterSetModelBinderProvider));
    }
}
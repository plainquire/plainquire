﻿using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Sort.Mvc;
using Plainquire.Sort.Mvc.ModelBinders;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Plainquire.Sort.Tests.Tests.ModelBinder;

[TestClass]
public class MvcBuilderExtensionsTests
{
    [TestMethod]
    public void WhenSortSupportIsAdded_AllRequiredModelBindersAreRegistered()
    {
        var servCollection = new ServiceCollection();
        var mvcBuilderType = typeof(IMvcBuilder).Assembly.DefinedTypes.FirstOrDefault(x => x.Name == "MvcBuilder");
        var mvcBuilder = (IMvcBuilder)Activator.CreateInstance(mvcBuilderType!, servCollection, new ApplicationPartManager())!;

        mvcBuilder.AddSortSupport();

        using var serviceProvider = servCollection.BuildServiceProvider();
        var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        using var _ = new AssertionScope();
        mvcOptions.ModelBinderProviders.Should().HaveCount(2);
        mvcOptions.ModelBinderProviders.Should().Contain(x => x.GetType().Name == nameof(EntitySortModelBinderProvider));
        mvcOptions.ModelBinderProviders.Should().Contain(x => x.GetType().Name == nameof(EntitySortSetModelBinderProvider));
    }
}
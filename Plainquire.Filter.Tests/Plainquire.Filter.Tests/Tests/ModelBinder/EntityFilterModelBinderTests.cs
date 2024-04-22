using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Mvc.ModelBinders;
using Plainquire.Filter.Tests.Models;
using Plainquire.TestSupport.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Filter.Tests.Tests.ModelBinder;

[TestClass, ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityFilterModelBinderTests
{
    [TestMethod]
    public async Task WhenGenericEntityFilterIsGiven_ParameterBoundAsExpected()
    {
        // Arrange
        var serviceProvider = A.Fake<IServiceProvider>();
        A.CallTo(() => serviceProvider.GetService(default!)).WithAnyArguments().Returns(null);

        var queryParameters = new Dictionary<string, string> { ["TestModelDateTimeValueC"] = "Hello" };

        var binder = new EntityFilterModelBinder();
        const string actionName = nameof(EntityFilterController.SingleFilter);
        var filterBindingContext = BindingExtensions.CreateBindingContext<EntityFilterController>(actionName, "testModel", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(filterBindingContext);

        // Assert
        using var _ = new AssertionScope();

        var filter = (Filter.EntityFilter)filterBindingContext.Result.Model!;
        var propertyFilter = filter.PropertyFilters.Single();

        filterBindingContext.Result.IsModelSet.Should().BeTrue();
        propertyFilter.ValueFilters.Select(x => x.Value).Should().Equal(["Hello"]);
    }
}
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plainquire.Filter.Mvc.ModelBinders;
using Plainquire.Filter.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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

        var queryParameters = new Dictionary<string, string> { ["TestModelStringValueC"] = "Hello" };

        var binder = new EntityFilterModelBinder();
        const string actionName = nameof(EntityFilterController.SingleFilter);
        var filterBindingContext = CreateBindingContext<EntityFilterController>(actionName, "testModel", queryParameters, serviceProvider);

        // Act
        await binder.BindModelAsync(filterBindingContext);

        // Assert
        using var _ = new AssertionScope();

        var filter = (Filter.EntityFilter)filterBindingContext.Result.Model!;
        var propertyFilter = filter.PropertyFilters.Single();

        filterBindingContext.Result.IsModelSet.Should().BeTrue();
        propertyFilter.ValueFilters.Select(x => x.Value).Should().Equal(["Hello"]);
    }

    private static ModelBindingContext CreateBindingContext<TController>(string actionName, string parameterName, Dictionary<string, string> queryParameters, IServiceProvider serviceProvider)
    {
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request = { QueryString = new QueryBuilder(queryParameters).ToQueryString() },
                RequestServices = serviceProvider
            }
        };

        var bindingSource = new BindingSource("Query", "Query", false, true);
        var routeValueDictionary = new RouteValueDictionary(queryParameters!);
        var valueProvider = new RouteValueProvider(bindingSource, routeValueDictionary);

        var parameterInfo = typeof(TController)
            .GetMethod(actionName)?
            .GetParameters()
            .FirstOrDefault(parameter => parameter.Name == parameterName)
            ?? throw new ArgumentException("Method or parameter not found", nameof(actionName));

        var modelMetadata = (DefaultModelMetadata)new EmptyModelMetadataProvider().GetMetadataForParameter(parameterInfo, parameterInfo.ParameterType);
        var binderModelName = parameterInfo.GetCustomAttribute<FromQueryAttribute>()?.Name;

        var bindingContext = DefaultModelBindingContext
            .CreateBindingContext(
                actionContext,
                valueProvider,
                modelMetadata,
                bindingInfo: null,
                binderModelName ?? parameterInfo.Name ?? throw new InvalidOperationException("Parameter name not found")
            );

        return bindingContext;
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.TestSupport.Extensions;

public static class BindingExtensions
{
    public static ModelBindingContext CreateBindingContext<TController>(string actionName, string parameterName, Dictionary<string, string> queryParameters, IServiceProvider serviceProvider)
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
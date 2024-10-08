using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerGen.Test;
using Swashbuckle.AspNetCore.TestSupport;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Plainquire.Swashbuckle.TestSupport.Services;

[ExcludeFromCodeCoverage]
public static class SwaggerGeneratorFactory
{
    public static SwaggerGenerator Create<TController>(string actionName, List<IOperationFilter> operationFilters) where TController : new()
    {
        var apiDescription = GetApiDescription<TController>(actionName);

        var generator = new SwaggerGenerator(
            new SwaggerGeneratorOptions
            {
                SwaggerDocs = new Dictionary<string, OpenApiInfo>
                {
                    ["v1"] = new() { Version = "v1", Title = "Test API" }
                },
                OperationFilters = operationFilters
            },

            new FakeApiDescriptionGroupCollectionProvider([apiDescription]),
            new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new JsonSerializerOptions())),
            null
        );

        return generator;
    }

    private static ApiDescription GetApiDescription<TController>(string actionName) where TController : new()
    {
        var methodInfo = typeof(TController).GetMethod(actionName);
        if (methodInfo == null)
            throw new ArgumentException("Method not found", nameof(actionName));

        var methodName = methodInfo.Name;
        var httpMethodAttributes = methodInfo
            .GetCustomAttributes(typeof(HttpMethodAttribute), true)
            .OfType<HttpMethodAttribute>()
            .ToList();
        var httpMethod = httpMethodAttributes.FirstOrDefault()?.HttpMethods.FirstOrDefault() ?? HttpMethods.Get;

        var parameters = methodInfo
            .GetParameters()
            .Select(parameter => new ApiParameterDescription
            {
                Name = parameter.Name ?? throw new ArgumentNullException(nameof(parameter.Name)),
                Source = GetBindingSource(parameter, httpMethod)
            })
            .ToArray();

        var apiDescription = ApiDescriptionFactory
            .Create<TController>(
                _ => methodName,
                groupName: "v1",
                httpMethod: httpMethod,
                relativePath: methodName,
                parameterDescriptions: parameters
            );

        return apiDescription;
    }

    private static BindingSource GetBindingSource(ParameterInfo parameter, string httpMethod)
    {
        var bindingSource = parameter
            .GetCustomAttributes()
            .OfType<IBindingSourceMetadata>()
            .FirstOrDefault()?
            .BindingSource;

        if (bindingSource != null)
            return bindingSource;

        return httpMethod == HttpMethods.Get
            ? BindingSource.Query
            : BindingSource.Body;
    }
}
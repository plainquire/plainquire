using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Plainquire.Demo.Extensions;
using Plainquire.Demo.Routing;
using Plainquire.Filter.Swashbuckle;
using Plainquire.Page.Swashbuckle;
using Plainquire.Sort.Swashbuckle;
using System;
using System.IO;

namespace Plainquire.Demo.Startup;

internal static class OpenApi
{
    public const string API_UI_ROUTE = "api/";
    private const string OPEN_API_UI_ROUTE = "/openapi";
    private const string SWAGGER_UI_ROUTE = "/swagger";
    private const string OPEN_API_SPEC = "openapi.json";

    internal static IApplicationBuilder RegisterOpenApiRoutes(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            if (context.Request.Method == HttpMethods.Get && context.Request.Path.RedirectRequired())
                context.Response.Redirect($"/{API_UI_ROUTE}");

            await next();
        });

        return applicationBuilder
            .UseSwagger(c => c.RouteTemplate = $"{API_UI_ROUTE}{{documentName}}/{OPEN_API_SPEC}")
            .UseSwaggerUI(c =>
            {
                c.RoutePrefix = API_UI_ROUTE.Trim('/');
                c.SwaggerEndpoint($"{V1ApiController.API_VERSION}/{OPEN_API_SPEC}", $"API version {V1ApiController.API_VERSION}");
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableTryItOutByDefault();
                c.ConfigObject.AdditionalItems.Add("requestSnippetsEnabled", true);
                c.InjectStylesheet("/css/swagger-ui/custom.css");
            });
    }

    internal static IServiceCollection RegisterOpenApiController(this IServiceCollection services)
    {
        var filterExpressionDoc = Path.Combine(AppContext.BaseDirectory, "Plainquire.Filter.xml");
        var sortQueryableDoc = Path.Combine(AppContext.BaseDirectory, "Plainquire.Sort.xml");
        var pageQueryableDoc = Path.Combine(AppContext.BaseDirectory, "Plainquire.Page.xml");
        var plainquireDemoDoc = Path.Combine(AppContext.BaseDirectory, "Plainquire.Demo.xml");

        return services
            .AddSwaggerGenNewtonsoftSupport()
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc(V1ApiController.API_VERSION, new OpenApiInfo { Title = $"{AssemblyExtensions.GetProgramProduct()} API", Version = V1ApiController.API_VERSION });
                c.AddFilterSupport(filterExpressionDoc, plainquireDemoDoc);
                c.AddSortSupport(sortQueryableDoc, plainquireDemoDoc);
                c.AddPageSupport(pageQueryableDoc, plainquireDemoDoc);
                c.IncludeXmlComments(filterExpressionDoc);
                c.IncludeXmlComments(sortQueryableDoc);
                c.IncludeXmlComments(pageQueryableDoc);
                c.IncludeXmlComments(plainquireDemoDoc);
            });
    }

    private static bool RedirectRequired(this PathString path) =>
        path.StartsWithSegments(SWAGGER_UI_ROUTE) || path.StartsWithSegments(OPEN_API_UI_ROUTE);
}
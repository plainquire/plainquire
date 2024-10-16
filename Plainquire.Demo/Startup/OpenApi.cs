using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Plainquire.Demo.Extensions;
using Plainquire.Demo.Routing;
using Plainquire.Filter.Abstractions;
using Plainquire.Filter.Swashbuckle;
using Plainquire.Page.Swashbuckle;
using Plainquire.Sort.Abstractions;
using Plainquire.Sort.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics.CodeAnalysis;
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
            var isGetMethod = string.Equals(context.Request.Method, HttpMethods.Get, StringComparison.Ordinal);

            if (isGetMethod && context.Request.Path.IsOpenApiRoute())
                context.Response.Redirect($"/{API_UI_ROUTE}");

            if (isGetMethod && context.Request.Path.StartsWithSegments("/syntax", StringComparison.OrdinalIgnoreCase))
                context.Response.Redirect("https://github.com/plainquire/plainquire#syntax");

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
                c.DocumentFilter<RemoveUnusedSchemata>();
                c.IncludeXmlComments(filterExpressionDoc);
                c.IncludeXmlComments(sortQueryableDoc);
                c.IncludeXmlComments(pageQueryableDoc);
                c.IncludeXmlComments(plainquireDemoDoc);
            });
    }

    private static bool IsOpenApiRoute(this PathString path) =>
        path.StartsWithSegments(SWAGGER_UI_ROUTE, StringComparison.OrdinalIgnoreCase) ||
        path.StartsWithSegments(OPEN_API_UI_ROUTE, StringComparison.OrdinalIgnoreCase);

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
    private class RemoveUnusedSchemata : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Components.Schemas.Remove(nameof(FilterConditionalAccess));
            swaggerDoc.Components.Schemas.Remove(nameof(SortConditionalAccess));
            swaggerDoc.Components.Schemas.Remove(nameof(FilterOperator));
        }
    }
}
using FilterExpressionCreator.Demo.Extensions;
using FilterExpressionCreator.Demo.Routing;
using FilterExpressionCreator.Swashbuckle.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace FilterExpressionCreator.Demo.Startup
{
    internal static class OpenApi
    {
        public const string OPEN_API_UI_ROUTE = "openapi/";
        public const string OPEN_API_SPEC = "openapi.json";

        internal static IApplicationBuilder RegisterOpenApiRoutes(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseSwagger(c => c.RouteTemplate = $"{OPEN_API_UI_ROUTE}{{documentName}}/{OPEN_API_SPEC}")
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = OPEN_API_UI_ROUTE.Trim('/');
                    c.SwaggerEndpoint($"{V1ApiController.API_VERSION}/{OPEN_API_SPEC}", $"API version {V1ApiController.API_VERSION}");
                    c.DisplayRequestDuration();
                });

        internal static IServiceCollection RegisterOpenApiController(this IServiceCollection services)
        {
            var filterExpressionCreatorDoc = Path.Combine(AppContext.BaseDirectory, "FilterExpressionCreator.xml");
            var filterExpressionCreatorDemoDoc = Path.Combine(AppContext.BaseDirectory, "FilterExpressionCreator.Demo.xml");

            return services
                .AddSwaggerGenNewtonsoftSupport()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(V1ApiController.API_VERSION, new OpenApiInfo { Title = $"{AssemblyExtensions.GetProgramProduct()} API", Version = V1ApiController.API_VERSION });
                    c.AddFilterExpressionsSupport(filterExpressionCreatorDoc, filterExpressionCreatorDemoDoc);
                    c.IncludeXmlComments(filterExpressionCreatorDoc);
                    c.IncludeXmlComments(filterExpressionCreatorDemoDoc);
                });
        }
    }
}

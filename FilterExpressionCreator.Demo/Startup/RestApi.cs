using FilterExpressionCreator.Mvc.Extensions;
using FilterExpressionCreator.Mvc.Newtonsoft.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace FilterExpressionCreator.Demo.Startup
{
    internal static class RestApi
    {
        public static IApplicationBuilder RegisterRestApiRoutes(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseEndpoints(endpoints => endpoints.MapControllers());

        public static IServiceCollection RegisterRestApiController(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddFilterExpressionsSupport()
                .AddFilterExpressionsNewtonsoftSupport();

            return services;
        }
    }
}

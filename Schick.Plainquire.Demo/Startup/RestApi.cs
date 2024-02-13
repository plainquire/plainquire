using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Schick.Plainquire.Filter.Mvc.Extensions;
using Schick.Plainquire.Filter.Mvc.Newtonsoft.Extensions;
using Schick.Plainquire.Page.Mvc.Extensions;
using Schick.Plainquire.Sort.Mvc.Extensions;
using Schick.Plainquire.Sort.Mvc.Newtonsoft.Extensions;

namespace Schick.Plainquire.Demo.Startup;

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
            .AddFilterExpressionSupport()
            .AddFilterExpressionNewtonsoftSupport()
            .AddSortQueryableSupport()
            .AddSortQueryableNewtonsoftSupport()
            .AddPageQueryableSupport();

        return services;
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Mvc.Newtonsoft;
using Plainquire.Page.Mvc;
using Plainquire.Page.Mvc.Newtonsoft;
using Plainquire.Sort.Mvc;
using Plainquire.Sort.Mvc.Newtonsoft;

namespace Plainquire.Demo.Startup;

internal static class RestApi
{
    public static IApplicationBuilder RegisterRestApiRoutes(this IApplicationBuilder applicationBuilder)
        => applicationBuilder
            .UseEndpoints(endpoints => endpoints.MapControllers());

    public static void RegisterRestApiController(this IServiceCollection services)
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
            .AddFilterSupport()
            .AddFilterNewtonsoftSupport()
            .AddSortSupport()
            .AddSortNewtonsoftSupport()
            .AddPageSupport()
            .AddPageNewtonsoftSupport();
    }
}
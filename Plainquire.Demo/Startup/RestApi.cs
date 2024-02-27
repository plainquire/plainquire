using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Plainquire.Filter.Mvc.Extensions;
using Plainquire.Filter.Mvc.Newtonsoft.Extensions;
using Plainquire.Page.Mvc.Extensions;
using Plainquire.Page.Mvc.Newtonsoft.Extensions;
using Plainquire.Sort.Mvc.Extensions;
using Plainquire.Sort.Mvc.Newtonsoft.Extensions;

namespace Plainquire.Demo.Startup;

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
            .AddFilterSupport()
            .AddFilterNewtonsoftSupport()
            .AddSortSupport()
            .AddSortNewtonsoftSupport()
            .AddPageSupport()
            .AddPageNewtonsoftSupport();

        return services;
    }
}
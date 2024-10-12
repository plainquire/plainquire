
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Plainquire.Filter.Mvc;
using Plainquire.Filter.Mvc.Newtonsoft;
using Plainquire.Filter.Swashbuckle;
using Plainquire.Page.Mvc;
using Plainquire.Page.Mvc.Newtonsoft;
using Plainquire.Page.Swashbuckle;
using Plainquire.Sort.Mvc;
using Plainquire.Sort.Mvc.Newtonsoft;
using Plainquire.Sort.Swashbuckle;

namespace Plainquire.Integration.Tests.TestSupport;

public class Program
{
    private const string API_VERSION = "V1";
    private const string API_UI_ROUTE = "api/";
    private const string OPEN_API_SPEC = "openapi.json";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var useNewtonsoftArgument = builder.Configuration["use-newtonsoft"];
        var useNewtonsoft = useNewtonsoftArgument != null && useNewtonsoftArgument != "false";

        var mvcBuilder = builder.Services.AddControllers();

        mvcBuilder
            .AddFilterSupport()
            .AddSortSupport()
            .AddPageSupport();

        if (useNewtonsoft)
        {
            mvcBuilder
                .AddNewtonsoftJson()
                .AddFilterNewtonsoftSupport()
                .AddSortNewtonsoftSupport()
                .AddPageNewtonsoftSupport();
        }

        builder.Services.AddEndpointsApiExplorer();

        if (useNewtonsoft)
            builder.Services.AddSwaggerGenNewtonsoftSupport();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(API_VERSION, new OpenApiInfo { Title = " Integration Test Support", Version = API_VERSION });
            options.AddFilterSupport();
            options.AddSortSupport();
            options.AddPageSupport();
        });

        var app = builder.Build();

        app.UseSwagger(c => c.RouteTemplate = $"{API_UI_ROUTE}{{documentName}}/{OPEN_API_SPEC}");
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = API_UI_ROUTE.Trim('/');
            c.SwaggerEndpoint($"{API_VERSION}/{OPEN_API_SPEC}", $"API version {API_VERSION}");
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableTryItOutByDefault();
            c.ConfigObject.AdditionalItems.Add("requestSnippetsEnabled", true);
        });

        app.MapControllers();

        app.Run();
    }
}
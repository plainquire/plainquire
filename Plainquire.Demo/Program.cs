using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Plainquire.Demo.Models.Configuration;
using Plainquire.Demo.Startup;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Plainquire.Demo;

internal static class Program
{
    private const string CONFIG_BASE_NAME = "Plainquire.Demo.config";

    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args)
            .UseWindowsService()
            .UseSystemd()
            .Build();

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.ClearConfiguration();
                builder.AddConfigurationFromEnvironment(args);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(ConfigureServerServices);
                webBuilder.Configure((builder, app) => ConfigureServerApplication(app, builder.HostingEnvironment));
            });
    }

    private static void AddConfigurationFromEnvironment(this IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
    {
        configurationBuilder
            .AddJsonFile($"{CONFIG_BASE_NAME}.json", false, true)
            .AddJsonFile($"{CONFIG_BASE_NAME}.Development.json", true, true)
            .AddEnvironmentVariables()
            .AddCommandLine(commandLineArgs);
    }

    private static void ClearConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        var chainedConfigurationSource = configurationBuilder.Sources.OfType<ChainedConfigurationSource>().First();
        configurationBuilder.Sources.Clear();
        configurationBuilder
            .Add(chainedConfigurationSource);
    }

    private static void ConfigureServerServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services
            .Configure<FilterConfiguration>(context.Configuration.GetSection(FilterConfiguration.CONFIGURATION_SECTION))
            .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<FilterConfiguration>>().Value)
            .RegisterApplicationServices()
            .RegisterOpenApiController()
            .RegisterRestApiController();

        services.AddRazorPages();
        services.AddServerSideBlazor();
    }

    private static void ConfigureServerApplication(IApplicationBuilder applicationBuilder, IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment.IsDevelopment())
        {
#if DEBUG
            applicationBuilder.UseDeveloperExceptionPage();
            applicationBuilder.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
#endif
        }
        else
        {
            applicationBuilder.UseExceptionHandler("/Error");
            applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseHsts();
        }

        applicationBuilder
            .AddLocalizationSupport()
            .UseStaticFiles()
            .UseRouting()
            .RegisterOpenApiRoutes()
            .RegisterRestApiRoutes()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
    }

    private static IApplicationBuilder AddLocalizationSupport(this IApplicationBuilder applicationBuilder)
    {
        var supportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(x => x.Name).ToArray();
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures.First(culture => culture.Equals("en-US", StringComparison.Ordinal)))
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        return applicationBuilder.UseRequestLocalization(localizationOptions);
    }
}
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Demo.Database;

namespace Plainquire.Demo.Startup;

internal static class Services
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddDbContext<FreelancerDbContext>();
        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;
using Plainquire.Demo.Database;

namespace Plainquire.Demo.Startup;

internal static class Services
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddDbContext<FreelancerDbContext>();
        services.AddHttpClient();
        return services;
    }
}
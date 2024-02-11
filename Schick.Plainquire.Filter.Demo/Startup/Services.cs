using Microsoft.Extensions.DependencyInjection;
using Schick.Plainquire.Demo.Database;

namespace Schick.Plainquire.Demo.Startup;

internal static class Services
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddDbContext<FreelancerDbContext>();
        services.AddHttpClient();
        return services;
    }
}
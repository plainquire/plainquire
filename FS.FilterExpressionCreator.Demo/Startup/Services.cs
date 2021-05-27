using FS.FilterExpressionCreator.Demo.Database;
using Microsoft.Extensions.DependencyInjection;

namespace FS.FilterExpressionCreator.Demo.Startup
{
    internal static class Services
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddDbContext<FreelancerDbContext>();
            services.AddHttpClient();
            return services;
        }
    }
}

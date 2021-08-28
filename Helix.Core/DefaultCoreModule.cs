using Helix.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.Core
{
    public static class DefaultCoreModule
    {
        public static IServiceCollection AddDefaultCoreModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultDomainModule(configuration);
            return services;
        }
    }
}

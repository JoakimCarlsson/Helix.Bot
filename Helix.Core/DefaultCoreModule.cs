using Helix.Core.Abstractions;
using Helix.Core.Services;
using Helix.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.Core
{
    public static class DefaultCoreModule
    {
        public static IServiceCollection AddDefaultCoreModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLazyCache();
            services.AddDefaultDomainModule(configuration);
            services.AddTransient<IGuildService, GuildService>();
            return services;
        }
    }
}

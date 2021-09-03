using System.Linq;
using Helix.BackgroundWorker;
using Helix.Core.Abstractions;
using Helix.Core.Services;
using Helix.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remora.Discord.Gateway.Extensions;

namespace Helix.Core
{
    public static class DefaultCoreModule
    {
        public static IServiceCollection AddDefaultCoreModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLazyCache();
            services.AddDefaultDomainModule(configuration);
            services.AddDefaultBackgroundWorker();
            services.AddScoped<IGuildService, GuildService>();
            services.AddScoped<IUserService, UserService>();
            
            var responderTypes = typeof(DefaultCoreModule).Assembly
                .GetExportedTypes()
                .Where(t => t.IsResponder());

            foreach (var responderType in responderTypes)
            {
                services.AddResponder(responderType);
            }
            
            return services;
        }
    }
}

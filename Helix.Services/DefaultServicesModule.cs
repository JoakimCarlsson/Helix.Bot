using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helix.Domain;
using Helix.Services.Abstractions;
using Helix.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.Services
{
    public static class DefaultServicesModule
    {
        public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddLazyCache()
                .AddDefaultDomainModule(configuration)
                .AddTransient<IGuildService, GuildService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IUserReminderService, UserReminderService>();

            return services;
        }
    }
}

using Helix.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.Domain
{
    public static class DefaultDomainModule
    {
        public static IServiceCollection AddDefaultDomainModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<HelixDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
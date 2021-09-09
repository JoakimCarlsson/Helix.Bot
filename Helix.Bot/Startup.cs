using Helix.BackgroundWorker;
using Helix.Bot.Abstractions;
using Helix.Bot.Extensions;
using Helix.Bot.Services;
using Helix.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helix.Bot
{
    class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLazyCache()
                .AddDefaultDomainModule(_configuration)
                .AddDefaultBackgroundWorker()
                .AddDiscordBotClient(_configuration, true)
                .AddScoped<IGuildService, GuildService>()
                .AddScoped<IUserService, UserService>()
                .AddHostedService<BotClient>();
        }
    }
}

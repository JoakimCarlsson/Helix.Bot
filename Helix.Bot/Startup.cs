using Helix.BackgroundWorker;
using Helix.Bot.Extensions;
using Helix.Bot.Helpers;
using Helix.Bot.Helpers.Abstractions;
using Helix.Bot.Infrastructure;
using Helix.Domain;
using Helix.Services;
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
                .AddDefaultBackgroundWorker()
                .AddDefaultDomainModule(_configuration)
                .AddDefaultServices(_configuration)
                .AddDiscordBotClient(_configuration, true)
                .AddTransient<IRespondService, RespondService>()
                .AddHostedService<ReminderBackgroundService>()
                .AddHostedService<BotClient>();
        }
    }
}

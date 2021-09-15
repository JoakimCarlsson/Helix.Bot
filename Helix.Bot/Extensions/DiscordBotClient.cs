using System.Linq;
using Helix.Bot.Commands;
using Helix.Bot.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Responders;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;

namespace Helix.Bot.Extensions
{
    public static class DiscordBotClient
    {
        public static IServiceCollection AddDiscordBotClient(this IServiceCollection services, IConfiguration configuration, bool suppressAutomaticResponses)
        {
            services
                .AddDiscordGateway(_ => configuration["DiscordConfiguration:BotToken"])
                .Configure<DiscordGatewayClientOptions>(o =>
                {
                    o.Intents |= GatewayIntents.GuildPresences;
                    o.Intents |= GatewayIntents.GuildVoiceStates;
                    o.Intents |= GatewayIntents.GuildMembers;
                    o.Intents |= GatewayIntents.GuildMessages;
                })
                .AddDiscordCommands(true)
                .AddDiscordCaching()
                .AddCommands()
                .AddCommandGroup<ReminderCommands>()
                .AddParser<TimeSpanParser>()
                .Configure<CommandResponderOptions>(o => o.Prefix = ">!");

            services.AddInteractionResponder(x => x.SuppressAutomaticResponses = suppressAutomaticResponses);

            var responderTypes = typeof(Program).Assembly
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

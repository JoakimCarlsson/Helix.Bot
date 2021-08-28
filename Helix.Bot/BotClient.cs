using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.Commands.Services;
using Remora.Discord.Core;
using Remora.Discord.Gateway;

namespace Helix.Bot
{
    class BotClient : IHostedService
    {
        private readonly ILogger<BotClient> _logger;
        private readonly IConfiguration _configuration;
        private readonly SlashService _slashService;
        private readonly DiscordGatewayClient _discordGatewayClient;
        private readonly HelixDbContext _dbContext;

        public BotClient(ILogger<BotClient> logger, IConfiguration configuration, SlashService slashService, DiscordGatewayClient discordGatewayClient, HelixDbContext dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _slashService = slashService;
            _discordGatewayClient = discordGatewayClient;
            _dbContext = dbContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting service");

            var migrations = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            if (migrations.Any())
            {
                _logger.LogInformation("Applying Migrations: {pending}", migrations);
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }

            Snowflake? debugGuild = null;
#if DEBUG
            var debugGuildId = _configuration["DiscordConfiguration:DebugGuildId"];
            if (debugGuildId is not null)
            {
                if (!Snowflake.TryParse(debugGuildId, out debugGuild))
                {
                    _logger.LogCritical("Could not parse Guild Id");
                    throw new Exception("Could not parse guild Id");
                }
            }
#endif

            var slashSupport = _slashService.SupportsSlashCommands();
            if (!slashSupport.IsSuccess)
            {
                _logger.LogWarning($"The registered commands of the bot don't support slash commands: {slashSupport.Error.Message}");
                throw new Exception("Unable to continue.");
            }

            var updateSlashCommands = await _slashService.UpdateSlashCommandsAsync(debugGuild, cancellationToken);
            if (!updateSlashCommands.IsSuccess)
            {
                _logger.LogWarning($"Failed to update slash commands: {updateSlashCommands.Error.Message}");
                throw new Exception("Unable to continue.");
            }

            var runResult = await _discordGatewayClient.RunAsync(cancellationToken);

            if (!runResult.IsSuccess)
            {
                _logger.LogCritical(runResult.Error.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot client stopping.");
            _discordGatewayClient.Dispose();
            return Task.CompletedTask;
        }
    }
}

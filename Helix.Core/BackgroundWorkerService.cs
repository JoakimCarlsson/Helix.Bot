using System;
using System.Threading;
using System.Threading.Tasks;
using Helix.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Objects;

namespace Helix.Core
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio
    class BackgroundWorkerService : BackgroundService
    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceProvider _services;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IBackgroundTaskQueue backgroundTaskQueue, IServiceProvider services)
        {
            _logger = logger;
            _backgroundTaskQueue = backgroundTaskQueue;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Task queue is running");
            await ProcessItems(stoppingToken);
        }

        private async ValueTask ProcessItems(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workEvent = await _backgroundTaskQueue.DequeueAsync(cancellationToken);
                try
                {
                    using var scope = _services.CreateScope();
                    var services = scope.ServiceProvider;

                    _logger.LogInformation("Executing {workEvent}", workEvent);
                    switch (workEvent)
                    {
                        case AddGuildEvent addGuildEvent:
                            await services.GetRequiredService<IGuildService>().AddGuildAsync(addGuildEvent.GuildId, cancellationToken);
                            break;
                        case AddGuildMemberEvent addGuildMemberEvent:
                            await services.GetRequiredService<IUserService>().AddUserAsync(addGuildMemberEvent.UserId, addGuildMemberEvent.GuildId, addGuildMemberEvent.FirstSeen, cancellationToken);
                            break;
                        case RemoveGuildMemberEvent removeGuildMemberEvent:
                            await services.GetRequiredService<IUserService>().DeleteUserAsync(removeGuildMemberEvent.UserId, removeGuildMemberEvent.GuildId, cancellationToken);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(workEvent));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Error occurred executing {item}\n{ex}", workEvent, e);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}

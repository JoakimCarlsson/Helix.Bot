using System.Threading;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Absractions;
using Helix.Core.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Helix.Core.Responders
{
    public class GuildReadyResponder : IResponder<IGuildCreate>
    {
        private readonly IGuildService _guildService;
        private readonly IBackgroundTaskQueue _backgroundTask;

        public GuildReadyResponder(IGuildService guildService, IBackgroundTaskQueue backgroundTask)
        {
            _guildService = guildService;
            _backgroundTask = backgroundTask;
        }
        
        public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _backgroundTask.QueueAsync(new AddGuildEvent(gatewayEvent.ID.Value));

            await _guildService.AddGuildAsync(gatewayEvent.ID.Value, ct);
            return Result.FromSuccess();
        }
    }
}
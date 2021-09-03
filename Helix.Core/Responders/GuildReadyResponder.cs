using System.Threading;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Abstractions;
using Helix.Core.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Helix.Core.Responders
{
    public class GuildReadyResponder : IResponder<IGuildCreate>
    {
        private readonly IWorkQueueService _workQueueService;

        public GuildReadyResponder(IWorkQueueService workQueueService)
        {
            _workQueueService = workQueueService;
        }
        
        public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _workQueueService.QueueAsync<IGuildService>(async x => await x.AddGuildAsync(gatewayEvent.ID.Value, ct));
            return Result.FromSuccess();
        }
    }
}
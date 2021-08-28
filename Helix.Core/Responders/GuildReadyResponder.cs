using System.Threading;
using System.Threading.Tasks;
using Helix.Core.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Helix.Core.Responders
{
    public class GuildReadyResponder : IResponder<IGuildCreate>
    {
        private readonly IGuildService _guildService;

        public GuildReadyResponder(IGuildService guildService)
        {
            _guildService = guildService;
        }
        
        public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _guildService.AddGuild(gatewayEvent.ID.Value, ct);
            return Result.FromSuccess();
        }
    }
}
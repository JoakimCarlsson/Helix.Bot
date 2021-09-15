using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Helix.Bot.Responders
{
    public class ThreadJoinResponder : IResponder<IThreadCreate>
    {
        private readonly IDiscordRestChannelAPI _channelApi;

        public ThreadJoinResponder(IDiscordRestChannelAPI channelApi)
        {
            _channelApi = channelApi;
        }

        public async Task<Result> RespondAsync(IThreadCreate gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _channelApi.JoinThreadAsync(gatewayEvent.ID, ct);
            return Result.FromSuccess();
        }
    }
}

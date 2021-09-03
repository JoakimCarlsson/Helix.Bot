using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.BackgroundWorker.Abstractions;
using Helix.Core.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Helix.Core.Responders
{
    public class GuildMemberResponder : IResponder<IGuildMemberAdd>, IResponder<IGuildMemberRemove>
    {
        private readonly IWorkQueueService _workQueueService;

        public GuildMemberResponder(IWorkQueueService workQueueService)
        {
            _workQueueService = workQueueService;
        }

        public async Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _workQueueService.QueueAsync<IUserService>(async x => await x.AddUserAsync(gatewayEvent.User.Value.ID.Value, gatewayEvent.GuildID.Value, gatewayEvent.JoinedAt.DateTime, ct));
            return Result.FromSuccess();
        }

        public async Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _workQueueService.QueueAsync<IUserService>(async x => await x.DeleteUserAsync(gatewayEvent.User.ID.Value, gatewayEvent.GuildID.Value, ct));
            return Result.FromSuccess();
        }
    }
}

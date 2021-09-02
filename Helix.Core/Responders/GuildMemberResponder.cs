using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.Core.Abstractions;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Helix.Core.Responders
{
    public class GuildMemberResponder : IResponder<IGuildMemberAdd>, IResponder<IGuildMemberRemove>
    {
        private readonly IUserService _userService;
        private readonly IBackgroundTaskQueue _backgroundTask;

        public GuildMemberResponder(IUserService userService, IBackgroundTaskQueue backgroundTask)
        {
            _userService = userService;
            _backgroundTask = backgroundTask;
        }

        public async Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            //await _userService.AddUserAsync(gatewayEvent.User.Value.ID.Value, gatewayEvent.GuildID.Value, gatewayEvent.JoinedAt.DateTime, ct);
            await _backgroundTask.QueueAsync(new AddGuildMemberEvent(gatewayEvent.User.Value.ID.Value, gatewayEvent.GuildID.Value, gatewayEvent.JoinedAt.DateTime));
            return Result.FromSuccess();
        }

        public async Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            //await _userService.DeleteUserAsync(gatewayEvent.User.ID.Value, gatewayEvent.GuildID.Value, ct);
            await _backgroundTask.QueueAsync(new RemoveGuildMemberEvent(gatewayEvent.User.ID.Value, gatewayEvent.GuildID.Value));
            return Result.FromSuccess();
        }
    }
}

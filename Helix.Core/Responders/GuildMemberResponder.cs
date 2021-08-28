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

        public GuildMemberResponder(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _userService.AddUser(gatewayEvent.User.Value.ID.Value, gatewayEvent.GuildID.Value, gatewayEvent.JoinedAt.DateTime, ct);
            return Result.FromSuccess();
        }

        public async Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = new CancellationToken())
        {
            await _userService.DeleteUser(gatewayEvent.User.ID.Value, gatewayEvent.GuildID.Value, ct);
            return Result.FromSuccess();
        }
    }
}

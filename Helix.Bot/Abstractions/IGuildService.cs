using System.Threading;
using System.Threading.Tasks;
using Helix.Bot.Response;
using Helix.Domain.Models;

namespace Helix.Bot.Abstractions
{
    public interface IGuildService
    {
        public Task<ServiceResponse<Guild>> AddGuildAsync(ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<bool>> GuildExistsAsync(ulong guildId, CancellationToken cancellationToken = default);
    }
}
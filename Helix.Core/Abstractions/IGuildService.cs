using System.Threading;
using System.Threading.Tasks;
using Helix.Core.Response;
using Helix.Domain.Models;

namespace Helix.Core.Abstractions
{
    public interface IGuildService
    {
        public Task<ServiceResponse<Guild>> AddGuildAsync(ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<bool>> GuildExistsAsync(ulong guildId, CancellationToken cancellationToken = default);
    }
}
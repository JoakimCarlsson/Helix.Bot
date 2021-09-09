using System.Threading;
using System.Threading.Tasks;
using Helix.Domain.Models;
using Helix.Services.Services;

namespace Helix.Services.Abstractions
{
    public interface IGuildService
    {
        public Task<ServiceResponse<Guild>> AddGuildAsync(ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<bool>> GuildExistsAsync(ulong guildId, CancellationToken cancellationToken = default);
    }
}
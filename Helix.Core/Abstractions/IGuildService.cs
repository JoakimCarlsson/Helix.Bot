using System.Threading;
using System.Threading.Tasks;
using Helix.Core.Response;
using Helix.Domain.Models;

namespace Helix.Core.Abstractions
{
    public interface IGuildService
    {
        public ValueTask<ServiceResponse<Guild>> AddGuildAsync(ulong guildId, CancellationToken cancellationToken = default);
        public ValueTask<ServiceResponse<bool>> GuildExistsAsync(ulong guildId, CancellationToken cancellationToken = default);
    }
}
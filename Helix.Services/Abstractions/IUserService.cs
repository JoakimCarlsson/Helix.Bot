using System;
using System.Threading;
using System.Threading.Tasks;
using Helix.Domain.Models;
using Helix.Services.Services;

namespace Helix.Services.Abstractions
{
    public interface IUserService
    {
        public Task<ServiceResponse<User>> AddUserAsync(ulong userId, ulong guildId, DateTime firstSeen, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<User>> GetUserAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse> DeleteUserAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<bool>> UserExistAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
    }
}
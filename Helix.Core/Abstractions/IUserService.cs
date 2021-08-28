using System;
using System.Threading;
using System.Threading.Tasks;
using Helix.Core.Response;
using Helix.Domain.Models;

namespace Helix.Core.Abstractions
{
    public interface IUserService
    {
        public Task<ServiceResponse<User>> AddUser(ulong userId, ulong guildId, DateTime firstSeen, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<User>> GetUser(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse> DeleteUser(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
        public Task<ServiceResponse<bool>> UserExist(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
    }
}
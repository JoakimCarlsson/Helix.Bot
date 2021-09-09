using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helix.Bot.Abstractions;
using Helix.Bot.Response;
using Helix.Domain.Data;
using Helix.Domain.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helix.Bot.Services
{
    public class UserService : IUserService
    {
        private readonly HelixDbContext _dbContext;
        private readonly IAppCache _appCache;
        private readonly ILogger<UserService> _logger;

        public UserService(HelixDbContext dbContext, IAppCache appCache, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _appCache = appCache;
            _logger = logger;
        }

        public async Task<ServiceResponse<User>> AddUserAsync(ulong userId, ulong guildId, DateTime firstSeen, CancellationToken cancellationToken = default)
        {
            try
            {
                var userExist = await UserExistAsync(userId, guildId, cancellationToken);
                if (userExist.Entity)
                    return ServiceResponse<User>.Fail(new ErrorResult("User already exist."));

                var user = new User
                {
                    UserId = userId,
                    GuildId = guildId,
                    FirstSeen = firstSeen,
                    LastSeen = firstSeen
                };

                var addedEntity = await _dbContext.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            
                _appCache.Add($"user:{userId}:{guildId}", addedEntity.Entity, TimeSpan.FromHours(1));
                _appCache.Remove($"userExist:{userId}:{guildId}");
                return ServiceResponse<User>.Ok(addedEntity.Entity);
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e.Message);
                return ServiceResponse<User>.Fail(new ErrorResult("Could not add user.", new ErrorResult(e.Message)));
            }
        }

        public async Task<ServiceResponse<User>> GetUserAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default)
        {
            var userExist = await UserExistAsync(userId, guildId, cancellationToken);
            if (!userExist.Entity)
                return ServiceResponse<User>.Fail(new ErrorResult("User does not exist."));

            var user = _appCache.Get<User>($"user:{userId}:{guildId}");
            if (user is not null)
                return ServiceResponse<User>.Ok(user);

            user = _dbContext.Users.First(x => x.UserId == userId && x.GuildId == guildId);
            _appCache.Add($"user:{userId}:{guildId}", user, TimeSpan.FromHours(1));
            return ServiceResponse<User>.Ok(user);
        }

        public async Task<ServiceResponse> DeleteUserAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userExist = await UserExistAsync(userId, guildId, cancellationToken);
                if (!userExist.Entity)
                    return ServiceResponse.Fail(new ErrorResult("Can't delete a user that does not exist"));

                var user = await GetUserAsync(userId, guildId, cancellationToken);

                _dbContext.Remove(user.Entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            
                _appCache.Remove($"user:{userId}:{guildId}");
                _appCache.Remove($"userExist:{userId}:{guildId}");
                return ServiceResponse.Ok();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e.Message);
                return ServiceResponse.Fail(new ErrorResult("Could not remove user", new ErrorResult(e.Message)));
            }
        }

        public async Task<ServiceResponse<bool>> UserExistAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default)
        {
            var userExists = _appCache.Get<ServiceResponse<bool>>($"userExist:{userId}:{guildId}");
            if (userExists is not null)
                return userExists;

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.GuildId == guildId, cancellationToken);
            ServiceResponse<bool> response = null;

            response = ServiceResponse<bool>.Ok(user is not null);

            _appCache.Add($"userExist:{userId}:{guildId}", response, TimeSpan.FromHours(1));
            
            return response;
        }
    }
}
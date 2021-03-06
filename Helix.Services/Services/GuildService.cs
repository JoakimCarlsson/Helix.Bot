using System;
using System.Threading;
using System.Threading.Tasks;
using Helix.Domain.Data;
using Helix.Domain.Models;
using Helix.Services.Abstractions;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helix.Services.Services
{
    public class GuildService : IGuildService
    {
        private readonly HelixDbContext _dbContext;
        private readonly IAppCache _appCache;
        private readonly ILogger<GuildService> _logger;

        public GuildService(HelixDbContext dbContext, IAppCache appCache, ILogger<GuildService> logger)
        {
            _dbContext = dbContext;
            _appCache = appCache;
            _logger = logger;
        }

        public async Task<ServiceResponse<Guild>> AddGuildAsync(ulong guildId, CancellationToken cancellationToken = default)
        {
            try
            {
                var guildExist = await GuildExistsAsync(guildId, cancellationToken);
                if (guildExist.Entity)
                    return ServiceResponse<Guild>.Fail(new ErrorResult("Guild already exist."));

                var guild = new Guild
                {
                    Id = guildId,
                    Prefix = ">!",
                    GuildConfiguration = new GuildConfiguration(),
                };

                var addedEntity = await _dbContext.AddAsync(guild, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _appCache.Add($"Guild:{guildId}", addedEntity.Entity, TimeSpan.FromHours(5));
                
                return ServiceResponse<Guild>.Ok(addedEntity.Entity);
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e.Message);
                return ServiceResponse<Guild>.Fail(new ErrorResult("Could not add guild.", new ErrorResult(e.Message)));
            }
        }

        public async Task<ServiceResponse<bool>> GuildExistsAsync(ulong guildId, CancellationToken cancellationToken = default)
        {
            var guild = await _dbContext.Guilds.AsNoTracking().FirstOrDefaultAsync(x => x.Id == guildId, cancellationToken);
            if (guild is null)
                return ServiceResponse<bool>.Ok(false);

            return ServiceResponse<bool>.Ok(true);
        }
    }
}
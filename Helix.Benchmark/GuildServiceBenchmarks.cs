using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Helix.Domain.Data;
using Helix.Domain.Models;
using Helix.Services.Services;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helix.Benchmark
{
    [MemoryDiagnoser]
    public class GuildServiceBenchmarks
    {
        private readonly GuildService _guildService;
        private readonly IAppCache _appCache;
        private readonly ILogger<GuildService> _logger;

        public GuildServiceBenchmarks()
        {
            _appCache = new CachingService();
            _guildService = new GuildService(CreateDbContext(), _appCache, _logger);
        }

        [Benchmark]
        public async Task<ServiceResponse<Guild>> AddGuildTask()
        {
            return await _guildService.AddGuildAsync(500);
        }

        private HelixDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HelixDbContext>().UseSqlite($"Data Source=Helix-Test{Guid.NewGuid()}.db;");
            var dbContext = new HelixDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();
            return dbContext;
        }
    }
}

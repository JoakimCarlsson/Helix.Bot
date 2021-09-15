using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Helix.Domain.Data;
using Helix.Domain.Models;
using Helix.Services.Services;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace Helix.Benchmark
{
    [MemoryDiagnoser]
    public class GuildServiceBenchmarks
    {
        private readonly GuildService _guildService;

        public GuildServiceBenchmarks()
        {
            IAppCache appCache = new CachingService();
            _guildService = new GuildService(CreateDbContext(), appCache, default);
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

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Helix.Core.Services;
using Helix.Domain.Data;
using Helix.Domain.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Helix.Core.Test
{
    public class GuildServiceTests
    {
        private readonly HelixDbContext _dbContext;
        private readonly IAppCache _appCache;
        private readonly Mock<ILogger<GuildService>> _loggerMock;

        public GuildServiceTests()
        {
            _dbContext = CreateDbContext();
            _appCache = new CachingService();
            _loggerMock = new Mock<ILogger<GuildService>>();
        }
        
        [Fact]
        public async Task AddGuildShouldAddValidGuild()
        {
            //Arrange
            var sut = new GuildService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = await sut.AddGuild(500);
            
            //Assert
            actual.Entity.Should().NotBeNull();
            actual.Entity.GuildConfiguration.Should().NotBeNull();
            actual.Entity.Prefix.Should().Be(">!"); //default prefix 
        }

        [Fact]
        public async Task AddGuildShouldReturnFailIfGuildExists()
        {
            //Arrange
            var sut = new GuildService(_dbContext, _appCache, _loggerMock.Object);
            await sut.AddGuild(500);
            
            //Act
            var actual = await sut.AddGuild(500);
            
            //Assert
            actual.Success.Should().Be(false);
            actual.Errors.ErrorMessage.Should().Be("Guild already exist.");
        }

        [Fact]
        public async Task AddInvalidGuildShouldReturnFail()
        {
            //Arrange
            var sut = new GuildService(_dbContext, _appCache, _loggerMock.Object);
            //Act
            var actual = await sut.AddGuild(0);
            
            //Assert
            actual.Success.Should().Be(false);
            actual.Errors.ErrorMessage.Should().Be("Could not add guild.");
        }
        
        [Fact]
        public async Task AddGuildShouldAddGuildToCache()
        {
            //Arrange
            var sut = new GuildService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = await sut.AddGuild(500);
            var expected = _appCache.Get<Guild>($"Guild:{actual.Entity.Id}");
            //Assert

            expected.Should().Be(actual.Entity);
        }

        [Fact]
        public async Task GuildExistShouldReturnFalseIfGuildDontExist()
        {
            //Arrange
            var sut = new GuildService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = await sut.GuildExists(500);
            
            //Assert
            actual.Entity.Should().Be(false);
        }

        [Fact]
        public async Task GuildExistShouldReturnTrueIfGuildExists()
        {
            //Arrange
            var sut = new GuildService(_dbContext, _appCache, _loggerMock.Object);
            await sut.AddGuild(500);
            //Act
            var actual = await sut.GuildExists(500);
            
            //Assert
            actual.Entity.Should().Be(true);
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
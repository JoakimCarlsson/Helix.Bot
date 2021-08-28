using System;
using System.Threading.Tasks;
using Bogus.DataSets;
using FluentAssertions;
using Helix.Core.Response;
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
    public class UserServiceTests
    {
        private readonly HelixDbContext _dbContext;
        private readonly IAppCache _appCache;
        private readonly Mock<ILogger<UserService>> _loggerMock;

        public UserServiceTests()
        {
            _dbContext = CreateDbContext();
            _appCache = new CachingService();
            _loggerMock = new Mock<ILogger<UserService>>();
        }

        [Fact]
        public async Task GetUserShouldReturnUser()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = await sut.GetUser(600, 500);
            
            //Assert
            actual.Entity.Should().NotBeNull();
            actual.Success.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetUserShouldReturnFailIfUserDoesNotExist()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = await sut.GetUser(1, 500);
            
            //Assert
            actual.Success.Should().BeFalse();
            actual.Entity.Should().BeNull();
            actual.Errors.ErrorMessage.Should().Be("User does not exist.");
        }
        
        [Fact]
        public async Task GetUserShouldCacheUser()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = _appCache.Get<User>($"user:{600}:{500}");
            var user = await sut.GetUser(600, 500);
            var expected = _appCache.Get<User>($"user:{600}:{500}");
            
            //Assert
            actual.Should().BeNull();
            expected.Should().NotBeNull();
            expected.Id.Should().Be(user.Entity.Id);
        }
        
        [Fact]
        public async Task AddUserShouldAddUser()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var expected = await sut.AddUser(1, 500, DateTime.Now);
            
            //Assert
            expected.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteUserShouldDeleteUser()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var expected = await sut.DeleteUser(600, 500);
            
            //Assert
            expected.Success.Should().BeTrue();
        }
        
        [Fact]
        public async Task DeleteUserShouldReturnFailIfUserDoesNotExist()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var expected = await sut.DeleteUser(69, 500);
            
            //Assert
            expected.Success.Should().BeFalse();
            expected.Errors.ErrorMessage.Should().Be("Can't delete a user that does not exist");
        }

        [Fact]
        public async Task DeleteUserShouldClearCache()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            await sut.DeleteUser(1337, 500);
            
            var expectedUserCache = _appCache.Get<User>($"user:{1337}:{500}");
            var expectedUserExistCache = _appCache.Get<User>($"userExist:{1337}:{500}");
            
            //Assert
            expectedUserCache.Should().BeNull();
            expectedUserExistCache.Should().BeNull();
        }
        
        [Fact]
        public async Task DeleteUserShouldReturnFailIfInvalidUser()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = await sut.DeleteUser(600, 200);
            
            //Assert
            actual.Success.Should().BeFalse();
            actual.Errors.ErrorMessage.Should().Be("Can't delete a user that does not exist");
        }
        
        [Fact]
        public async Task AddUserShouldReturnFailIfInvalidUser()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var expected = await sut.AddUser(0, 0, DateTime.Now);
            
            //Assert
            expected.Success.Should().BeFalse();
            expected.Errors.ErrorMessage.Should().Be("Could not add user.");
        }
        
        [Fact]
        public async Task AddUserShouldReturnFailIfUserExists()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var expected = await sut.AddUser(600, 500, DateTime.Now);
            
            //Assert
            expected.Success.Should().BeFalse();
            expected.Errors.ErrorMessage.Should().Be("User already exist.");
        }
        
        [Fact]
        public async Task AddUserShouldAddToCache()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            
            //Act
            var actual = _appCache.Get<User>($"user:{200}:{500}");
            await sut.AddUser(200, 500, DateTime.Now);
            var expected = _appCache.Get<User>($"user:{200}:{500}");

            //Assert
            actual.Should().BeNull();
            expected.Should().NotBeNull();
        }
        
        [Fact]
        public async Task UserExistShouldReturnTrueIfExist()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            //Act
            var actual = await sut.UserExist(600, 500);
            
            //Assert
            actual.Success.Should().BeTrue();
            actual.Entity.Should().BeTrue();
        }
        
        [Fact]
        public async Task UserExistShouldReturnFalseIfUserDoesNotExist()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
        
            //Act
            var actual = await sut.UserExist(1, 2);
            
            //Assert
            actual.Success.Should().BeTrue();
            actual.Entity.Should().BeFalse();
        }
        
        [Fact]
        public async Task UserExistShouldCacheResult()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);

            //Act
            var actual = _appCache.Get<ServiceResponse<bool>>($"userExist:{1}:{1}");
            await sut.UserExist(1, 1);
            var expected = _appCache.Get<ServiceResponse<bool>>($"userExist:{1}:{1}");
            
            //Assert
            actual.Should().BeNull();
            expected.Should().NotBeNull();
        }
        
        [Fact]
        public async Task UserExistShouldReturnFromCacheIfNotNull()
        {
            //Arrange
            var sut = new UserService(_dbContext, _appCache, _loggerMock.Object);
            var foo = ServiceResponse<bool>.Ok(true);
            _appCache.Add($"userExist:{1}:{1}", foo);
            
            //Act
            var actual = await sut.UserExist(1, 1);
            //Assert
            actual.Success.Should().BeTrue();
        }
        
        public HelixDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HelixDbContext>().UseSqlite($"Data Source=Helix-Test{Guid.NewGuid()}.db;");
            var dbContext = new HelixDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();

            var guild = new Guild
            {
                Id = 500,
                Prefix = ">!",
                GuildConfiguration = new GuildConfiguration(),
            };

            var user = new User
            {
                UserId = 600,
                GuildId = 500,
                FirstSeen = DateTime.Now,
            };
            var userOne = new User
            {
                UserId = 1337,
                GuildId = 500,
                FirstSeen = DateTime.Now,
            };

            dbContext.Add(guild);
            dbContext.Add(user);
            dbContext.Add(userOne);
            dbContext.SaveChanges();
            
            return dbContext;
        }
    }
}
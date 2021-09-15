using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helix.Domain.Data;
using Helix.Domain.Models;
using Helix.Services.Abstractions;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace Helix.Services.Services
{
    public class UserReminderService : IUserReminderService
    {
        private readonly HelixDbContext _dbContext;
        private readonly IAppCache _appCache;

        public UserReminderService(HelixDbContext dbContext, IAppCache appCache)
        {
            _dbContext = dbContext;
            _appCache = appCache;
        }

        public async Task<ServiceResponse<UserReminder>> GetReminderAsync(int id, CancellationToken cancellationToken = default)
        {
            var userReminder = _appCache.Get<UserReminder>($"UserReminder:{id}");
            if (userReminder is not null)
                return ServiceResponse<UserReminder>.Ok(userReminder);

            userReminder = await _dbContext.Reminders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (userReminder is null)
                return ServiceResponse<UserReminder>.Fail(new ErrorResult($"Reminder with following ID: {id} does not exist."));

            _appCache.Add($"UserReminder:{id}", userReminder, TimeSpan.FromHours(5));
            return ServiceResponse<UserReminder>.Ok(userReminder);
        }

        public async Task<ServiceResponse<UserReminder>> GetUserReminderAsync(int id, ulong userId, ulong guildId, CancellationToken cancellationToken = default)
        {
            var userReminder = await GetReminderAsync(id, cancellationToken);
            if (!userReminder.Success)
                return ServiceResponse<UserReminder>.Fail(userReminder.Errors);

            if (userReminder.Entity.UserId == userId && userReminder.Entity.GuildId != guildId)
                return ServiceResponse<UserReminder>.Fail(new ErrorResult("User has not set this reminder in this guild."));

            if (userReminder.Entity.UserId != userId && userReminder.Entity.GuildId == guildId)
                return ServiceResponse<UserReminder>.Fail(new ErrorResult("This reminder does not belong to this user."));

            if (userReminder.Entity.UserId == userId && userReminder.Entity.GuildId == guildId)
                return ServiceResponse<UserReminder>.Ok(userReminder.Entity);

            return ServiceResponse<UserReminder>.Fail(new ErrorResult("Wat"));
        }

        public async Task<ServiceResponse<IEnumerable<UserReminder>>> GetAllUserRemindersAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default)
        {
            var reminders = await GetAllRemindersAsync(cancellationToken);
            if (reminders.Success)
            {
                var userReminders = reminders.Entity.Where(x => x.UserId == userId && x.GuildId == guildId);
                if (!userReminders.Any())
                    return ServiceResponse<IEnumerable<UserReminder>>.Fail(new ErrorResult("You do not have any reminders set in this guild."));

                return ServiceResponse<IEnumerable<UserReminder>>.Ok(userReminders);
            }

            return ServiceResponse<IEnumerable<UserReminder>>.Fail(reminders.Errors);
        }

        public async Task<ServiceResponse<IEnumerable<UserReminder>>> GetAllRemindersAsync(CancellationToken cancellationToken = default)
        {
            var userReminders = await _dbContext.Reminders.ToListAsync(cancellationToken);
            if (!userReminders.Any())
                return ServiceResponse<IEnumerable<UserReminder>>.Fail(new ErrorResult("No reminders saved in the DB"));

            return ServiceResponse<IEnumerable<UserReminder>>.Ok(userReminders);
        }

        public async Task<ServiceResponse<IEnumerable<UserReminder>>> GetAllRemindersInGuildAsync(ulong guildId, CancellationToken cancellationToken = default)
        {
            var reminders = await GetAllRemindersAsync(cancellationToken);
            if (reminders.Success)
            {
                var guildReminders = reminders.Entity.Where(x => x.GuildId == guildId);
                if (!guildReminders.Any())
                    return ServiceResponse<IEnumerable<UserReminder>>.Fail(new ErrorResult("No reminders have been set in this guild."));

                return ServiceResponse<IEnumerable<UserReminder>>.Ok(guildReminders);
            }

            return ServiceResponse<IEnumerable<UserReminder>>.Fail(reminders.Errors);
        }

        public async Task<ServiceResponse<UserReminder>> AddUserReminderAsync(ulong userId, ulong guildId, ulong channelId, string content, TimeSpan remindAt, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                    return ServiceResponse<UserReminder>.Fail(new ErrorResult("Reminder message can't be empty."));

                var userReminder = new UserReminder
                {
                    Content = content,
                    CreatedAt = DateTime.Now,
                    RemindAt = DateTime.Now.Add(remindAt),
                    ChannelId = channelId,
                    UserId = userId,
                    GuildId = guildId
                };

                var addedEntity = await _dbContext.AddAsync(userReminder, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _appCache.Add($"UserReminder:{addedEntity.Entity.Id}", addedEntity.Entity, TimeSpan.FromHours(5));
                return ServiceResponse<UserReminder>.Ok(addedEntity.Entity);
            }
            catch (DbUpdateException e)
            {
                ErrorResult errorResult = new ErrorResult(e.Message);
                return ServiceResponse<UserReminder>.Fail(errorResult);
            }
        }

        public async Task<ServiceResponse> DeleteUserReminderAsync(ulong userId, ulong guildId, int reminderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reminder = await _dbContext.Reminders.FirstOrDefaultAsync(x => x.UserId == userId && x.GuildId == guildId && x.Id == reminderId, cancellationToken);
                if (reminder is null)
                    return ServiceResponse.Fail(new ErrorResult("This reminder does not exist, or does not belong to this user"));

                _dbContext.Reminders.Remove(reminder);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _appCache.Remove($"UserReminder:{reminderId}");
                return ServiceResponse.Ok();
            }
            catch (DbUpdateException e)
            {
                ErrorResult errorResult = new ErrorResult(e.Message);
                return ServiceResponse.Fail(errorResult);
            }
        }
    }
}

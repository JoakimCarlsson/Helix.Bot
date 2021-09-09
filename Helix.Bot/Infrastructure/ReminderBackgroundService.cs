using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helix.Bot.Helpers.Formatters;
using Helix.Services.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Core;

namespace Helix.Bot.Infrastructure
{
    class ReminderBackgroundService : BackgroundService
    {
        private readonly IUserReminderService _reminderService;
        private readonly IDiscordRestChannelAPI _discordRestChannelApi;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(IUserReminderService reminderService, IDiscordRestChannelAPI discordRestChannelApi, ILogger<ReminderBackgroundService> logger)
        {
            _reminderService = reminderService;
            _discordRestChannelApi = discordRestChannelApi;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);

                var reminders = await _reminderService.GetAllRemindersAsync(stoppingToken);
                if (reminders.Success && reminders.Entity.Any())
                {
                    foreach (var reminder in reminders.Entity)
                    {
                        if ((reminder.RemindAt - DateTime.Now) < TimeSpan.FromMinutes(2))
                        {
                            _logger.LogInformation("Sending reminder Id:{id}", reminder.Id);
                            await _discordRestChannelApi.CreateMessageAsync(new Snowflake(reminder.ChannelId), $"Hey! {MentionFormatter.User(new Snowflake(reminder.UserId))} :partying_face:\nYou wanted me too remind you of: {TextFormatter.Bold(reminder.Content)}", ct: stoppingToken);
                            await _reminderService.DeleteUserReminderAsync(reminder.UserId, reminder.GuildId, reminder.Id, stoppingToken);
                        }
                    }
                }
            }
        }
    }
}

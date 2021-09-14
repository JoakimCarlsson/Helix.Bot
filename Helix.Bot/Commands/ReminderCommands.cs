using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Helix.Bot.Helpers.Abstractions;
using Helix.Bot.Helpers.Formatters;
using Helix.Services.Abstractions;
using Humanizer;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Core;
using Remora.Results;

namespace Helix.Bot.Commands
{
    [Group("remind")]
    [RequireContext(ChannelContext.Guild)]
    public class ReminderCommands : CommandGroup
    {
        private readonly IUserReminderService _userReminderService;
        private readonly ICommandContext _commandContext;
        private readonly IRespondService _respondService;

        public ReminderCommands(IUserReminderService userReminderService, ICommandContext commandContext, IRespondService respondService)
        {
            _userReminderService = userReminderService;
            _commandContext = commandContext;
            _respondService = respondService;
        }

        [Command("me"), Description("Adds a reminder for the using user.")]
        public async Task<IResult> RemindMeAsync([Description("When you want want to be reminded.")] TimeSpan remindAt, [Description("The reminder message.")][Greedy] string message)
        {
            var serviceResponse = await _userReminderService.AddUserReminderAsync(_commandContext.User.ID.Value, _commandContext.GuildID.Value.Value, _commandContext.ChannelID.Value, message, remindAt, CancellationToken);
            Result<IMessage> result;

            if (serviceResponse.Success)
                result = await _respondService.RespondWithSuccessEmbedAsync($"Great, i'll remind you in: {DateFormatter.RelativeTime(DateTime.Now.Add(remindAt))}");
            else
                result = await _respondService.RespondWithErrorEmbedAsync(serviceResponse.Errors.ErrorMessage);

            return result.IsSuccess ? Result.FromSuccess() : Result.FromError(result.Error);
        }

        [Command("get"), Description("Get's an reminder by id.")]
        public async Task<IResult> GetReminderAsync([Description("The ID of the reminder you want to get.")] int id)
        {
            var reminderResponse = await _userReminderService.GetReminderAsync(id, CancellationToken);

            if (!reminderResponse.Success)
                return await _respondService.RespondWithErrorEmbedAsync(reminderResponse.Errors.ErrorMessage, CancellationToken);

            var author = new EmbedAuthor($"Reminder [{reminderResponse.Entity.Id}]");

            var fields = new List<EmbedField>
            {
                new EmbedField("Created At", DateFormatter.RelativeTime(reminderResponse.Entity.CreatedAt), true),
                new EmbedField("Remind At", DateFormatter.RelativeTime(reminderResponse.Entity.RemindAt), true)
            };

            var embed = new Embed
            {
                Author = author,
                Description = $"{TextFormatter.Bold("Reminder Content:")}\n{reminderResponse.Entity.Content}",
                Fields = fields,
            };

            var result = await _respondService.RespondWithEmbedAsync(embed, CancellationToken);

            return result.IsSuccess ? Result.FromSuccess() : Result.FromError(result.Error);
        }


        [Command("list"), Description("List all reminder for the using user.")]
        public async Task<IResult> GetAllUserRemindersAsync()
        {
            var serviceResponse = await _userReminderService.GetAllUserRemindersAsync(_commandContext.User.ID.Value, _commandContext.GuildID.Value.Value, CancellationToken);

            Result<IMessage> result;

            if (serviceResponse.Success)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (var reminder in serviceResponse.Entity)
                {
                    stringBuilder.AppendLine($"[{reminder.Id}] {reminder.Content.Truncate(10, "...")} {DateFormatter.RelativeTime(reminder.RemindAt)}");
                }

                var reminderEmbed = new Embed
                {
                    Title = "Reminders",
                    Description = stringBuilder.ToString()
                };
                result = await _respondService.RespondWithEmbedAsync(reminderEmbed, CancellationToken);
            }
            else
            {
                result = await _respondService.RespondWithErrorEmbedAsync(serviceResponse.Errors.ErrorMessage, CancellationToken);
            }
            return result.IsSuccess ? Result.FromSuccess() : Result.FromError(result.Error);
        }

        [Command("list-all"), Description("List all the minders that have been set in the guild")]
        [RequireDiscordPermission(DiscordPermission.Administrator)]
        public async Task<IResult> GetAllUserRemindersInGuildAsync()
        {
            var serviceResponse = await _userReminderService.GetAllRemindersInGuildAsync(_commandContext.GuildID.Value.Value, CancellationToken);

            Result<IMessage> result;

            if (serviceResponse.Success)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (var reminder in serviceResponse.Entity)
                {
                    stringBuilder.AppendLine($"[{reminder.Id}] UserId: {reminder.UserId}, {reminder.Content.Truncate(10, "...")} {DateFormatter.RelativeTime(reminder.RemindAt)}");
                }

                var reminderEmbed = new Embed
                {
                    Title = "Reminders in guild.",
                    Description = stringBuilder.ToString()
                };
                result = await _respondService.RespondWithEmbedAsync(reminderEmbed, CancellationToken);
            }
            else
            {
                result = await _respondService.RespondWithErrorEmbedAsync(serviceResponse.Errors.ErrorMessage, CancellationToken);
            }
            return result.IsSuccess ? Result.FromSuccess() : Result.FromError(result.Error);
        }

        [Command("delete"), Description("Deletes the reminder of the using user.")]
        public async Task<IResult> DeleteReminderAsync([Description("Reminder Id")] int id)
        {
            IResult result;
            var serviceResponse = await _userReminderService.DeleteUserReminderAsync(_commandContext.User.ID.Value, _commandContext.GuildID.Value.Value, id);

            if (serviceResponse.Success)
                result = await _respondService.RespondWithSuccessEmbedAsync($"Deleted reminder with ID: {id}");
            else
                result = await _respondService.RespondWithErrorEmbedAsync(serviceResponse.Errors.ErrorMessage);

            return result.IsSuccess ? Result.FromSuccess() : Result.FromError(result.Error);
        }
    }
}

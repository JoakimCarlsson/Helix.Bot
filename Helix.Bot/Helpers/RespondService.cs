using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.Bot.Helpers.Abstractions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Core;
using Remora.Results;

namespace Helix.Bot.Helpers
{
    public class RespondService : IRespondService
    {
        private readonly ICommandContext _commandContext;
        private readonly IDiscordRestChannelAPI _channelApi;
        private readonly IDiscordRestInteractionAPI _interactionApi;

        public RespondService(ICommandContext commandContext, IDiscordRestChannelAPI channelApi, IDiscordRestInteractionAPI interactionApi)
        {
            _commandContext = commandContext;
            _channelApi = channelApi;
            _interactionApi = interactionApi;
        }

        public async Task<Result<IMessage>> RespondWithSuccessEmbedAsync(string content, CancellationToken cancellationToken = default) => await RespondWithEmbedAsync(GetUserSuccessEmbed(content), cancellationToken);
        public async Task<Result<IMessage>> RespondWithErrorEmbedAsync(string content, CancellationToken cancellationToken = default) => await RespondWithEmbedAsync(GetUserErrorEmbed(content), cancellationToken);
        public async Task<Result<IMessage>> RespondWithEmbedAsync(IEmbed embed, CancellationToken cancellationToken = default)
        {
            if (_commandContext is InteractionContext interactionContext)
                return await RespondToInteraction(interactionContext, default, new List<IEmbed> { embed }, cancellationToken);

            return await RespondToMessageAsync(default, new List<IEmbed> { embed }, cancellationToken);
        }

        public async Task<Result<IMessage>> RespondWithMessageAsync(Optional<string> content = default, Optional<IReadOnlyList<IEmbed>> embeds = default, CancellationToken cancellationToken = default)
        {
            if (_commandContext is InteractionContext interactionContext)
                return await RespondToInteraction(interactionContext, content, embeds, cancellationToken);

            return await RespondToMessageAsync(content, embeds, cancellationToken);
        }

        public async Task<Result<IMessage>> WriteMessageAsync(Snowflake channelId, Optional<string> content = default, Optional<IReadOnlyList<IEmbed>> embeds = default, CancellationToken cancellationToken = default)
        {
            return await _channelApi.CreateMessageAsync(channelId, content, embeds: embeds, ct: cancellationToken);
        }

        private async Task<Result<IMessage>> RespondToMessageAsync(Optional<string> content = default, Optional<IReadOnlyList<IEmbed>> embeds = default, CancellationToken cancellationToken = default)
        {
            return await _channelApi.CreateMessageAsync(_commandContext.ChannelID, content, embeds: embeds, ct: cancellationToken);
        }

        private async Task<Result<IMessage>> RespondToInteraction(InteractionContext interactionContext, Optional<string> content = default, Optional<IReadOnlyList<IEmbed>> embeds = default, CancellationToken cancellationToken = default)
        {
            var interactionApplicationCommand = new InteractionCallbackData(Content: content, Embeds: embeds);
            var interactionResponse = new InteractionResponse(InteractionCallbackType.ChannelMessageWithSource, interactionApplicationCommand);
            await _interactionApi.CreateInteractionResponseAsync(interactionContext.ID, interactionContext.Token, interactionResponse, cancellationToken);

            return Result<IMessage>.FromSuccess(null);
            //return await _interactionApi.CreateFollowupMessageAsync(interactionContext.ApplicationID, interactionContext.Token, content, embeds: embeds, ct: cancellationToken); Should work, but it does not I guess?
        }

        public IEmbed GetUserErrorEmbed(string content) => new Embed
        {
            Colour = Color.FromArgb(233, 40, 40),
            Description = content
        };

        public IEmbed GetUserSuccessEmbed(string content) => new Embed
        {
            Colour = Color.FromArgb(82, 234, 33),
            Description = content
        };
    }
}

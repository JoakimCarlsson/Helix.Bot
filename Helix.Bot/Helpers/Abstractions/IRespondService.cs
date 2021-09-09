using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Core;
using Remora.Results;

namespace Helix.Bot.Helpers.Abstractions
{
    public interface IRespondService
    {
        Task<Result<IMessage>> RespondWithSuccessEmbedAsync(string content, CancellationToken cancellationToken = default);
        Task<Result<IMessage>> RespondWithErrorEmbedAsync(string content, CancellationToken cancellationToken = default);
        Task<Result<IMessage>> RespondWithEmbedAsync(IEmbed embed, CancellationToken cancellationToken = default);
        Task<Result<IMessage>> RespondWithMessageAsync(Optional<string> content = default, Optional<IReadOnlyList<IEmbed>> embeds = default, CancellationToken cancellationToken = default);
        Task<Result<IMessage>> WriteMessageAsync(Snowflake channelId, Optional<string> content = default, Optional<IReadOnlyList<IEmbed>> embeds = default, CancellationToken cancellationToken = default);
    }
}
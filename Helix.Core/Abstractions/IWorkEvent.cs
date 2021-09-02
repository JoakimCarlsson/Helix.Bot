using System;

namespace Helix.Core.Abstractions
{
   public interface IWorkEvent{ }

   public sealed record AddGuildEvent(ulong GuildId) : IWorkEvent;

   public sealed record AddGuildMemberEvent(ulong UserId, ulong GuildId, DateTime FirstSeen) : IWorkEvent;

   public sealed record RemoveGuildMemberEvent(ulong UserId, ulong GuildId) : IWorkEvent;
}

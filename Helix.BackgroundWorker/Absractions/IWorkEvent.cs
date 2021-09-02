using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.BackgroundWorker.Absractions
{
   public interface IWorkEvent{ }

   public sealed record AddGuildEvent(ulong GuildId) : IWorkEvent;
}

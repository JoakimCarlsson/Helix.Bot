using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Domain.Models
{
    public class UserReminder
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RemindAt { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
    }
}

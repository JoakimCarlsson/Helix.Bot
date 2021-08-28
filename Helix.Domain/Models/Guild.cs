using System.Collections.Generic;

namespace Helix.Domain.Models
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public GuildConfiguration GuildConfiguration { get; set; }
        public ICollection<User> Users { get; set; }
    }
}

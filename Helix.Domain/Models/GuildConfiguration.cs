namespace Helix.Domain.Models
{
    public class GuildConfiguration
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public Guild Guild { get; set; }
    }
}
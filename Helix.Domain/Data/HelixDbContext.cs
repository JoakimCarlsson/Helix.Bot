using Helix.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Helix.Domain.Data
{
    public class HelixDbContext : DbContext
    {
        public HelixDbContext(DbContextOptions<HelixDbContext> options) : base(options) { }

        public DbSet<Guild> Guilds { get; set; }
        public DbSet<GuildConfiguration> GuildConfigurations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserReminder> Reminders {  get; set; }
    }
}
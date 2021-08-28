using Microsoft.EntityFrameworkCore;

namespace Helix.Domain.Data
{
    public class HelixDbContext : DbContext
    {
        public HelixDbContext(DbContextOptions<HelixDbContext> options) : base(options) { }
    }
}
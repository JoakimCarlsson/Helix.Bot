using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Helix.Domain.Data
{
    class HelixContextFactory : IDesignTimeDbContextFactory<HelixDbContext>
    {
        public HelixDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<HelixDbContext>()
                .Build();

            var options = new DbContextOptionsBuilder<HelixDbContext>();
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            return new HelixDbContext(options.Options);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helix.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helix.Domain.Configurations
{
    class GuildConfig : IEntityTypeConfiguration<Guild>
    {
        public void Configure(EntityTypeBuilder<Guild> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.HasMany(x => x.Users)
                .WithOne(x => x.Guild);

            builder.HasOne(x => x.GuildConfiguration)
                .WithOne(x => x.Guild)
                .HasForeignKey<GuildConfiguration>(x => x.GuildId);
        }
    }
}

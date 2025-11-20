using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabAI_API.Domain.Entites;

namespace VocabAI_API.Persistence.EntityConfigurations
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            builder.HasMany(l => l.Words)
                   .WithOne(w => w.Level)
                   .HasForeignKey(w => w.LevelId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Level { Id = 1, Name = "A1" },
                new Level { Id = 2, Name = "A2" },
                new Level { Id = 3, Name = "B1" },
                new Level { Id = 4, Name = "B2" }
            );
        }
    }
}

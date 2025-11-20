using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabAI_API.Domain.Entites;

namespace VocabAI_API.Persistence.Context
{
    public class VocabAIDBContext : DbContext
    {
        public DbSet<Level> Levels { get; set; }
        public DbSet<Word> Words { get; set; }
        public VocabAIDBContext(DbContextOptions options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using UTB.Data.Models;

namespace UTB.Data
{
    public class UtbContext: DbContext
    {
        public UtbContext(DbContextOptions<UtbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Station>()
                .HasMany(x => x.Samples)
                .WithOne(x => x.Station)
                .HasForeignKey(x => x.StationId);
        }

        public DbSet<Station> Stations { get; set; }
        
        public DbSet<Sample> Samples { get; set; }
    }
}
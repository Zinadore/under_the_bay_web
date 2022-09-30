using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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

        public async Task<bool> CheckIfDatabaseReady() 
        {
            try {
                return await Database.GetService<IRelationalDatabaseCreator>().ExistsAsync();
            } 
            catch (Npgsql.PostgresException e) {
                e.Message.Contains("starting up");
                return false;
            }
        }
    }
}
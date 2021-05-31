using Microsoft.EntityFrameworkCore;
using Under_the_Bay.Data.Models;

namespace Under_the_Bay.Data
{
    public class UtbContext: DbContext
    {
        public UtbContext(DbContextOptions<UtbContext> options): base(options)
        {
            
        }
        public DbSet<Station> Stations { get; set; }
    }
}
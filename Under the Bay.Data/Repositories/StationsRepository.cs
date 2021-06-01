using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Under_the_Bay.Data.Models;

namespace Under_the_Bay.Data.Repositories
{
    public class StationsRepository: IStationsRepository
    {
        private readonly UtbContext _context;

        public StationsRepository(UtbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Station>> GetAll()
        {
            return await _context.Stations.ToListAsync();
        }

        public async Task<Station> GetById(Guid id, bool includeMeasurements, DateTimeOffset? startTime,
            DateTimeOffset? endTime)
        {
            var query = _context.Stations.Where(s => s.Id == id);

            if (includeMeasurements)
            {
                query = query.Include(x => x.Samples
                    .Where(s => s.SampleDate >= startTime && s.SampleDate <= endTime)
                    .OrderByDescending(s=> s.SampleDate)
                );
            }
            
            return await query.SingleOrDefaultAsync();
        }

        public async Task<Station> GetById(Guid id)
        {
            // var query = _context.Stations.AsQueryable();
            return await _context.Stations.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> SaveChanges()
        {
            var numEntitiesSaved = await _context.SaveChangesAsync();
            return numEntitiesSaved != 0;
        }
    }
}
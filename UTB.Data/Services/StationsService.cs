using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using UTB.Contracts.DTO;
using UTB.Data.Models;

namespace UTB.Data.Services
{
    internal class StationsService : IStationsService
    {
        private readonly UtbContext _context;
        private readonly IMapper mapper;

        public StationsService(UtbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        public async ValueTask AddAsync(StationDTO stationDTO, CancellationToken cancellationToken = default)
        {
            var station = new Station()
            {
                Name = stationDTO.Name,
                Layer = stationDTO.Layer,
                ThreeLetterId = stationDTO.ThreeLetterId,
                StationId = stationDTO.StationId
            };
            await _context.AddAsync(station, cancellationToken);
        }

        public async Task<IEnumerable<StationDTO>> GetAll()
        {
            var stations = await _context.Stations.ToListAsync();

            return mapper.Map<List<StationDTO>>(stations);
        }

        public async Task<StationDTO> GetById(Guid id, bool includeMeasurements, DateTimeOffset? startTime,
            DateTimeOffset? endTime)
        {
            var query = _context.Stations.Where(s => s.Id == id);

            if (includeMeasurements && startTime.HasValue && endTime.HasValue)
            {
                var a = Instant.FromDateTimeOffset(startTime.Value);
                var b = Instant.FromDateTimeOffset(endTime.Value);
                query = query.Include(x => x.Samples
                    .Where(s => s.SampleDate >= a && s.SampleDate <= b)
                    .OrderByDescending(s => s.SampleDate)
                    .Take(100)
                );
            }

            var result = await query.SingleOrDefaultAsync();

            return mapper.Map<StationDTO>(result);
        }

        public async Task<StationDTO> GetById(Guid id)
        {
            // var query = _context.Stations.AsQueryable();
            return await _context.Stations.Where(s => s.Id == id).ProjectToSingleOrDefaultAsync<StationDTO>();
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task UpdateStation(StationDTO stationDTO, CancellationToken cancellationToken = default)
        {
            var station = await _context.Stations.Where(s => s.Id == stationDTO.Id).FirstOrDefaultAsync();

            if (station == null)
                return;

            station.LastUpdate = stationDTO.LastUpdate;
            _context.Update(station);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using UTB.Contracts.DTO;
using UTB.Data.Models;

namespace UTB.Data.Services
{
    public interface IStationsService
    {
        public Task<IEnumerable<StationDTO>> GetAll();

        public Task<StationDTO> GetById(Guid id, bool includeMeasurements = false, DateTimeOffset? startTime = null,
            DateTimeOffset? endTime = null);

        public ValueTask AddAsync(StationDTO stationDTO, CancellationToken cancellationToken = default);

        public Task<int> SaveChanges();
    }
}
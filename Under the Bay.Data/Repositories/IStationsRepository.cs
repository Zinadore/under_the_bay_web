using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using Under_the_Bay.Data.Models;

namespace Under_the_Bay.Data.Repositories
{
    public interface IStationsRepository
    {
        public Task<IEnumerable<Station>> GetAll();

        public Task<Station> GetById(Guid id, bool includeMeasurements = false, DateTimeOffset? startTime = null,
            DateTimeOffset? endTime = null);

        public Task<bool> SaveChanges();
    }
}
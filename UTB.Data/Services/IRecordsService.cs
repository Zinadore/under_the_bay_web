using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTB.Contracts.DTO;

namespace UTB.Data.Services
{
    public interface ISamplesService
    {
        ValueTask AddAsync(SampleFromCSV dto, CancellationToken cancellationToken = default);
        Task AddAsync(SampleFromCSV csvRecord, Instant instant, Guid stationId, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

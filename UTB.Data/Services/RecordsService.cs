using AutoMapper;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTB.Contracts.DTO;
using UTB.Data.Models;

namespace UTB.Data.Services
{
    internal class SamplesService: ISamplesService
    {
        private readonly UtbContext context;
        private readonly IMapper mapper;

        public SamplesService(UtbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        public async ValueTask AddAsync(SampleFromCSV dto, CancellationToken cancellationToken = default)
        {
            var sample = mapper.Map<Sample>(dto);

            await this.context.Samples.AddAsync(sample, cancellationToken);
        }

        public async Task AddAsync(SampleFromCSV csvRecord, Instant instant, Guid stationId, CancellationToken cancellationToken = default)
        {
            var sample = MapCSVToSample(csvRecord);
            sample.SampleDate = instant;
            sample.StationId = stationId;

            await this.context.Samples.AddAsync(sample, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.context.SaveChangesAsync(cancellationToken);
        }

        private Sample MapCSVToSample(SampleFromCSV record)
        {
            var sample = new Sample();
            sample.SampleDepth = ClampProperty(record.SampleDepth_m, 0, 200);
            sample.WaterTemperature= ClampProperty(record.Temp_C, 0, 100);
            sample.DissolvedOxygen = ClampProperty(record.DO_mgL, 0, 21);
            sample.DissolvedOxygenSaturation= ClampProperty(record.DO_sat, 0, 1);
            sample.Salinity = ClampProperty(record.Salinity_ppt, 0, 32);
            sample.pH = ClampProperty(record.pH, 0, 14);
            sample.Turbidity = ClampProperty(record.Turbidity_NTU, 0, 100);
            sample.Chlorophyll = ClampProperty(record.ChlA_ugL, 0, 100);
            sample.BlueGreenAlgae = ClampProperty(record.BGA_RFU, 0, 100);

            return sample;
        }

        public static float ClampProperty(float? source, float min, float max)
        {
            return source != null ? Math.Clamp(source.Value, min, max) : min;
        }
    }
}

using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace UTB.Contracts.DTO
{
    public class StationDTO
    {
        public Guid Id { get; set; }
        public string ThreeLetterId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public string Layer { get; set; } = string.Empty;
        public Instant LastUpdate { get; set; }

        public IList<SampleDTO> Samples { get; set; } = new List<SampleDTO>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace UTB.Data.Models
{
    public class Station
    {
        public Guid Id { get; set; }
        [Required]
        public string ThreeLetterId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string StationId { get; set; }
        [Required]
        public string Layer { get; set; }
        [Required] 
        public Instant LastUpdate { get; set; }
        
        public virtual IEnumerable<Sample> Samples { get; set; }
    }
}
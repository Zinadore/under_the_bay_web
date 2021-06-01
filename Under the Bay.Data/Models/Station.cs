using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Under_the_Bay.Data.Models
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
        public DateTimeOffset LastUpdate { get; set; }
        
        public virtual IEnumerable<Sample> Samples { get; set; }
    }
}
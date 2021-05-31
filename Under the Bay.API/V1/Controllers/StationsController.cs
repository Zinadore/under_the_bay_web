using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Under_the_Bay.Data.Models;

namespace Under_the_Bay.API.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StationsController : ControllerBase
    {
        private static List<Station> _stations = new List<Station>{
            new Station  { Id = Guid.Parse("46820eb0-5d7e-45b9-900b-0dbc92237f09"), StationId = "XIE7136", Layer = "S", Name = "AquariumEast" },
            new Station  { Id = Guid.Parse("a461b5a1-b587-48d9-aa9c-b31b21cc197d"), StationId = "XIE7136", Layer = "B", Name = "AquariumEastBottom" }
        };
        public StationsController()
        {
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Station>), 200)]
        public ActionResult<List<string>> GetAll()
        {
            return Ok(_stations);
        }

        [HttpGet("{id}")]
        public ActionResult<Station> Get(Guid id)
        {
            var station = _stations.FirstOrDefault(s => s.Id == id);

            if (station == null)
            {
                return NotFound();
            }

            return Ok(station);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Under_the_Bay.API.V1.Contracts.Requests;
using Under_the_Bay.API.V1.Contracts.Responses;
using Under_the_Bay.Data.Models;
using Under_the_Bay.Data.Repositories;

namespace Under_the_Bay.API.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IStationsRepository _repo;
        private readonly IMapper _mapper;
        public StationsController(IStationsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StationResponse>), 200)]
        public async Task<ActionResult> GetAll()
        {
            var stations = await _repo.GetAll(); 
            return Ok(_mapper.Map<List<StationResponse>>(stations));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StationResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Get([FromRoute][FromQuery]StationRequest dto)
        {
            if (dto.IncludeMeasurements && !dto.EndDate.HasValue)
            {
                dto.EndDate = DateTimeOffset.Now;
            }
            
            var station = await _repo.GetById(dto.Id, dto.IncludeMeasurements, dto.StartDate, dto.EndDate);

            if (station == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<StationResponse>(station));
        }
    }
}
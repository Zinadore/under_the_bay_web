using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UTB.API.V1.Contracts.Requests;
using UTB.API.V1.Contracts.Responses;
using UTB.Contracts.DTO;
using UTB.Data.Services;

namespace UTB.API.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IStationsService _repo;
        private readonly IMapper _mapper;
        public StationsController(IStationsService repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Retrieves all available stations
        /// </summary>
        /// <remarks>
        /// This will fetch all available stations along with some metadata. Some stations can be more up-to-date
        /// than others, as that depends on external data source we have no control over. You can use the ids provided
        /// from this endpoint to make requests for samples from that station.
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StationDTO>), 200)]
        public async Task<ActionResult> GetAll()
        {
            var stations = await _repo.GetAll(); 
            return Ok(_mapper.Map<List<StationResponse>>(stations));
        }
        
        /// <summary>
        /// Fetches a specific station denoted by id
        /// </summary>
        /// <remarks>
        /// You can use the query string parameters to include samples in the specified data range.
        /// </remarks>
        /// <param name="dto"></param>
        /// <returns></returns>
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
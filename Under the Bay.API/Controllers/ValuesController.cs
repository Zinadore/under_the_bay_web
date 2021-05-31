using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Under_the_Bay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<string>> GetAll()
        {
            return Ok(new List<string> { "value1", "value2" });
        }

        [HttpGet("{id}")]
        public ActionResult<int> Get(int id)
        {
            return Ok(id);
        }
    }
}
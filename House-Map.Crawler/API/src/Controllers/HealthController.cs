using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HouseMapAPI.Controllers
{
    [Route("v1/health")]
    public class HealthController : ControllerBase
    {


        public HealthController()
        {

        }

        [HttpGet("")]
        public IActionResult GetHealth()
        {
            return Ok(new { data = new {  }, code = 0 });
        }

    
    }
}

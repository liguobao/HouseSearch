using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;

namespace HouseMapAPI.Controllers
{
    public class HealthController : ControllerBase
    {


        public HealthController()
        {

        }

        [EnableCors("APICors")]
        [HttpGet("v1/health")]
        public IActionResult GetHealth()
        {
            var header = Request.Headers;
            Console.WriteLine($"header:{Newtonsoft.Json.JsonConvert.SerializeObject(header)}");
            return Ok(new { data = new { }, code = 0 });
        }

        [EnableCors("APICors")]
        [HttpGet("/")]
        public IActionResult GetHost([FromHeader(Name = "X-Real-IP")]string xRealIP, [FromHeader(Name = "X-Forwarded-For")]string xForwardedFor)
        {
            var header = Request.Headers;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]|xRealIP:{xRealIP},xForwardedFor:{xForwardedFor},header:{Newtonsoft.Json.JsonConvert.SerializeObject(header)}");
            return Ok(new { data = new { }, code = 0 });
        }
    }
}

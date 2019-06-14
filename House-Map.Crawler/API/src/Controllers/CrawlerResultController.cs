using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using HouseMap.Common;
using HouseMap.Dao;
using Microsoft.AspNetCore.Cors;

namespace HouseMapAPI.Controllers
{
    [Route("v2/crawler-result")]
    public class CrawlerResultController : ControllerBase
    {


        private readonly IServiceProvider _serviceProvider;

        public CrawlerResultController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [EnableCors("APICors")]
        [HttpGet("")]
        public IActionResult GetHealth([FromQuery]string source)
        {
            var redisTool = _serviceProvider.GetRequiredService<RedisTool>();
            var sourceGroup = _serviceProvider.GetRequiredService<ConfigDapper>().LoadAll().GroupBy(c => c.Source);
            if (!string.IsNullOrEmpty(source))
            {
                sourceGroup = sourceGroup.Where(s => s.Key == source);
            }
            var allState = new List<Object>();
            foreach (var configSource in sourceGroup)
            {
                var keyConfig = RedisKeys.CrawlerState.CopyOne(configSource.Key);
                var value = redisTool.ReadHash(keyConfig);
                allState.Add(new
                {
                    name = configSource.Key,
                    count = configSource.Count(),
                    result = value
                });
            }
            var currentCrawlerList = redisTool.ReadHash(RedisKeys.CurrentCrawler);
            return Ok(new
            {
                code = 0,
                crawlResult = allState,
                currentCrawlers = currentCrawlerList
            });
        }


    }
}

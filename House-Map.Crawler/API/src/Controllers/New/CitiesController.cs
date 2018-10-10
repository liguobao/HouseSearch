using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
using HouseMapAPI.Service;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v2/cities/")]
    public class CitiesController : ControllerBase
    {

        private ConfigService _configService;

        private CrawlerConfigService _crawlerConfigService;

        public CitiesController(
                              ConfigService configService,
                              CrawlerConfigService crawlerConfigService)
        {
            _configService = configService;
            _crawlerConfigService = crawlerConfigService;
        }


        [EnableCors("APICors")]
        [HttpGet("")]
        public IActionResult LoadDashboards()
        {
            var id = 1;
            var city = HttpContext.Request.Query["city"];
            return Ok(new
            {
                success = true,
                data = _configService.LoadCitySources(city).Select(i => new
                {
                    id = id++,
                    city = i.Key,
                    sources = i.Value
                })
            });
        }
        
        [EnableCors("APICors")]
        [HttpGet("{city}")]
        public IActionResult LoadSources(string city)
        {
            return Ok(new
            {
                success = true,
                data = _configService.LoadCitySources(city).FirstOrDefault().Value
            });
        }

        [EnableCors("APICors")]
        [HttpPost("douban")]
        public IActionResult AddDouBanGroup([FromBody]JToken model)
        {
            string doubanGroup = model?["groupId"].ToString();
            string cityName = model?["cityName"].ToString();
            _crawlerConfigService.AddDoubanConfig(doubanGroup, cityName);
            return Ok(new { success = true });
        }
    }
}

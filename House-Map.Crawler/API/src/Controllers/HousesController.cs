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
    [Route("v1/[controller]/")]
    public class HousesController : ControllerBase
    {

        private ConfigService _configService;

        private CrawlerConfigService _crawlerConfigService;

        private HouseService _houseService;



        public HousesController(HouseService houseService,
                              ConfigService configService,
                              CrawlerConfigService crawlerConfigService)
        {
            _houseService = houseService;
            _configService = configService;
            _crawlerConfigService = crawlerConfigService;
        }



        [HttpPost("", Name = "Search")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] HouseCondition search)
        {
            return Ok(new { success = true, data = _houseService.Search(search) });
        }

        [EnableCors("APICors")]
        [HttpGet("dashboard")]
        public IActionResult Dashboards()
        {
            return Ok(new
            {
                success = true,
                data = _configService.LoadDashboard()
            });
        }

        [EnableCors("APICors")]
        [HttpGet("city-source")]
        public IActionResult LoadDashboards()
        {
            var id = 1;
            return Ok(new
            {
                success = true,
                data = _configService.LoadCitySources().Select(i => new
                {
                    id = id + 1,
                    city = i.Key,
                    sources = i.Value
                })
            });
        }



        [EnableCors("APICors")]
        [HttpGet("cities")]
        public IActionResult Cities()
        {
            var id = 1;
            return Ok(new
            {
                success = true,
                data = _configService.LoadConfigs()
                .GroupBy(c => c.City).Select(i => new
                {
                    id = id + 1,
                    name = i.Key
                })
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

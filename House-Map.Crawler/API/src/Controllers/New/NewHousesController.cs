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
    [Route("v2/houses/")]
    public class NewHousesController : ControllerBase
    {

        private ConfigService _configService;

        private CrawlerConfigService _crawlerConfigService;

        private HouseService _houseService;



        public NewHousesController(HouseService houseService,
                              ConfigService configService,
                              CrawlerConfigService crawlerConfigService)
        {
            _houseService = houseService;
            _configService = configService;
            _crawlerConfigService = crawlerConfigService;
        }



        [HttpPost("")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] NewHouseCondition search)
        {
            return Ok(new { success = true, data = _houseService.NewSearch(search) });
        }


        [HttpGet("")]
        [EnableCors("APICors")]
        public IActionResult GetHouse()
        {
            var condition = new NewHouseCondition();
            condition.City = !string.IsNullOrEmpty(HttpContext.Request.Query["city"]) ? HttpContext.Request.Query["city"].ToString() : "上海";
            condition.Source = HttpContext.Request.Query["source"];
            condition.Keyword = HttpContext.Request.Query["keyword"];
            condition.IntervalDay = !string.IsNullOrEmpty(HttpContext.Request.Query["intervalDay"]) ? int.Parse(HttpContext.Request.Query["intervalDay"]) : 14;
            condition.RentType = !string.IsNullOrEmpty(HttpContext.Request.Query["rentType"]) ? int.Parse(HttpContext.Request.Query["RentType"]) : 0;
            condition.FromPrice = !string.IsNullOrEmpty(HttpContext.Request.Query["fromPrice"]) ? int.Parse(HttpContext.Request.Query["fromPrice"]) : 0;
            condition.ToPrice = !string.IsNullOrEmpty(HttpContext.Request.Query["toPrice"]) ? int.Parse(HttpContext.Request.Query["toPrice"]) : 0;
            return Ok(new { success = true, data = _houseService.NewSearch(condition) });
        }

        [HttpGet("{houseId}")]
        [EnableCors("APICors")]
        public IActionResult FindById(string houseId)
        {
            return Ok(new { success = true, data = _houseService.FindById(houseId) });
        }


        [EnableCors("APICors")]
        [HttpGet("sources")]
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
                    id = id++,
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

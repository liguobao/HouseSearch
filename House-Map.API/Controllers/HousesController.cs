using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
using HouseMapAPI.Service;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Dapper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v1/[controller]/")]
    public class HousesController : ControllerBase
    {

        private DashboardService _dashboardService;

        private ConfigService _configService;

        private HouseService _houseService;



        public HousesController(HouseService houseService,
                              DashboardService dashboardService,
                              ConfigService configService)
        {
            _houseService = houseService;
            _dashboardService = dashboardService;
            _configService = configService;
        }



        [HttpPost("", Name = "Search")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] HouseSearchCondition search)
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
                data = _dashboardService.LoadCityDashboards()
            });
        }

        [EnableCors("APICors")]
        [HttpGet("citys")]
        public IActionResult Citys()
        {
            return Ok(new { success = true, data = _dashboardService.LoadCitys() });
        }

        [EnableCors("APICors")]
        [HttpPost("douban")]
        public IActionResult AddDouBanGroup([FromBody]JToken model)
        {
            string doubanGroup = model?["groupId"].ToString();
            string cityName = model?["cityName"].ToString();
            _configService.AddDoubanConfig(doubanGroup, cityName);
            return Ok(new { success = true });
        }

    }
}

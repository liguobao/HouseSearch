using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using HouseMapAPI.Service;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using Swashbuckle.AspNetCore.Annotations;
using HouseMapAPI.Models;


namespace HouseCrawler.Web.API.Controllers
{
    public class CitiesController : ControllerBase
    {

        private readonly ConfigService _configService;

        private readonly DBGroupService _groupService;


        public CitiesController(
                              ConfigService configService,
                              DBGroupService groupService)
        {
            _configService = configService;
            _groupService = groupService;
        }

        /// <summary>
        /// 获取城市房源看板信息
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("v2/cities")]
        [SwaggerResponse(200, "success", typeof(List<SourceDashboard>))]
        public IActionResult LoadDashboards([FromQuery] string city)
        {
            var id = 1;
            var dashboards = _configService.LoadCitySources(city).Select(i => new SourceDashboard()
            {
                id = id++,
                city = i.Key,
                sources = i.Value
            });
            return Ok(new
            {
                success = true,
                data = dashboards
            });
        }

        /// <summary>
        /// 获取某个城市下支持的房源
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("v2/cities/{city}")]
        [SwaggerResponse(200, "success", typeof(List<DBConfig>))]
        public IActionResult LoadSources([FromRoute]string city)
        {
            return Ok(new
            {
                success = true,
                data = _configService.LoadCitySources(city).FirstOrDefault().Value
            });
        }

        /// <summary>
        /// 添加豆瓣来源
        /// </summary>
        [EnableCors("APICors")]
        [HttpPost("v2/cities/douban")]
        public IActionResult AddGroup([FromBody]DoubanSource model)
        {
            return Ok(new { success = _groupService.AddGroupConfig(model?.groupId, model?.city) });
        }
    }
}

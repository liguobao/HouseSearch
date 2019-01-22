using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
using HouseMapAPI.Service;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;



namespace HouseCrawler.Web.API.Controllers
{

    public class HousesController : ControllerBase
    {
        private HouseService _houseService;

        public HousesController(HouseService houseService)
        {
            _houseService = houseService;
        }



        [HttpPost("v2/houses")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] HouseCondition search)
        {
            return Ok(new { success = true, data = _houseService.NewSearch(search) });
        }


        [HttpGet("v2/houses")]
        [EnableCors("APICors")]
        public IActionResult GetHouse()
        {
            var condition = new HouseCondition();
            condition.City = !string.IsNullOrEmpty(HttpContext.Request.Query["city"]) ? HttpContext.Request.Query["city"].ToString() : "上海";
            condition.Source = HttpContext.Request.Query["source"];
            condition.Keyword = HttpContext.Request.Query["keyword"];
            condition.Page = !string.IsNullOrEmpty(HttpContext.Request.Query["page"]) ? int.Parse(HttpContext.Request.Query["page"]) : 0;
            condition.Size = !string.IsNullOrEmpty(HttpContext.Request.Query["size"]) ? int.Parse(HttpContext.Request.Query["size"]) : 600;
            condition.IntervalDay = !string.IsNullOrEmpty(HttpContext.Request.Query["intervalDay"]) ? int.Parse(HttpContext.Request.Query["intervalDay"]) : 14;
            condition.RentType = !string.IsNullOrEmpty(HttpContext.Request.Query["rentType"]) ? int.Parse(HttpContext.Request.Query["rentType"]) : 0;
            condition.FromPrice = !string.IsNullOrEmpty(HttpContext.Request.Query["fromPrice"]) ? int.Parse(HttpContext.Request.Query["fromPrice"]) : 0;
            condition.ToPrice = !string.IsNullOrEmpty(HttpContext.Request.Query["toPrice"]) ? int.Parse(HttpContext.Request.Query["toPrice"]) : 0;
            condition.Refresh = !string.IsNullOrEmpty(HttpContext.Request.Query["refresh"]) ? bool.Parse(HttpContext.Request.Query["refresh"]) : false;
            return Ok(new { success = true, data = _houseService.NewSearch(condition) });
        }

        [HttpGet("v2/houses/{houseId}")]
        [EnableCors("APICors")]
        public IActionResult FindById(string houseId)
        {
            return Ok(new { success = true, data = _houseService.FindById(houseId) });
        }


        [HttpPut("v2/houses/{houseId}/lnglat")]
        [EnableCors("APICors")]
        public IActionResult UpdateLatLng(string houseId, [FromQuery] string lat, [FromQuery] string lng)
        {
            _houseService.UpdateLngLat(houseId, lng, lat);
            return Ok(new { success = true });
        }


    }
}

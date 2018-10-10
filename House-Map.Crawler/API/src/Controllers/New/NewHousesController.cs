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
        private HouseService _houseService;

        public NewHousesController(HouseService houseService)
        {
            _houseService = houseService;
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


    }
}

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseMapAPI.Service;
using HouseMapAPI.Filters;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMap.Common;

namespace HouseCrawler.Web.API.Controllers
{

    public class UserHousesController : ControllerBase
    {

        private UserHouseService _userHouseService;

        public UserHousesController(UserHouseService userHouseService)
        {
            _userHouseService = userHouseService;
        }


        [EnableCors("APICors")]
        [HttpPost("v1/users/{userId}/houses", Name = "AddOne")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult AddOne(long userId, [FromBody] UserHouse userHouse)
        {
            var newOne = _userHouseService.AddOne(userHouse);
            return Ok(new { success = true, data = newOne });
        }


        [EnableCors("APICors")]
        [HttpPut("v1/users/{userId}/houses/{id}", Name = "ModifyOne")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public IActionResult ModifyOne(long userId, string id, [FromBody] UserHouse userHouse)
        {
            userHouse.Id = id;
            var update = _userHouseService.Modify(userId, userHouse);
            return Ok(new { success = true, data = update });
        }

        [EnableCors("APICors")]
        [HttpGet("v1/users/{userId}/houses", Name = "GetList")]
        public IActionResult GetList(long userId)
        {
            return Ok(new
            {
                success = true,
                data = _userHouseService.GetUserHouses(userId)
            });
        }
    }
}
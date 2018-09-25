using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseMapAPI.Dapper;
using HouseMapAPI.Service;
using HouseMapAPI.Common;
using HouseMapAPI.Filters;
using HouseMapAPI.DBEntity;

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
    }
}
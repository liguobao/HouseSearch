using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMapAPI.Service;
using HouseMap.Common;
using HouseMapAPI.Filters;

namespace HouseCrawler.Web.API.Controllers
{

    public class UserController : ControllerBase
    {

        private UserService _userService;



        public UserController(UserService userService)
        {
            _userService = userService;
        }


        [EnableCors("APICors")]
        [HttpGet("v1/users/{userId}", Name = "Info")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public ActionResult Info(long userId, [FromHeader] string token)
        {
            return Ok(new { success = true, data = _userService.GetUserByToken(token) });
        }

        [EnableCors("APICors")]
        [HttpPost("v1/users/{userId}/address", Name = "SetWorkAddress")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public ActionResult SetWorkAddress(long userId, [FromHeader] string token, [FromBody]JToken model)
        {
            return Ok(new
            {
                success = _userService.SaveWorkAddress(userId, model?["address"]?.ToString()),
                data = _userService.GetUserByToken(token)
            });
        }


        [EnableCors("APICors")]
        [HttpPost("v1/users/{userId}/email")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public ActionResult SetEmail(long userId, [FromHeader] string token, [FromBody]JToken model)
        {
            return Ok(new
            {
                success = _userService.SaveEmail(userId, model?["email"]?.ToString()),
                data = _userService.GetUserByToken(token)
            });
        }



    }
}
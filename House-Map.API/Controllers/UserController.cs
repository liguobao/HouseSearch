using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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

namespace HouseCrawler.Web.API.Controllers
{

    public class UserController : ControllerBase
    {

        private UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }


        [EnableCors("APICors")]
        [HttpGet("v1/users/{userId}", Name = "Info")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public ActionResult Info(long userId, [FromHeader] string token)
        {
            return Ok(new { success = true, data = userService.GetUserByToken(token) });
        }

        [EnableCors("APICors")]
        [HttpPost("v1/users/{userId}/address", Name = "SetWorkAddress")]
        [ServiceFilter(typeof(UserTokenFilter))]
        public ActionResult SetWorkAddress(long userId, [FromHeader] string token, [FromBody]JToken model)
        {
            return Ok(new
            {
                success = userService.SaveWorkAddress(userId, model?["address"]?.ToString())
            });
        }
    }
}
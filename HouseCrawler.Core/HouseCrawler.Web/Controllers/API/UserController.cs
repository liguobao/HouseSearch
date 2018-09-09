using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using HouseCrawler.Web;
using HouseCrawler.Web.Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Web.API.Controllers
{

    public class UserController : ControllerBase
    {

        private UserDataDapper userDataDapper;

        private EmailService emailService;

        private EncryptionTools encryptionTools;


        private RedisService redisService;

        private UserService userService;

        public UserController(UserDataDapper userDataDapper,
                          EmailService emailService,
                          EncryptionTools encryptionTools,
                          RedisService redisService,
                          UserService userService)
        {
            this.userDataDapper = userDataDapper;
            this.emailService = emailService;
            this.encryptionTools = encryptionTools;
            this.redisService = redisService;
            this.userService = userService;
        }


        [EnableCors("APICors")]
        [HttpGet("v1/users/{userId}", Name = "Info")]
        public ActionResult Info(long userId, [FromHeader] string token)
        {
            var userInfo = userService.GetUserInfo(userId, token);
            if (userInfo != null)
            {
                return Ok(new { success = true, data = userInfo });
            }
            return Ok(new { success = false, error = "用户不存在/鉴权失败!" });
        }

        [EnableCors("APICors")]
        [HttpPost("v1/users/{userId}/address", Name = "SetWorkAddress")]
        public ActionResult SetWorkAddress(long userId, [FromHeader] string token, [FromBody]JToken model)
        {
            if (string.IsNullOrEmpty(model?["address"]?.ToString()))
            {
                return Ok(new { success = false, error = "地址不能为空!" });
            }
            var userInfo = userService.GetUserInfo(userId, token);
            if (userInfo == null)
            {
                return Ok(new { success = false, error = "用户不存在/鉴权失败!" });
            }
            var success = userDataDapper.SaveWorkAddress(userId, model?["address"]?.ToString());
            return Ok(new { success = success });
        }
    }
}
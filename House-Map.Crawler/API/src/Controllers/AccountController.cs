using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseMapAPI.Service;
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using HouseMap.Common;
using HouseMapAPI.Models;

namespace HouseMapAPI.Controllers
{

    [Route("v1/[controller]")]
    public class AccountController : ControllerBase
    {

        private UserService _userService;

        private IOAuthClient _authClient;


        public AccountController(UserService userService,
                          QQOAuthClient authClient)
        {
            this._userService = userService;
            _authClient = authClient.GetAPIOAuthClient();
        }

        [EnableCors("APICors")]
        [HttpPost("register", Name = "Register")]
        public ActionResult Register([FromBody]UserSave registerUser)
        {
            var result = _userService.Register(registerUser);
            return Ok(new { success = true, message = "注册成功!", token = result.Item1, data = result.Item2 });

        }

        [EnableCors("APICors")]
        [HttpPost("", Name = "Login")]
        public ActionResult Login([FromBody]UserSave loginUser)
        {

            var result = _userService.Login(loginUser);
            return Ok(new { success = true, message = "登录成功!", token = result.Item1, data = result.Item2 });
        }


        [EnableCors("APICors")]
        [HttpGet("callback", Name = "Callback")]
        public ActionResult Callback(string code, string state)
        {
            var result = _userService.OAuthCallback(code);
            return Ok(new { success = true, token = result.Item1, message = "登录成功!", data = result.Item2 });
        }

        [EnableCors("APICors")]
        [HttpGet("activated/{code}", Name = "Activated")]
        public ActionResult Activated(string code)
        {
            var result = _userService.Activated(code);
            return Ok(new { success = true, token = result.Item1, message = "激活成功!", data = result.Item2 });
        }


        [EnableCors("APICors")]
        [HttpPost("weixin", Name = "Weixin")]
        public ActionResult Weixin([FromBody]WechatLoginInfo loginInfo)
        {
            var result = _userService.WechatLogin(loginInfo);
            return Ok(new { success = true, token = result.Item1, message = "登录成功!", data = result.Item2 });
        }

        [EnableCors("APICors")]
        [HttpGet("oauth-url", Name = "GetQQOAuthUrl")]
        public IActionResult GetQQOAuthUrl()
        {
            var url = _authClient.GetAuthUrl();
            return Ok(new { success = true, url = url });
        }

    }
}
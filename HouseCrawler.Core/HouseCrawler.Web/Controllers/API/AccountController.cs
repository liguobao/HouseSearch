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

namespace HouseCrawler.Web.API.Controllers
{

    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private UserDataDapper userDataDapper;

        private EmailService emailService;

        private EncryptionTools encryptionTools;


        private RedisService redisService;

        private UserService userService;

        public AccountController(UserDataDapper userDataDapper,
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
        [HttpPost("/register", Name = "Register")]
        public ActionResult Register([FromBody]UserSave registerUser)
        {
            if (registerUser == null || string.IsNullOrEmpty(registerUser.Email) || string.IsNullOrEmpty(registerUser.UserName))
            {
                return Ok(new { success = false, error = "用户名/用户邮箱不能为空." });
            }
            var checkUser = userDataDapper.FindUser(registerUser.UserName);
            if (checkUser != null)
            {
                return Ok(new { success = false, error = "用户已存在!" });
            }
            try
            {
                string token = Tools.GetMD5(encryptionTools.Crypt(registerUser.UserName + registerUser.Email));
                EmailInfo email = new EmailInfo();
                email.Body = $"Hi,{registerUser.UserName}. <br>欢迎您注册地图搜租房(woyaozufang.live),你的账号已经注册成功." +
                "<br/>为了保证您能正常体验网站服务，请点击下面的链接完成邮箱验证以激活账号."
                + $"<br><a href='https://woyaozufang.live/Account/Activated?activatedCode={token}'>https://woyaozufang.live/Account/Activate?activatedCode={token}</a> "
                + "<br>如果您以上链接无法点击，您可以将以上链接复制并粘贴到浏览器地址栏打开."
                + "<br>此信由系统自动发出，系统不接收回信，因此请勿直接回复。" +
                "<br>如果有其他问题咨询请发邮件到codelover@qq.com.";
                email.Receiver = registerUser.Email;
                email.Subject = "地图找租房-激活账号";
                email.ReceiverName = registerUser.UserName;
                emailService.Send(email);
                var insertUser = new UserInfo();
                insertUser.Email = registerUser.Email;
                insertUser.UserName = registerUser.UserName;
                insertUser.Password = insertUser.Password;
                insertUser.ActivatedCode = token;
                userDataDapper.InsertUser(insertUser);
                return Ok(new { success = true, message = "注册成功!" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.ToString() });
            }


        }

        [EnableCors("APICors")]
        [HttpPost("", Name = "Login")]
        public ActionResult Login([FromBody]UserSave loginUser)
        {
            if (loginUser == null || string.IsNullOrEmpty(loginUser.UserName))
            {
                return Ok(new { success = false, error = "用户名/用户邮箱不能为空." });
            }
            var userInfo = userDataDapper.FindUser(loginUser.UserName);
            if (userInfo != null)
            {
                if (userInfo.Password == Tools.GetMD5(loginUser.Password))
                {
                    string token = encryptionTools.Crypt($"{userInfo.ID}|{userInfo.UserName}");
                    userService.WriteUserToken(userInfo, token);
                    return Ok(new { success = true, token = token, messgae = "登录成功!", data = userInfo });
                }
                else
                {
                    return Ok(new { success = false, error = "密码错误!" });
                }
            }
            else
            {
                return Ok(new { success = false, error = "找不到用户信息或密码错误!" });
            }

        }

        [EnableCors("APICors")]
        [HttpPost("/{userId}", Name = "Info")]
        [HttpGet("/{userId}", Name = "Info")]
        public ActionResult Info(long userId, [FromHeader] string token)
        {
            var userInfo = userService.GetUserInfo(userId, token);
            if (userInfo != null)
            {
                return Ok(new { success = true, data = userInfo });
            }
            return Ok(new { success = false, error = "用户不存在/鉴权失败!" });
        }
    }
}
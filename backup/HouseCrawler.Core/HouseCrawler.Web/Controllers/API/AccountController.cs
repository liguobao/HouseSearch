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

    [Route("v1/[controller]")]
    public class AccountController : ControllerBase
    {

        private UserDataDapper _userDataDapper;

        private EmailService _emailService;

        private EncryptionTools _encryptionTools;



        private UserService _userService;

        private IOAuthClient _authClient;

        public AccountController(UserDataDapper userDataDapper,
                          EmailService emailService,
                          EncryptionTools encryptionTools,
                          UserService userService,
                          QQOAuthClient authClient)
        {
            this._userDataDapper = userDataDapper;
            this._emailService = emailService;
            this._encryptionTools = encryptionTools;
            this._userService = userService;
            _authClient = authClient.GetAPIOAuthClient();
        }

        [EnableCors("APICors")]
        [HttpPost("register", Name = "Register")]
        public ActionResult Register([FromBody]UserSave registerUser)
        {
            if (registerUser == null || string.IsNullOrEmpty(registerUser.Email) || string.IsNullOrEmpty(registerUser.UserName))
            {
                return Ok(new { success = false, error = "用户名/用户邮箱不能为空." });
            }
            var checkUser = _userDataDapper.FindUser(registerUser.UserName);
            if (checkUser != null)
            {
                return Ok(new { success = false, error = "用户已存在!" });
            }
            try
            {
                string token = Tools.GetMD5(_encryptionTools.Crypt(registerUser.UserName + registerUser.Email));
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
                _emailService.Send(email);
                var insertUser = new UserInfo();
                insertUser.Email = registerUser.Email;
                insertUser.UserName = registerUser.UserName;
                insertUser.Password = registerUser.Password;
                insertUser.ActivatedCode = token;
                _userDataDapper.InsertUser(insertUser);
                var userInfo = _userDataDapper.FindUser(insertUser.UserName);
                string loginToken = userInfo.NewLoginToken;
                _userService.WriteUserToken(userInfo, loginToken);
                return Ok(new { success = true, message = "注册成功!", token = loginToken, data = userInfo });
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
            var userInfo = _userDataDapper.FindUser(loginUser.UserName);
            if (userInfo != null)
            {
                if (userInfo.Password == Tools.GetMD5(loginUser.Password))
                {
                    string token = userInfo.NewLoginToken;
                    _userService.WriteUserToken(userInfo, token);
                    return Ok(new { success = true, token = token, message = "登录成功!", data = userInfo });
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
        [HttpGet("callback", Name = "Callback")]
        public ActionResult Callback(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var accessToken = _authClient.GetAccessToken(code).Result;
                    var qqUser = _authClient.GetUserInfo(accessToken).Result;
                    //未登录,通过此ID获取用户
                    var userInfo = _userDataDapper.FindUserByQQOpenUID(qqUser.Id);
                    if (userInfo == null)
                    {
                        //新增用户
                        _userDataDapper.InsertUserForQQAuth(new UserInfo() { UserName = qqUser.Name, QQOpenUID = qqUser.Id });
                        userInfo = _userDataDapper.FindUserByQQOpenUID(qqUser.Id);
                    }
                    string token = userInfo.NewLoginToken;
                    _userService.WriteUserToken(userInfo, token);
                    return Ok(new { success = true, token = token, message = "登录成功!", data = userInfo });
                }
                catch (Exception ex)
                {
                    return Ok(new { success = false, error = ex.ToString() });
                }
            }
            return Ok(new { success = false, error = "无效的auth code" });
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
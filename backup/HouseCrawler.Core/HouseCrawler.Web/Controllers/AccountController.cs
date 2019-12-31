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

namespace HouseCrawler.Web.Controllers
{
    public class AccountController : Controller
    {
        private static string userTokenKey = "user_token_";

        private static string userInfoKey = "user_";

        private UserDataDapper userDataDapper;

        private EmailService emailService;

        private EncryptionTools encryptionTools;

        private APPConfiguration configuration;

        private IOAuthClient authClient;

        private RedisService redisService;

        public AccountController(UserDataDapper userDataDapper,
                          EmailService emailService,
                          EncryptionTools encryptionTools,
                          IOptions<APPConfiguration> configuration,
                          RedisService redisService,
                          QQOAuthClient qqOAuthClient
                          )
        {
            this.userDataDapper = userDataDapper;
            this.emailService = emailService;
            this.encryptionTools = encryptionTools;
            this.configuration = configuration.Value;
            this.redisService = redisService;
            this.authClient = qqOAuthClient.GetOAuthClient();
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Register()
        {
            return View();
        }

        public ActionResult CheckUserEmail(string email)
        {
            var checkUser = userDataDapper.FindUser(email);
            if (checkUser != null)
            {
                return Json(new { success = false, error = "用户已存在!" });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        [EnableCors("APICors")]
        public ActionResult RegisterUser(string userName, string userEmail, string password)
        {
            var checkUser = userDataDapper.FindUser(userName);
            if (checkUser != null)
            {
                return Json(new { success = false, error = "用户已存在!" });
            }
            try
            {
                string token = Tools.GetMD5(encryptionTools.Crypt(userName + userEmail));
                EmailInfo email = new EmailInfo();
                email.Body = $"Hi,{userName}. <br>欢迎您注册地图搜租房(woyaozufang.live),你的账号已经注册成功." +
                "<br/>为了保证您能正常体验网站服务，请点击下面的链接完成邮箱验证以激活账号."
                + $"<br><a href='https://woyaozufang.live/Account/Activated?activatedCode={token}'>https://woyaozufang.live/Account/Activate?activatedCode={token}</a> "
                + "<br>如果您以上链接无法点击，您可以将以上链接复制并粘贴到浏览器地址栏打开."
                + "<br>此信由系统自动发出，系统不接收回信，因此请勿直接回复。" +
                "<br>如果有其他问题咨询请发邮件到codelover@qq.com.";
                email.Receiver = userEmail;
                email.Subject = "地图找租房-激活账号";
                email.ReceiverName = userName;
                emailService.Send(email);
                var user = new UserInfo();
                user.UserName = userName;
                user.Password = password;
                user.Email = userEmail;
                user.ActivatedCode = token;
                userDataDapper.InsertUser(user);
                return Json(new { success = true, message = "注册成功!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.ToString() });
            }


        }

        [EnableCors("APICors")]
        public ActionResult Login(string userName, string password)
        {
            //var loginUser = new UserInfo(){ Email="test@qq.com", Password = "e10adc3949ba59abbe56e057f20f883e", Status =1};
            var loginUser = userDataDapper.FindUser(userName);
            if (loginUser != null)
            {
                if (loginUser.Status != 1)
                {
                    return Json(new { success = false, error = "账号未激活/已被禁用,请点击激活邮件中的URL完成账号激活!" });
                }

                if (loginUser.Password == Tools.GetMD5(password))
                {
                    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                                  {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, loginUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, loginUser.ID.ToString())
                    }, CookieAuthenticationDefaults.AuthenticationScheme));
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                    }).Wait();
                    string token = loginUser.NewLoginToken;
                    return Json(new { success = true, token = token, message = "登录成功!" });
                }
                else
                {
                    return Json(new { success = false, error = "密码错误!" });
                }
            }
            else
            {
                return Json(new { success = false, error = "找不到用户信息或密码错误!" });
            }

        }


        public ActionResult SignOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Json(new { success = true, message = "退出成功!" });
        }


        public ActionResult Activated(string activatedCode)
        {
            ViewResult result = new ViewResult();
            var userInfo = userDataDapper.FindUserByActivatedCode(activatedCode);
            if (userInfo != null)
            {
                if (userInfo.Status == 1)
                {
                    result.success = true;
                    result.message = "账号已激活。";
                }
                else
                {
                    userInfo.Status = 1;
                    userDataDapper.SaveUserStatus(userInfo.ID, 1);
                    result.success = true;
                    result.message = "激活账号成功！";
                }

            }
            else
            {
                result.success = true;
                result.error = "找不到用户,请重新注册.";
            }

            return View(result);
        }

        public ActionResult RetrievePassword()
        {
            return View();
        }


        public ActionResult SendRetrievePasswordEmail(string emailAccount)
        {
            try
            {
                var user = userDataDapper.FindUser(emailAccount);
                var token = Tools.GetMD5(encryptionTools.Crypt(user.UserName + user.Email + DateTime.Now.ToString()));
                EmailInfo email = new EmailInfo();
                email.Body = $"Hi,{user.UserName}. <br>您正在通过注册邮箱找回密码,如果非本人操作,请勿继续."
                + "<br>请在24小时内点击以下链接重置密码:"
                + $"<br><a href='https://woyaozufang.live/Account/ModifyPassword?token={token}'>https://woyaozufang.live/Account/ModifyPassword?token={token}</a> "
                + "<br>如果您以上链接无法点击，您可以将以上链接复制并粘贴到浏览器地址栏打开."
                + "<br>此信由系统自动发出，系统不接收回信，因此请勿直接回复。" +
                "<br>如果有其他问题咨询请发邮件到codelover@qq.com.";
                email.Receiver = user.Email;
                email.Subject = "地图找租房-找回密码";
                email.ReceiverName = user.UserName;
                emailService.Send(email);
                userDataDapper.SaveRetrievePasswordToken(user.ID, token);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.ToString() });
            }

        }

        public ActionResult ModifyPassword()
        {
            return View();
        }

        public ActionResult ResetPassword(string token, string password)
        {
            var user = userDataDapper.FindUserByToken(token);
            if (user != null && (DateTime.Now.ToLocalTime() - user.TokenTime).TotalHours > 24)
            {
                return Json(new { success = false, error = "Token无效或者重置密码链接已超过24小时,请重新操作." });
            }
            userDataDapper.SavePassword(user.ID, Tools.GetMD5(password));
            return Json(new { success = true });
        }


        public ActionResult SetWorkAddress(string address)
        {
            var userId = GetUserID();
            var success = userDataDapper.SaveWorkAddress(userId, address);
            return Json(new { success = success });
        }

        public ActionResult Info()
        {
            var userId = GetUserID();
            if (userId == 0)
            {
                return Json(new { success = false, error = "用户尚未登录." });
            }
            var userInfo = userDataDapper.FindByID(userId);
            return Json(new { success = true, data = userInfo });
        }


        public ActionResult Auth(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var accessToken = authClient.GetAccessToken(code).Result;
                    var qqUser = authClient.GetUserInfo(accessToken).Result;
                    var userInfo = userDataDapper.FindByID(GetUserID());
                    //已登录 = 绑定此QQ
                    if (userInfo != null)
                    {
                        userInfo.QQOpenUID = qqUser.Id;
                        userDataDapper.UpdateUser(userInfo, new List<string>() { "QQOpenUID" });
                        return View(new ViewResult() { success = true, message = "绑定成功!" });
                    }
                    //未登录,通过此ID获取用户
                    userInfo = userDataDapper.FindUserByQQOpenUID(qqUser.Id);
                    if (userInfo == null)
                    {
                        //新增用户
                        userDataDapper.InsertUserForQQAuth(new UserInfo() { UserName = qqUser.Name, QQOpenUID = qqUser.Id });
                        userInfo = userDataDapper.FindUserByQQOpenUID(qqUser.Id);
                    }
                    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                                      {
                    new Claim(ClaimTypes.Name, userInfo.UserName),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.NameIdentifier, userInfo.ID.ToString())
                    }, CookieAuthenticationDefaults.AuthenticationScheme));
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
                    }).Wait();
                    return View(new ViewResult() { success = true, message = "登录成功!" });
                }
                catch (Exception ex)
                {
                    return View(new ViewResult() { success = false, error = ex.ToString() });
                }
            }
            return View(new ViewResult() { success = false, error = "无效的auth code" });
        }

        private long GetUserID()
        {
            var identity = ((ClaimsIdentity)HttpContext.User.Identity);
            if (identity == null || identity.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return 0;
            }
            var userID = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(userID) || userID == "0")
            {
                return 0;
            }

            return long.Parse(userID);
        }



        public IActionResult GetOAuthQQUrl()
        {
            var url = authClient.GetAuthUrl();
            return Redirect(url);
        }

    }
}
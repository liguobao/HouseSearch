using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using HouseCrawler.Web;
using HouseCrawler.Web.DataContent;
using HouseCrawler.Web.Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace HouseCrawler.Web.Controllers
{
    public class AccountController : Controller
    {
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
            var checkUser = UserDataDapper.FindUser(email);
            if (checkUser != null)
            {
                return Json(new { success = false, error = "用户已存在!" });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        public ActionResult RegisterUser(string userName, string userEmail, string password)
        {
            var checkUser = UserDataDapper.FindUser(userName);
            if (checkUser != null)
            {
                return Json(new { success = false, error = "用户已存在!" });
            }
            try
            {
                string token = Tools.GetMD5(EncryptionTools.Crypt(userName + userEmail));
                EmailInfo email = new EmailInfo();
                email.Body =  $"Hi,{userName}. <br>欢迎您注册地图搜租房(woyaozufang.live),你的账号已经注册成功."+
                "<br/>为了保证您能正常体验网站服务，请点击下面的链接完成邮箱验证以激活账号."
                +$"<br><a href='http://woyaozufang.live:8080/Account/Activated?activatedCode={token}'>http://woyaozufang.live:8080/Account/Activate?activatedCode={token}</a> "
                +"<br>如果您以上链接无法点击，您可以将以上链接复制并粘贴到浏览器地址栏打开."
                +"<br>此信由系统自动发出，系统不接收回信，因此请勿直接回复。"+
                "<br>如果有其他问题咨询请发邮件到codelover@qq.com.";
                email.Receiver = userEmail;
                email.Subject = "地图找租房-激活账号";
                email.ReceiverName = userName;
                email.Send();
                var user = new UserInfo();
                user.UserName = userName;
                user.Password = password;
                user.Email = userEmail;
                user.ActivatedCode = token;
                UserDataDapper.InsertUser(user);
                return Json(new { success = true, message = "注册成功!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.ToString() });
            }


        }


        public ActionResult Login(string userName, string password)
        {
            //var loginUser = new UserInfo(){ Email="test@qq.com", Password = "e10adc3949ba59abbe56e057f20f883e", Status =1};

            var loginUser = UserDataDapper.FindUser(userName);
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
                    string token = EncryptionTools.Crypt($"{loginUser.ID}|{loginUser.UserName}");
                    return Json(new { success = true, token = token, messgae = "登录成功!" });
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



        public ActionResult Activated(string activatedCode)
        {
            ViewResult result = new ViewResult();
            var userInfo = UserDataDapper.FindUserByActivatedCode(activatedCode);
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
                    UserDataDapper.SaveUserStatus(userInfo.ID, 1);
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
    }
}
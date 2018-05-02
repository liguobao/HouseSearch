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
            else
            {
                string token =Tools.GetMD5(EncryptionTools.Crypt(userName + userEmail));
                EmailInfo email = new EmailInfo();
                email.Body = token;
                email.Receiver = userEmail;
                email.Subject = "woyaozufang.live Get Toekn";
                email.ReceiverName = userName;
                email.Send();
                var user = new UserInfo();
                user.UserName = userName;
                user.Password = password;
                user.Email = userEmail;
                user.ActivatedCode = token;
                UserDataDapper.InsertUser(user);
                return Json(new { success = true, messgae = "注册成功!" });

            }
        }


        public ActionResult Login(string userName, string password)
        {
            var loginUser = UserDataDapper.FindUser(userName);
            if (loginUser != null)
            {
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
    }
}
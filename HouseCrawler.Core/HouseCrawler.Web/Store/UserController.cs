using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HouseCrawler.Web.DataContent;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HouseCrawler.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(String email, string token)
        {
            var user = new ClaimsPrincipal( new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
            },
            CookieAuthenticationDefaults.AuthenticationScheme));
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user,
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
            }).Wait();
            return new JsonResult(new { success = true});
        }



        public async Task<IActionResult> Logout()
        {
            var result = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }


        public IActionResult AddUserCollection(string onlineURL)
        {
            var house =  CrawlerDataDapper.GetHouseByOnlineURL(onlineURL);
            return null;
        }


    }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using HouseCrawler.Web;
using HouseCrawler.Web.DataContent;
using HouseCrawler.Web.Service;

namespace HouseCrawler.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetToken(string userName, string userEmail)
        {
            string token = EncryptionTools.Crypt(userName+userEmail);
            EmailInfo email = new EmailInfo();
            email.Body = token;
            email.Receiver = userEmail;
            email.Subject = "woyaozufang.live Get Toekn";
            email.ReceiverName = userName;
            email.Send();
            return Json(new { IsSuccess = true, messgae= "发送成功!" });
        }


        public ActionResult Login(string userName, string password)
        {
            var user = UserDataDapper.FindUser(userName, password);
            return Json(new { success = true, messgae= "登录成功!" });
        }
    }
}
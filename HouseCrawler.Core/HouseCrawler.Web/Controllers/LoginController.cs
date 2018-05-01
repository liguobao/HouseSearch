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
    public class LoginController : Controller
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

        public ActionResult Login(string token)
        {
            // check token
            //login
            return View();
        }

        

    }
}
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
            string token =  EncryptionTools.DecryptString(userName+userEmail);
            // creat token
            //send email
            return View();
        }

        public ActionResult Login(string token)
        {
            // check token
            //login
            return View();
        }

        

    }
}
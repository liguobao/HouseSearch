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

namespace HouseCrawler.Web.Controllers
{
    public class UserHouseController : Controller
    {

        private UserDataDapper userDataDapper;

        private UserHouseDapper userHouseDapper;

        private EncryptionTools encryptionTools;

        public UserHouseController(UserDataDapper userDataDapper,
                          UserHouseDapper userHouseDapper,
                          EncryptionTools encryptionTools)
        {
            this.userDataDapper = userDataDapper;
            this.encryptionTools = encryptionTools;
            this.userHouseDapper = userHouseDapper;
        }

        public ActionResult AddHouse(HouseInfo house)
        {
            return Json(new { success = true });
        }

    }
}
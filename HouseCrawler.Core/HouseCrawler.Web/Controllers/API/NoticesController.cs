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
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Web.API.Controllers
{

    [Route("v1/[controller]/")]
    public class NoticesController : ControllerBase
    {
        private NoticeDapper _dapper;

        public NoticesController(NoticeDapper dapper)
        {
            _dapper = dapper;
        }

        [EnableCors("APICors")]
        [HttpGet("", Name = "Notice")]
        public ActionResult Info()
        {
            return Ok(new { success = true, data = _dapper.FindAllNotice() });
        }


        [EnableCors("APICors")]
        [HttpGet("last", Name = "FindLastNotice")]
        public ActionResult FindLastNotice()
        {
            return Ok(new { success = true, data = _dapper.FindLastNotice() });
        }
    }
}
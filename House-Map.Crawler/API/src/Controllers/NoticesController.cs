using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;


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
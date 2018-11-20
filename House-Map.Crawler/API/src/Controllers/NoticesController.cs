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
using HouseMapAPI.Service;

namespace HouseCrawler.Web.API.Controllers
{

    [Route("v1/[controller]/")]
    public class NoticesController : ControllerBase
    {
        private readonly NoticeDapper _dapper;

        private readonly NoticeService _noticeService;

        public NoticesController(NoticeDapper dapper, NoticeService noticeService)
        {
            _dapper = dapper;
            _noticeService = noticeService;
        }

        [EnableCors("APICors")]
        [HttpGet("", Name = "Notice")]
        public ActionResult Info()
        {
            return Ok(new { success = true, data = _dapper.FindAllNotice() });
        }


        [EnableCors("APICors")]
        [HttpGet("{id}")]
        public ActionResult One(long id)
        {
            return Ok(new { success = true, data = _noticeService.FindNotice(id) });
        }





        [EnableCors("APICors")]
        [HttpGet("last", Name = "FindLastNotice")]
        public ActionResult FindLastNotice()
        {
            return Ok(new { success = true, data = _dapper.FindLastNotice() });
        }
    }
}
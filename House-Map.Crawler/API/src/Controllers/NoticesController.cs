using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using HouseMap.Dao;
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

        /// <summary>
        /// 获取所有的公告
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("", Name = "Notice")]
        public ActionResult Info()
        {
            return Ok(new { success = true, data = _dapper.FindAllNotice() });
        }

        /// <summary>
        /// 获取单条公告
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("{id}")]
        public ActionResult One(long id)
        {
            return Ok(new { success = true, data = _noticeService.FindNotice(id) });
        }




        /// <summary>
        /// 获取最新公告
        /// </summary>
        [EnableCors("APICors")]
        [HttpGet("last", Name = "FindLastNotice")]
        public ActionResult FindLastNotice()
        {
            return Ok(new { success = true, data = _dapper.FindLastNotice() });
        }
    }
}
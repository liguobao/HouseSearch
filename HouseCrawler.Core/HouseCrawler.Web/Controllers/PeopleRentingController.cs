
using HouseCrawler.Web.Common;
using HouseCrawler.Web.Models;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Web.Controllers
{
    public class PeopleRentingController : Controller
    {
        //
        // GET: /PeopleRenting/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult GetRentingData()
        {

            var roomList = Enumerable.Range(1, 10).Select(index => GetRoomList(index)).Aggregate((a, b) => a.Concat(b));
            return Json(new { IsSuccess = true, HouseInfos = roomList });
        }


        public ActionResult GetRentingDatabyPageIndex(int index)
        {
            try
            {
                var roomList = GetRoomList(index);
                return Json(new { IsSuccess = true, HouseInfos = roomList });
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetRentingDatabyPageIndex Exception", ex);
                return Json(new
                {
                    IsSuccess = false,
                    Error = $"http://www.huzhumaifang.com/Renting/index/p/{index}.html" +
                    "获取数据异常，可能是哪里挂了吧。看不懂的异常如下：" + ex.ToString()
                });
            }

        }

        private int GetPageCount(string indexURL)
        {
            var htmlResult = HTTPHelper.GetHTMLByURL(indexURL);
            var page = new HtmlParser().Parse(htmlResult);
            return Convert.ToInt32(page.QuerySelector("a.end")?.TextContent ?? "0");
        }

        private IEnumerable<HouseInfo> GetRoomList(int pageNum)
        {
            var houses = PeopleRentingCrawler.GetHouseData(pageNum);

            return houses;

        }
    }
}

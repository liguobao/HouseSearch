
using HouseCrawler.Core.Common;
using HouseCrawler.Core.Models;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Core.Controllers
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
            //var indexURL = "http://www.huzhumaifang.com/Renting/index";
            //var pageCount = GetPageCount(indexURL);每页数据量太少，只取前一百条数据
            var roomList = Enumerable.Range(1, 10).Select(index => GetRoomList($"http://www.huzhumaifang.com/Renting/index/p/{index}.html")).Aggregate((a, b) => a.Concat(b));
            return Json(new { IsSuccess = true, HouseInfos = roomList });
        }


        public ActionResult GetRentingDatabyPageIndex(int index)
        {

            var roomurl = $"http://www.huzhumaifang.com/Renting/index/p/{index}.html";
            try
            {
                var roomList = GetRoomList(roomurl);
                return Json(new { IsSuccess = true, HouseInfos = roomList });
            }catch(Exception ex)
            {
                LogHelper.Error("GetRentingDatabyPageIndex Exception", ex,new { URL= roomurl });
                return Json(new { IsSuccess = false, Error =$"http://www.huzhumaifang.com/Renting/index/p/{index}.html"+
                    "获取数据异常，可能是哪里挂了吧。看不懂的异常如下：" +ex.ToString() });
            }
          
        }

        private int GetPageCount(string indexUrl)
        {
            var htmlResult = HTTPHelper.GetHTMLByURL(indexUrl);
            var page = new HtmlParser().Parse(htmlResult);
            return Convert.ToInt32(page.QuerySelector("a.end")?.TextContent ?? "0");
        }

        private IEnumerable<HouseInfo> GetRoomList(string url)
        {
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);
            return page.QuerySelector("ul.screening_left_ul").QuerySelectorAll("li").Select(room =>
            {
                var screeningTime = room.QuerySelector("p.screening_time").TextContent;
                var screeningPrice = room.QuerySelector("h5").TextContent;
                var locationInfo = room.QuerySelector("a");
                var locationContent = locationInfo.TextContent.Split('，').FirstOrDefault();
                var location = locationContent.Remove(0, locationContent.IndexOf("租") + 1);

                int.TryParse(screeningPrice.Replace("￥", "").Replace("元/月", ""),out var housePrice);

                var markBgType = LocationMarkBGType.SelectColor(housePrice/1000);

                return new HouseInfo
                {
                    Money = screeningPrice,
                    HouseURL = "http://www.huzhumaifang.com" + locationInfo.GetAttribute("href"),
                    HouseLocation = location,
                    HouseTime = screeningTime,
                    HousePrice = housePrice,
                    LocationMarkBG = markBgType,
                };
            });
        }
    }
}

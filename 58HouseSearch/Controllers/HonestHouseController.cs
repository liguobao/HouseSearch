using _58HouseSearch.Models;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _58HouseSearch.Controllers
{
    public class HonestHouseController : Controller
    {
        //
        // GET: /HonestHouse/

        public ActionResult Index()
        {
            HTTPHelper.WritePVInfo(Server.MapPath("./pv.json"), Request.UserHostAddress);
            return View();
        }

        public ActionResult GetHonestHouseData(int costFrom, int costTo, string cnName)
        {
            if (costTo <= 0 || costTo < costFrom)
            {
                return Json(new { IsSuccess = false, Error = "输入数据有误，请重新输入。" });
            }

            if (string.IsNullOrEmpty(cnName))
            {
                return Json(new { IsSuccess = false, Error = "城市定位失败，建议清除浏览器缓存后重新进入。" });
            }
           
            try
            {
                var pageCount = GetPageNum(costFrom, costTo, cnName);
                if (pageCount == 0)
                    return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房源信息,大部分情况是因为该市没有诚信房源数据。" });

                var roomList = Enumerable.Range(1, pageCount).Select(index => GetRoomList(costFrom, costTo, cnName, index)).Aggregate((a, b) => a.Concat(b));
                return Json(new { IsSuccess = true, HouseInfos = roomList });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Error = "由于某种诡异的原因，爬取数据奔溃了...建议重试或者重新输入条件。下面是你看不懂的异常信息："+ex.ToString()  });
            }
          

         

        }

        /// <summary>
        /// 未完成
        /// </summary>
        /// <param name="costFrom"></param>
        /// <param name="costTo"></param>
        /// <param name="cnName"></param>
        /// <param name="index"></param>
        /// <returns></returns>

        private IEnumerable<HouseInfo> GetRoomList(int costFrom, int costTo, string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/zufang/pn{index}/?isreal=true&minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);
            var houseList = page.QuerySelectorAll("tr[logr]").Select(room =>
              new HouseInfo
              {
                  // HouseLocation=room.QuerySelector("a.a_xq1").TextContent.Replace("租房",""),
                  HouseLocation = room.QuerySelector("span.f12")!=null && !string.IsNullOrEmpty(room.QuerySelector("span.f12").TextContent)? 
                  room.QuerySelector("span.f12").TextContent.Replace("租房", ""): room.QuerySelector("a.a_xq1").TextContent.Replace("租房", ""),
                 
                   HouseTitle=room.QuerySelector("a.t").TextContent,
                   Money=room.QuerySelector("b.pri").TextContent,
                   HouseURL= $"http://{cnName}.58.com/zufang/{room.GetAttribute("logr").Split('_')[3]}x.shtml"
              });
            return houseList;
        }
        public int GetPageNum(int costFrom, int costTo, string cnName)
        {
            var url = $"http://{cnName}.58.com/zufang/pn1/?isreal=true&minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var dom = new HtmlParser().Parse(htmlResult);
            var pageNums = dom.QuerySelector(".pager")?.QuerySelectorAll("span")?.Select(page => 
            {
                int number = 0;
                return int.TryParse(page.TextContent, out number) ? number : 0;
            });
            return pageNums!=null && pageNums.Count()!=0 ? pageNums.Max() : 0;
        }

    }
}

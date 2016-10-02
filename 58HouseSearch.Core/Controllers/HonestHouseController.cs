using _58HouseSearch.Core.Common;
using _58HouseSearch.Core.Models;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace _58HouseSearch.Core.Controllers
{
    public class HonestHouseController : Controller
    {
        //
        // GET: /HonestHouse/

        public ActionResult Index()
        {
            HTTPHelper.WritePVInfo(Request.Scheme, Request.Path);
            return View();
        }

      

        public ActionResult GetHonestHousePageCount(int costFrom, int costTo, string cnName)
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
                return Json(new { IsSuccess = true, PageCount = pageCount });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Error = "由于某种诡异的原因，爬取数据奔溃了...建议重试或者重新输入条件。下面是你看不懂的异常信息：" + ex.ToString() });
            }




        }


        public ActionResult GetHonestHouseDataByCostAndPageIndex(int costFrom, int costTo, string cnName,int index)
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

                var roomList = GetRoomList(costFrom, costTo, cnName, index);
                return Json(new { IsSuccess = true, HouseInfos = roomList,PageIndex=index });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Error = "由于某种诡异的原因，爬取数据奔溃了...建议重试或者重新输入条件。下面是你看不懂的异常信息：" + ex.ToString() });
            }




        }




        public ActionResult GetDefaultHonestHouseData( string cnName,int index)
        {
          
            try
            {
                var roomList = GetRoomListByIndex(cnName,index);
                if(roomList==null)
                    return  Json(new { IsSuccess = false, Error = "并没有找到房源信息..." });
                return Json(new { IsSuccess = true, HouseInfos = roomList });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Error = "由于某种诡异的原因，爬取数据奔溃了...建议重试或者重新输入条件。下面是你看不懂的异常信息：" + ex.ToString() });
            }




        }

        private IEnumerable<HouseInfo> GetRoomListByIndex(string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/zufang/pn{index}/?isreal=true";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);
            var houseList = page.QuerySelectorAll("tr[logr]").Where(room=> room.QuerySelector("b.pri")!=null).Select(room =>
            {
                decimal housePrice = 0;
                decimal.TryParse(room.QuerySelector("b.pri").TextContent, out housePrice);
                var markBGType = (housePrice / 1000) > (int)LocationMarkBGType.Black ? LocationMarkBGType.Black : (LocationMarkBGType)(housePrice / 1000);
                return new HouseInfo
                {
                    // HouseLocation=room.QuerySelector("a.a_xq1").TextContent.Replace("租房",""),
                    HouseLocation = room.QuerySelector("span.f12") != null && !string.IsNullOrEmpty(room.QuerySelector("span.f12").TextContent) ?
                   room.QuerySelector("span.f12").TextContent.Replace("租房", "") : room.QuerySelector("a.a_xq1") != null && !string.IsNullOrEmpty(room.QuerySelector("a.a_xq1").TextContent) ?
                   room.QuerySelector("a.a_xq1").TextContent.Replace("租房", "") : "",
                    HouseTitle = room.QuerySelector("a.t") != null ? room.QuerySelector("a.t").TextContent : "",
                    Money = room.QuerySelector("b.pri") != null ? room.QuerySelector("b.pri").TextContent : "",
                    HouseURL = $"http://{cnName}.58.com/zufang/{room.GetAttribute("logr").Split('_')[3]}x.shtml",
                    LocationMarkBG = markBGType.ToString()+".png",
                };
            } );
            return houseList.Where(room => !string.IsNullOrEmpty(room.HouseLocation) && !string.IsNullOrEmpty(room.HouseTitle) && !string.IsNullOrEmpty(room.Money));
        }



        private IEnumerable<HouseInfo> GetRoomList(int costFrom, int costTo, string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/zufang/pn{index}/?isreal=true&minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);
            var houseList = page.QuerySelectorAll("tr[logr]").Select(room =>
              new HouseInfo
              {
                  // HouseLocation=room.QuerySelector("a.a_xq1").TextContent.Replace("租房",""),
                  HouseLocation = GetLocation(room),
                  HouseTitle = room.QuerySelector("a.t")?.TextContent,
                  Money = room.QuerySelector("b.pri")?.TextContent,
                  HouseURL = $"http://{cnName}.58.com/zufang/{room.GetAttribute("logr").Split('_')[3]}x.shtml"
              });
            return houseList.Where(room=>!string.IsNullOrEmpty(room.HouseLocation) && !string.IsNullOrEmpty(room.HouseTitle) && !string.IsNullOrEmpty(room.Money));
        }

        private string GetLocation(IElement room)
        {
            var first = room.QuerySelector("span.f12")?.TextContent;
            var second = room.QuerySelector("a.a_xq1")?.TextContent;
            return !string.IsNullOrEmpty(first) ? first : second;
        }


        private int GetPageNum(int costFrom, int costTo, string cnName)
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

using HouseCrawler.Web.Common;
using HouseCrawler.Web.Models;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Web.Controllers
{
    public class HonestHouseController : Controller
    {
        //
        // GET: /HonestHouse/

        public ActionResult Index()
        {
           
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
                {
                    return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房源信息,大部分情况是因为该市没有诚信房源数据。" });
                }
                return Json(new { IsSuccess = true, PageCount = pageCount });
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetHonestHousePageCount Exception", ex, new { CNName = cnName, CostFrom = costFrom , CostTo  = costTo });

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
                {
                    return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房源信息,大部分情况是因为该市没有诚信房源数据。" });
                }
                var roomList = GetRoomList(costFrom, costTo, cnName, index);
                return Json(new { IsSuccess = true, HouseInfos = roomList, PageIndex = index });
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetHonestHouseDataByCostAndPageIndex Exception", ex, new { CNName = cnName, Index = index });
                return Json(new { IsSuccess = false, Error = "由于某种诡异的原因，爬取数据奔溃了...建议重试或者重新输入条件。下面是你看不懂的异常信息：" + ex.ToString() });
            }
        }

        public ActionResult GetDefaultHonestHouseData( string cnName,int index)
        {
          
            try
            {
                //var pages = GetPageNumByIndex(cnName);
                //if(pages==0) return Json(new { IsSuccess = false, Error = "并没有找到房源信息..." });
                var roomList = GetRoomListByIndex(cnName,index);
                if(roomList==null) return  Json(new { IsSuccess = false, Error = "并没有找到房源信息..." });
                return Json(new { IsSuccess = true, HouseInfos = roomList });
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetDefaultHonestHouseData Exception", ex,new { CNName = cnName,Index = index});
                return Json(new { IsSuccess = false, Error = "由于某种诡异的原因，爬取数据奔溃了...建议重试或者重新输入条件。下面是你看不懂的异常信息：" + ex.ToString() });
            }
        }

        private IEnumerable<HouseInfo> GetRoomListByIndex(string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/zufang/pn{index}/?isreal=true";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var houseList = ParseRoom(htmlResult);
            return houseList;
        }

        private IEnumerable<HouseInfo> GetRoomList(int costFrom, int costTo, string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/zufang/pn{index}/?isreal=true&minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var houseList = ParseRoom(htmlResult);
            return houseList;
        }

        private IEnumerable<HouseInfo> ParseRoom(string html)
        {
            if (html.Contains("验证码"))
            {
                return new List<HouseInfo>();
            }
            var page = new HtmlParser().Parse(html);
            return page.QuerySelector("ul.listUl").QuerySelectorAll("li[logr]").Select(room =>
            {
                int housePrice = 0;
                int.TryParse(room.QuerySelector("b").TextContent, out housePrice);
                var markBGType = LocationMarkBGType.SelectColor(housePrice / 1000);
                var a = room.QuerySelector("a");
                return new HouseInfo
                {
                    HouseLocation = room.QuerySelector("p.add").QuerySelectorAll("a").Last().TextContent.Replace("租房", ""),
                    HouseTitle = a.TextContent,
                    DisPlayPrice = housePrice.ToString(),
                    HouseOnlineURL = a.GetAttribute("href")
                };
            });
        }

        private int GetPageNum(int costFrom, int costTo, string cnName)
        {
            var url = $"http://{cnName}.58.com/zufang/pn1/?isreal=true&minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            return ParsePages(htmlResult);
        }

        private int GetPageNumByIndex(string cnName)
        {
            var url = $"http://{cnName}.58.com/zufang/pn1/?isreal=true";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            return ParsePages(htmlResult);
        }

        private int ParsePages(string html)
        {
            var dom = new HtmlParser().Parse(html);
            var pageNums = dom.QuerySelector(".pager")?.QuerySelectorAll("span")?.Select(page =>
            {
                int number = 0;
                return int.TryParse(page.TextContent, out number) ? number : 0;
            });
            return pageNums?.Max() ?? 0;
        }

    }
}

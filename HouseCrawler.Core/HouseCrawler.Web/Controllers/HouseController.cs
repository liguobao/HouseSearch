using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using AngleSharp.Parser.Html;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.Controllers
{
    public class HouseController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
           
            return View();
        }



        /// <summary>
        /// 获取本次请求总页面，用于下次请求总数
        /// </summary>
        /// <param name="costFrom"></param>
        /// <param name="costTo"></param>
        /// <param name="cnName"></param>
        /// <returns></returns>
        public ActionResult Get58CityRoomPageCount(int costFrom, int costTo, string cnName)
        {

            if (costTo <= 0 || costTo < costFrom)
            {
                return Json(new { IsSuccess = false, Error = "输入数据有误，请重新输入。" });
            }
            if (string.IsNullOrEmpty(cnName))
            {
                return Json(new { IsSuccess = false, Error = "城市定位失败，建议清除浏览器缓存后重新进入。" });
            }
            var listSum = GetListSum(costFrom, costTo, cnName);
            var pageCount = listSum % 20 == 0 ? listSum / 20 : listSum / 20 + 1;
            if (pageCount == 0)
            {
                return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房子。" });
            }
            return Json(new { IsSuccess = true, PageCount = pageCount });
        }

        public ActionResult Get58CityRoomDataByCostAndIndex(int costFrom, int costTo, string cnName, int index)
        {
            try
            {
                var roomList = GetRoomList(costFrom, costTo, cnName, index);
                return Json(new { IsSuccess = true, HouseInfos = roomList, PageIndex = index });
            }
            catch (Exception ex)
            {
                LogHelper.Error("Get58CityRoomDataByCostAndIndex Exception", ex, new { CNName = cnName, CostFrom = costFrom, CostTo = costTo,Index =index });
                return Json(new { IsSuccess = false, Error = "获取数据异常。" + ex.ToString(), PageIndex = index });
            }

        }

        public ActionResult Get58CityRoomData(int costFrom, int costTo, string cnName)
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
                var listSum = GetListSum(costFrom, costTo, cnName);
                var pageCount = listSum % 20 == 0 ? listSum / 20 : listSum / 20 + 1;
                if (pageCount == 0)
                {
                    return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房子。" });
                }
                var roomList = Enumerable.Range(1, pageCount).Select(index => GetRoomList(costFrom, costTo, cnName, index)).Aggregate((a, b) => a.Concat(b)).Take(listSum);
                return Json(new { IsSuccess = true, HouseInfos = roomList });
            }
            catch (Exception ex)
            {

                LogHelper.Error("Get58CityRoomData Exception", ex, new { CNName = cnName, CostFrom = costFrom, CostTo = costTo });

                return Json(new { IsSuccess = false, Error = "获取数据异常。" + ex.ToString() });
            }
        }

        public ActionResult GetDefault58CityRoomData(string cnName, int index)
        {
            var lstHouseInfo = GetRoomListByIndex(cnName, index);
            if (lstHouseInfo == null)
                return Json(new { IsSuccess = false, Error = "没有房源信息..." });
            return Json(new { IsSuccess = true, HouseInfos = lstHouseInfo });
        }

        private IEnumerable<HouseInfo> GetRoomListByIndex(string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/pinpaigongyu/pn/{index}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);
            return page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr")).Select(element =>
            {
                var houseTitle = element.QuerySelector("h2").TextContent;
                var houseInfoList = houseTitle.Split(' ');
                int housePrice = 0;
                int.TryParse(element.QuerySelector("b").TextContent, out housePrice);
                var markBGType = LocationMarkBGType.SelectColor(housePrice/1000);

                return new HouseInfo
                {
                    HouseTitle = houseTitle,
                    HouseOnlineURL = $"http://{cnName}.58.com" + element.QuerySelector("a").GetAttribute("href"),
                    DisPlayPrice = element.QuerySelector("b").TextContent,
                    HouseLocation = new[] { "公寓", "青年社区" }.All(s => houseInfoList.Contains(s)) ? houseInfoList[0] : houseInfoList[1]
                };
            });
        }

        private IEnumerable<HouseInfo> GetRoomList(int costFrom, int costTo, string cnName, int index)
        {
            var url = $"http://{cnName}.58.com/pinpaigongyu/pn/{index}/?minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var page = new HtmlParser().Parse(htmlResult);
            return page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr")).Select(element =>
            {
                var houseTitle = element.QuerySelector("h2").TextContent;
                var houseInfoList = houseTitle.Split(' ');
                return new HouseInfo
                {
                    HouseTitle = houseTitle,
                    HouseOnlineURL = $"http://{cnName}.58.com" + element.QuerySelector("a").GetAttribute("href"),
                    DisPlayPrice = element.QuerySelector("b").TextContent,
                    HouseLocation = new[] { "公寓", "青年社区" }.All(s => houseInfoList.Contains(s)) ? houseInfoList[0] : houseInfoList[1]
                };
            });
        }

        private int GetListSum(int costFrom, int costTo, string cnName)
        {
            var url = $"http://{cnName}.58.com/pinpaigongyu/pn/{1}/?minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var dom = new HtmlParser().Parse(htmlResult);
            var countNode = dom.GetElementsByClassName("listsum").FirstOrDefault()?.QuerySelector("em");
            return Convert.ToInt32((countNode?.TextContent) ?? "0");
        }

    }
}

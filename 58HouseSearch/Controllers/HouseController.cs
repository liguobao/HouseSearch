using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _58HouseSearch.Models;
using AngleSharp.Parser.Html;


namespace _58HouseSearch.Controllers
{
    public class HouseController : Controller
    {
        //
        // GET: /House/
        public ActionResult Index()
        {
            HTTPHelper.WritePVInfo(Server.MapPath("./pv.json"), Request.UserHostAddress, Request.Path);
            return View();
        }

       

        public ActionResult Get58CityRoomData(int costFrom, int costTo, string cnName)
        {
            if (costTo<=0 || costTo < costFrom)
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
                return Json(new { IsSuccess = false, Error = "获取数据异常。" + ex.ToString() });
            }
        }

        private IEnumerable<HouseInfo> GetRoomList(int costFrom, int costTo, string cnName,int index)
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
                    HouseURL = $"http://{cnName}.58.com" + element.QuerySelector("a").GetAttribute("href"),
                    Money = element.QuerySelector("b").TextContent,
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
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
           

            var pageCount = GetPageNum(costFrom, costTo, cnName);
            if(pageCount == 0)
                return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房子。" });

            var roomList = Enumerable.Range(1, pageCount).Select(index => GetRoomList(costFrom, costTo, cnName, index)).Aggregate((a, b) => a.Concat(b));
            return Json(new { IsSuccess = true, HouseInfos = roomList });

         

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
            List<HouseInfo> houseList = new List<HouseInfo>();
            foreach (var element in page.QuerySelectorAll("p").Where(element => element.ClassName == "qj-renaddr"))
            {
                var houseHref = string.IsNullOrEmpty(element.Children[1].GetAttribute("href")) ? element.Children[1].GetAttribute("href") :
                   element.Children[0].GetAttribute("href");
                if (string.IsNullOrEmpty(houseHref))
                    continue;

                houseList.Add(new HouseInfo
                {
                    HouseTitle = element.Children[0].TextContent,
                    HouseURL = houseHref + $"?isreal=true&minprice={costFrom}_{costTo}",
                    Money = "0",
                    HouseLocation = element.Children[1].TextContent.Replace("租房", "")
                });

            }
            return houseList;

        }



        public int GetPageNum(int costFrom, int costTo, string cnName)
        {
            var url = $"http://{cnName}.58.com/zufang/pn1/?isreal=true&minprice={costFrom}_{costTo}";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var dom = new HtmlParser().Parse(htmlResult);
            var pagerList = dom.GetElementsByClassName("pager").FirstOrDefault()?.Children.Where(children => children.ClassName == null);
            if (pagerList != null)
            {
                var pageNums = pagerList.Select(page => Convert.ToInt32(page?.QuerySelector("span").InnerHtml));

                return pageNums != null ? pageNums.Max() : 0;
            }
            return 0;
        }

    }
}

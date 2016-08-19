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

            var url = $"http://sh.58.com/zufang/pn1/?isreal=true&minprice=1000_2000";
            var htmlResult = HTTPHelper.GetHTMLByURL(url);
            var dom = new HtmlParser().Parse(htmlResult);


            var lst = dom.QuerySelectorAll("p").Where(element => element.ClassName == "qj-renaddr");

            var houses = dom.QuerySelectorAll("p").Where(element => element.ClassName == "qj-renaddr").Select(element =>
            {

                return new HouseInfo
                {
                    HouseTitle = element.Children[1].TextContent,
                    HouseURL = element.Children[0].GetAttribute("href"),
                    Money = "0",
                    HouseLocation = element.Children[1].TextContent.Replace("租房", "")
                };
            });

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
           

            var pageNum = GetPageNum(costFrom, costTo, cnName);
            if(pageNum==0)
                return Json(new { IsSuccess = false, Error = $"没有找到价格区间为{costFrom} - {costTo}的房子。" });



            return Json(new { IsSuccess = true });

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
            return page.QuerySelectorAll("p").Where(element => element.ClassName== "qj-renaddr").Select(element =>
            {
               
                return new HouseInfo
                {
                    HouseTitle = element.Children[1].TextContent,
                    HouseURL = element.Children[0].GetAttribute("href"),
                    Money = "0",
                    HouseLocation = element.Children[1].TextContent.Replace("租房","")
                };
            });
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

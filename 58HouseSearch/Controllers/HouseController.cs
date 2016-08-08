using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _58HouseSearch.Models;
using HtmlAgilityPack;

namespace _58HouseSearch.Controllers
{
    public class HouseController : Controller
    {
        //
        // GET: /House/
        public ActionResult Index()
        {
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
                var lstHouse = new List<HouseInfo>();

                string tempURL = "http://" + cnName + ".58.com/pinpaigongyu//pn/{0}/?minprice=" + costFrom + "_" + costTo;

                Uri uri = new Uri(tempURL);

                var htmlResult = HTTPHelper.GetHTMLByURL(string.Format(tempURL, 1));

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlResult);

                var countNodes = htmlDoc.DocumentNode.SelectSingleNode(".//span[contains(@class,'list')]");
                int pageCount = 10;

                if (countNodes != null && countNodes.HasChildNodes)
                {
                    pageCount = Convert.ToInt32(countNodes.ChildNodes[0].InnerText) / 20;

                    if(pageCount==0)
                    {
                        return Json(new { IsSuccess = false, Error =string.Format("没有找到价格区间为{0} - {1}的房子。",costFrom,costTo)});
                    }
                    
                }
                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    htmlResult = HTTPHelper.GetHTMLByURL(string.Format(tempURL, pageIndex));
                    htmlDoc.LoadHtml(htmlResult);
                    var roomList = htmlDoc.DocumentNode.SelectNodes(".//a[contains(@tongji_label,'listclick')]");
                    foreach (var room in roomList)
                    {
                        var houseTitle = room.SelectSingleNode(".//h2").InnerHtml;
                        var houseURL = uri.Host + room.Attributes["href"].Value;
                        var house_info_list = houseTitle.Split(' ');
                        var house_location = string.Empty;
                        if (house_info_list[1].Contains("公寓") || house_info_list[1].Contains("青年社区"))
                        {
                            house_location = house_info_list[0];
                        }
                        else
                        {
                            house_location = house_info_list[1];
                        }
                        var momey = room.SelectSingleNode(".//b").InnerHtml;

                        lstHouse.Add(new HouseInfo()
                        {
                            HouseTitle = houseTitle,
                            HouseLocation = house_location,
                            HouseURL = houseURL,
                            Money = momey,
                        });
                    }
                }

                return Json(new { IsSuccess = true, HouseInfos = lstHouse });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Error = "获取数据异常。" + ex.ToString() });
            }





        }
       

        
	}
}
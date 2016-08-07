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



        public ActionResult Get58CityRoomData(string dataURL)
        {
            if (string.IsNullOrEmpty(dataURL))
            {
                return Json(new { IsSuccess = false, Error = "URL不能为空。" });
            }

            try
            {
                var lstHouse = new List<HouseInfo>();

                string tempURL = dataURL + "/pn/{0}/?minprice=2000_4000";

                Uri uri = new Uri(dataURL);

                var htmlResult = HTTPHelper.GetHTMLByURL(string.Format(tempURL, 1));

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlResult);

                var countNodes = htmlDoc.DocumentNode.SelectSingleNode(".//span[contains(@class,'list')]");
                int pageCount = 10;

                if (countNodes != null && countNodes.HasChildNodes)
                {
                    pageCount = Convert.ToInt32(countNodes.ChildNodes[0].InnerText) / 20;
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
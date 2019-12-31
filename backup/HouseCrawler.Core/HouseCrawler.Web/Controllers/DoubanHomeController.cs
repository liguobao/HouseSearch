using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseCrawler.Web.Models;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.Controllers
{
    public class DoubanHomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string groupID="",int endstart= 0)
        {
           
            return View();
        }


        public IActionResult GetDoubanHouseInfo(string groupID="",int index=0)
        {
            if (string.IsNullOrEmpty(groupID))
                groupID = "shanghaizufang";
             var houses = DoubanHouseCrawler.GetHouseData(groupID, "", index);
            var lstRoomInfo = houses.Select(item => new HouseInfo()
            {
                HouseTitle = item.HouseTitle,
                HouseOnlineURL = item.HouseOnlineURL,
                HouseLocation = item.HouseTitle,
                DisPlayPrice ="暂无"
            });
            return Json(new { IsSuccess = true, HouseInfos = lstRoomInfo });

         
        }

        public IActionResult GetDoubanCity(string groupID = "")
        {
            if (string.IsNullOrEmpty(groupID))
                groupID = "shanghaizufang";
            var url = $"https://www.douban.com/group/{groupID}/";
            var htmlResult = HTTPHelper.GetHTML(url);
            return Json(new { IsSuccess = true,cityName ="上海" });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseCrawler.Core.Models;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Core.Controllers
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
            var url = $"https://www.douban.com/group/{groupID}/discussion?start={index*25}";
            var htmlResult = DoubanHTTPHelper.GetHTMLForDouban(url);
            var page = new HtmlParser().Parse(htmlResult);
            var lstRoomInfo = page.QuerySelectorAll("td.title").Select(item => new HouseInfo()
            {
                HouseTitle = item.QuerySelector("a").GetAttribute("title"),
                HouseURL = item.QuerySelector("a").GetAttribute("href"),
                HouseLocation = item.QuerySelector("a").GetAttribute("title"),
                Money ="暂无"
            });
            return Json(new { IsSuccess = true, HouseInfos = lstRoomInfo });

         
        }

        public IActionResult GetDoubanCity(string groupID = "")
        {
            if (string.IsNullOrEmpty(groupID))
                groupID = "shanghaizufang";
            var url = $"https://www.douban.com/group/{groupID}/";
            var htmlResult = DoubanHTTPHelper.GetHTMLForDouban(url);
            return Json(new { IsSuccess = true,cityName ="上海" });
        }
    }
}

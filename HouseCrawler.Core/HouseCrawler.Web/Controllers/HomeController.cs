using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using HouseCrawler.Web;
using HouseCrawler.Web.DAL;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {
        private CrawlerDataContent dataContent = new CrawlerDataContent();

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(HouseSourceInfo.LoadCityHouseInfo());
        }

        public IActionResult HouseList()
        {
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string source="", int houseCount = 100,int withinAnyDays = 3)
        {
            try
            {
                var lstHouseInfo = new DBHouseInfoDAL().SearchHouseInfo(cityName, source, houseCount, withinAnyDays);
                var lstRoomInfo = lstHouseInfo.Select(house =>
                {
                    var markBGType = string.Empty;
                    int housePrice = (int)house.HousePrice;
                    if (housePrice > 0)
                    {
                        markBGType = LocationMarkBGType.SelectColor(housePrice / 1000);
                    }

                    return new HouseInfo
                    {
                        Money = house.DisPlayPrice,
                        HouseURL = house.HouseOnlineURL,
                        HouseLocation = house.HouseLocation,
                        HouseTime = house.PubTime.ToString(),
                        HousePrice = housePrice,
                        LocationMarkBG = markBGType,
                        DisplaySource = ConstConfigurationName.ConvertToDisPlayName(house.Source)
                    };
                });
                 return Json(new { IsSuccess = true, HouseInfos = lstRoomInfo });
            }catch(Exception ex)
            {
                return Json(new { IsSuccess = false, error=ex.ToString() });
            }
           


        }


        public IActionResult AddDouBanGroup(string doubanGroup,string cityName)
        {
            if(string.IsNullOrEmpty(doubanGroup) || string.IsNullOrEmpty(cityName))
            {
                return Json(new { IsSuccess = false , error="请输入豆瓣小组Group和城市名称。" });
            }
            DoubanHouseCrawler.AddDoubanGroupConfig(doubanGroup, cityName);
            return Json(new { IsSuccess = true });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {
        private CrawlerDataContent dataContent = new CrawlerDataContent();

        // GET: /<controller>/
        public IActionResult Index()
        {
            //CityHouseInfo.RefashCityHouseInfo();
            return View(CityHouseInfo.LoadCityHouseInfo());
        }

        public IActionResult HouseList()
        {
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string sourece="", int count = 100)
        {
            var houses = dataContent.HouseInfos.Where(h => h.LocationCityName == cityName);
            if (!string.IsNullOrEmpty(sourece))
            {
                houses = houses.Where(h=>h.Source == sourece);
            }
            var lstHouseInfo = houses.OrderByDescending(h => h.PubTime).Take(count).ToList();

            var lstRoomInfo = lstHouseInfo.Select(house=> 
            {
                int housePrice = (int) house.HousePrice;
                var markBGType = LocationMarkBGType.SelectColor(housePrice / 1000);
                return new HouseInfo
                {
                    Money = house.DisPlayPrice,
                    HouseURL =house.HouseOnlineURL,
                    HouseLocation = house.HouseLocation,
                    HouseTime = house.PubTime.ToString(),
                    HousePrice = housePrice,
                    LocationMarkBG = markBGType,
                };
            });
            return Json(new { IsSuccess = true, HouseInfos = lstRoomInfo });


        }


    }
}

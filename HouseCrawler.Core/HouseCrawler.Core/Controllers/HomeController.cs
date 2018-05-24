using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Jobs;
using StackExchange.Redis;
using HouseCrawler.Core.DataContent;
using System;
using HouseCrawler.Core.Common;
using HouseCrawler.Core.Service;
using System.Linq;

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrawlerDataContent _dataContent = new CrawlerDataContent();

        private HouseDashboardJob _HouseDashboardJob;

        public HomeController(HouseDashboardJob houseDashboardJob)
        {
            _HouseDashboardJob = houseDashboardJob;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(HouseDashboardService.LoadDashboard());
        }

        public IActionResult HouseList()
        {
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string source = "", int houseCount = 500,
            int withAnyDays = 7, string keyword = "")
        {

            var houses = CrawlerDataDapper.SearchHouseInfo(cityName, source, houseCount, withAnyDays, keyword);
            var rooms = houses.Select(house =>
                {
                    var markBGType = string.Empty;
                    int housePrice = (int)house.HousePrice;
                    if (housePrice > 0)
                    {
                        markBGType = LocationMarkBGType.SelectColor(housePrice / 1000);
                    }

                    return new HouseInfo
                    {
                        Source = house.Source,
                        Money = house.DisPlayPrice,
                        HouseURL = house.HouseOnlineURL,
                        HouseLocation = house.HouseLocation,
                        HouseTime = house.PubTime.ToString(),
                        HouseTitle = house.HouseTitle,
                        HousePrice = housePrice,
                        LocationMarkBG = markBGType,
                        DisplaySource = ConstConfigurationName.ConvertToDisPlayName(house.Source)
                    };
                });
            return Json(new { IsSuccess = true, HouseInfos = rooms });

        }


        public IActionResult RunJobs()
        {
            DoubanHouseCrawler.Run();
            PinPaiGongYuHouseCrawler.Run();
            PeopleRentingCrawler.Run();
            CCBHouesCrawler.Run();
            ZuberHouseCrawler.Run();
            MoGuHouseCrawler.Run();
            return View();
        }

        public IActionResult RunZuber()
        {
            ZuberHouseCrawler.Run();
            MoGuHouseCrawler.Run();
            return View();
        }

        public IActionResult RunHK()
        {
            HKSpaciousCrawler.Run();
            return Json(new { success = true });
        }


        public IActionResult RunStatJob()
        {
            _HouseDashboardJob.Run();
            return Json(new { success = true });
        }


        public IActionResult StartGC()
        {
            new GCJob().Run();
            return Json(new { isSuccess = true });
        }
    }
}

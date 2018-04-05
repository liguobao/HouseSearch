using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Jobs;
using HouseCrawler.Core.Crawlers;

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrawlerDataContent _dataContent = new CrawlerDataContent();

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(HouseSourceInfo.LoadCityHouseInfo());
        }

        public IActionResult HouseList()
        {
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string source = "", int houseCount = 400, 
            int withAnyDays = 3,string keyword ="")
        {
            return Json(new { IsSuccess = true});
        }


        public IActionResult RunJobs()
        {
             DoubanHouseCrawler.CaptureHouseInfo();
            // PinPaiGongYuHouseCrawler.CapturPinPaiHouseInfo();
             //PeopleRentingCrawler.CapturHouseInfo();

             //CCBHouesCrawler.CaptureHouseInfo();
            return View();
        }


        public IActionResult StartGC()
        {
            new GCJob().Run();
            return Json(new { isSuccess = true });
        }
    }
}

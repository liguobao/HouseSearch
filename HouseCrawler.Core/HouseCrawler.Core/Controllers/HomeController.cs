using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Jobs;

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrawlerDataContent _dataContent = new CrawlerDataContent();

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(HouseDashboard.LoadCityDashboards());
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
             PinPaiGongYuHouseCrawler.CapturePinPaiHouseInfo();
             PeopleRentingCrawler.CaptureHouseInfo();
             CCBHouesCrawler.CaptureHouseInfo();
            return View();
        }


        public IActionResult StartGC()
        {
            new GCJob().Run();
            return Json(new { isSuccess = true });
        }
    }
}

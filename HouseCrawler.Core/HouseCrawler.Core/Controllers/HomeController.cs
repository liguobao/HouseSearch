using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;
using HouseCrawler.Core.Jobs;
using HouseCrawler.Web.DAL;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            var houses = new DBHouseInfoDAL().SearchHouseInfo(cityName, source, houseCount, withAnyDays, keyword);

            var lstRoomInfo = houses.Select(house =>
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
                    HouseTime = house.PubTime.ToString(CultureInfo.CurrentCulture),
                    HousePrice = housePrice,
                    LocationMarkBG = markBGType,
                    DisplaySource = ConstConfigurationName.ConvertToDisPlayName(house.Source)
                };
            });
            return Json(new { IsSuccess = true, HouseInfos = lstRoomInfo });


        }


        public IActionResult AddDouBanGroup(string doubanGroup, string cityName)
        {
            if (string.IsNullOrEmpty(doubanGroup) || string.IsNullOrEmpty(cityName))
            {
                return Json(new { IsSuccess = false, error = "请输入豆瓣小组Group和城市名称。" });
            }
            DoubanHouseCrawler.AddDoubanGroupConfig(doubanGroup, cityName);
            return Json(new { IsSuccess = true });
        }


        public IActionResult RunJobs()
        {



            DoubanHouseCrawler.CaptureHouseInfoFromConfig();
            HouseSourceInfo.RefreshHouseSourceInfo();
            PinPaiGongYuHouseCrawler.CapturPinPaiHouseInfo();

            //Task.Factory.StartNew(() =>
            //{
            //    try
            //    {

            //        PeopleRentingCrawler.CapturHouseInfo();
            //        DoubanHouseCrawler.CaptureHouseInfoFromConfig();
            //        DoubanHouseCrawler.AnalyzeDoubanHouseContentAll();
            //        HouseSourceInfo.RefreshHouseSourceInfo();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogHelper.Error("RunJobs", ex);
            //    }

            //});

            return View();
        }


        public IActionResult StartGC()
        {
            new GCJob().Run();
            return Json(new { isSuccess = true });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Jobs;
using StackExchange.Redis;
using System;
using HouseCrawler.Core.Common;
using HouseCrawler.Core.Service;
using System.Linq;
using System.Collections.Generic;

namespace HouseCrawler.Core.Controllers
{
    public class HomeController : Controller
    {

        private HouseDapper houseDapper;

        private TodayHouseDashboardJob houseDashboardJob;

        private SyncHousesToESJob syncHousesToESJob;

        private HouseDashboardService houseDashboardService;


        private PinPaiGongYuHouseCrawler pinpai;
        private PeopleRentingCrawler people;
        private DoubanHouseCrawler douban;
        private CCBHouesCrawler ccbHouse;
        private ZuberHouseCrawler zuber;
        private MoGuHouseCrawler mogu;
        private HKSpaciousCrawler hkSpacious;

        private BaiXingHouseCrawler baixing;
        ElasticsearchService elasticsearchService;

        private RefreshHouseCacheJob refreshHouseCacheJob;

        public HomeController(TodayHouseDashboardJob houseDashboardJob,
                              HouseDapper houseDapper,
                              HouseDashboardService houseDashboardService,
                              PinPaiGongYuHouseCrawler pinpai,
                              PeopleRentingCrawler people,
                              DoubanHouseCrawler douban,
                              CCBHouesCrawler ccbHouse,
                              ZuberHouseCrawler zuber,
                              MoGuHouseCrawler mogu,
                              HKSpaciousCrawler hkSpacious,
                              BaiXingHouseCrawler baixing,
                              SyncHousesToESJob syncHousesToESJob,
                              ElasticsearchService elasticsearchService,
                              RefreshHouseCacheJob refreshHouseCacheJob)
        {
            this.houseDashboardJob = houseDashboardJob;
            this.houseDapper = houseDapper;
            this.houseDashboardService = houseDashboardService;
            this.pinpai = pinpai;
            this.people = people;
            this.douban = douban;
            this.ccbHouse = ccbHouse;
            this.zuber = zuber;
            this.mogu = mogu;
            this.hkSpacious = hkSpacious;
            this.baixing = baixing;
            this.syncHousesToESJob = syncHousesToESJob;
            this.elasticsearchService = elasticsearchService;
            this.refreshHouseCacheJob = refreshHouseCacheJob;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(houseDashboardService.LoadDashboard());
        }

        public IActionResult HouseList()
        {
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string source = "", int houseCount = 500,
            int withAnyDays = 7, string keyword = "")
        {

            var houses = houseDapper.SearchHouses(new HouseSearchCondition()
            {
                CityName = cityName,
                HouseCount = houseCount,
                IntervalDay = withAnyDays,
                Keyword = keyword
            });
            return Json(new { IsSuccess = true, HouseInfos = houses });

        }


        public IActionResult RunJobs()
        {
            douban.Run();
            pinpai.Run();
            ccbHouse.Run();
            mogu.Run();
            return View();
        }

        public IActionResult RunZuber()
        {
            zuber.Run();
            return View();
        }

        public IActionResult RunHK()
        {
            hkSpacious.Run();
            return Json(new { success = true });
        }


        public IActionResult RunStatJob()
        {
            houseDashboardJob.Run();
            return Json(new { success = true });
        }



        public IActionResult StartGC()
        {
            new GCJob().Run();
            //syncHousesToESJob.Run();
            return Json(new { isSuccess = true });
        }


        public IActionResult RunSyncHouse(string datetime)
        {
            DateTime pubTime = DateTime.Parse(datetime);
            var houses = houseDapper.QueryByTimeSpan(pubTime, pubTime.AddHours(24));
            elasticsearchService.SaveHousesToES(houses);
            return Json(new { success = true });
        }

        public IActionResult RefreshHouseCache()
        {
            refreshHouseCacheJob.Run();
            return Json(new { success = true });
        }
    }
}

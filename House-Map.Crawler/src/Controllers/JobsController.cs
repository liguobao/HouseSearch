using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler;
using HouseMap.Crawler.Service;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.Jobs;
using HouseMap.Common;

namespace HouseMap.Crawler.Controllers
{
    public class JobsController : Controller
    {

        private HouseDapper houseDapper;

        private TodayHouseDashboardJob houseDashboardJob;



        private PinPaiGongYuHouseCrawler pinpai;
        private PeopleRentingCrawler people;
        private DoubanHouseCrawler douban;
        private CCBHouseCrawler ccbHouse;
        private ZuberHouseCrawler zuber;
        private MoGuHouseCrawler mogu;

        private BeikeHouseCrawler beike;
        private HKSpaciousCrawler hkSpacious;

        private BaiXingHouseCrawler baixing;

        private ChengduZufangCrawler chengdu;

        private RefreshHouseCacheJob refreshHouseCacheJob;

        private RefreshHouseSourceJob refreshHouseSourceJob;

        public JobsController(TodayHouseDashboardJob houseDashboardJob,
                              HouseDapper houseDapper,
                              PinPaiGongYuHouseCrawler pinpai,
                              PeopleRentingCrawler people,
                              DoubanHouseCrawler douban,
                              CCBHouseCrawler ccbHouse,
                              ZuberHouseCrawler zuber,
                              MoGuHouseCrawler mogu,
                              HKSpaciousCrawler hkSpacious,
                              BaiXingHouseCrawler baixing,
                              ChengduZufangCrawler chengdu,
                              RefreshHouseCacheJob refreshHouseCacheJob,
                              RefreshHouseSourceJob refreshHouseSourceJob,
                              BeikeHouseCrawler beike)
        {
            this.houseDashboardJob = houseDashboardJob;
            this.houseDapper = houseDapper;
            this.pinpai = pinpai;
            this.people = people;
            this.douban = douban;
            this.ccbHouse = ccbHouse;
            this.zuber = zuber;
            this.mogu = mogu;
            this.hkSpacious = hkSpacious;
            this.baixing = baixing;
            this.refreshHouseCacheJob = refreshHouseCacheJob;
            this.refreshHouseSourceJob = refreshHouseSourceJob;
            this.beike = beike;
            this.chengdu = chengdu;
        }


        public IActionResult Index(string source)
        {

            try
            {
                LogHelper.RunActionTaskNotThrowEx(() =>
                {
                    switch (source)
                    {
                        case "baixing":
                            baixing.Run();
                            break;
                        case "douban":
                            douban.Run();
                            break;
                        case "zuber":
                            zuber.Run();
                            break;
                        case "huzhuzufang":
                            people.Run();
                            break;
                        case "ccbhouse":
                            ccbHouse.Run();
                            break;
                        case "mogu":
                            mogu.Run();
                            break;
                        case "hkspacious":
                            hkSpacious.Run();
                            break;
                        case "pinpaigongyu":
                            pinpai.Run();
                            break;
                        case "beike":
                            beike.Run();
                            break;
                        case "chengdufgj":
                            chengdu.Run();
                            break;
                    }
                }, source);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = true, error = ex.ToString() });
            }

        }



        public IActionResult TodayDashboard()
        {
            LogHelper.RunActionTaskNotThrowEx(houseDashboardJob.Run, "TodayDashboard");
            return Json(new { success = true });
        }

        public IActionResult RefreshHouseCache()
        {
            LogHelper.RunActionTaskNotThrowEx(refreshHouseCacheJob.Run, "RefreshHouseCache");
            return Json(new { success = true });
        }

        public IActionResult RefreshHouseSource()
        {
            LogHelper.RunActionTaskNotThrowEx(refreshHouseSourceJob.Run, "RefreshHouseSource");
            return Json(new { success = true });
        }

        public IActionResult InitBeike()
        {
            beike.InitCityData();
            return Json(new { success = true });
        }
    }
}

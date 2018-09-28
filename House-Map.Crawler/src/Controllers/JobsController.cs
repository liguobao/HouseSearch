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
using Microsoft.Extensions.DependencyInjection;

namespace HouseMap.Crawler.Controllers
{
    public class JobsController : Controller
    {

        private readonly TodayHouseDashboardJob _dashboardJob;


        private readonly IServiceProvider _serviceProvider;

        private readonly RefreshHouseCacheJob _refreshHouseCacheJob;

        private readonly RefreshHouseSourceJob _refreshHouseSourceJob;



        public JobsController(TodayHouseDashboardJob houseDashboardJob,
                              RefreshHouseCacheJob refreshHouseCacheJob,
                              RefreshHouseSourceJob refreshHouseSourceJob,
                             IServiceProvider serviceProvider)
        {
            _dashboardJob = houseDashboardJob;
            _refreshHouseCacheJob = refreshHouseCacheJob;
            _refreshHouseSourceJob = refreshHouseSourceJob;
            _serviceProvider = serviceProvider;
        }


        public IActionResult Index(string source)
        {

            try
            {
                var crawler = _serviceProvider.GetServices<ICrawler>().FirstOrDefault(c => c.GetSource() == source);
                LogHelper.RunActionTaskNotThrowEx(() =>
                {
                    if (crawler != null)
                    {
                        crawler.Run();
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
            LogHelper.RunActionTaskNotThrowEx(_dashboardJob.Run, "TodayDashboard");
            return Json(new { success = true });
        }

        public IActionResult RefreshHouseCache()
        {
            LogHelper.RunActionTaskNotThrowEx(_refreshHouseCacheJob.Run, "RefreshHouseCache");
            return Json(new { success = true });
        }

        public IActionResult RefreshHouseSource()
        {
            LogHelper.RunActionTaskNotThrowEx(_refreshHouseSourceJob.Run, "RefreshHouseSource");
            return Json(new { success = true });
        }

        public IActionResult InitBeike()
        {
            return Json(new { success = true });
        }
    }
}

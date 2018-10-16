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
using HouseMap.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HouseMap.Crawler.Controllers
{
    public class JobsController : Controller
    {

        private readonly IServiceProvider _serviceProvider;


        public JobsController(

                             IServiceProvider serviceProvider)
        {
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


        public IActionResult RefreshHouseCache()
        {
            var houseService = _serviceProvider.GetServices<HouseService>().FirstOrDefault();
            LogHelper.RunActionTaskNotThrowEx(houseService.RefreshHouse, "RefreshHouseCache");
            return Json(new { success = true });
        }


        public IActionResult RefreshHouseCacheV2()
        {
            var houseService = _serviceProvider.GetServices<HouseService>().FirstOrDefault();
            LogHelper.RunActionTaskNotThrowEx(houseService.RefreshHouseV2, "RefreshHouseCacheV2");
            return Json(new { success = true });
        }

    }
}

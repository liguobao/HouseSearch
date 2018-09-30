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
    public class CrawlerController : Controller
    {
        private readonly IServiceProvider _serviceProvider;

        public CrawlerController(
                             IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IActionResult Index(string source)
        {

            try
            {
                var crawler = _serviceProvider.GetServices<INewCrawler>().FirstOrDefault(c => 
                c.GetSource().GetSourceName() == source);
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

       
    }
}

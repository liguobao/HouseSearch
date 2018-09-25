using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using HouseMap.Crawler.Dapper;
using HouseMap.Crawler;
using HouseMap.Crawler.Service;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.Jobs;

namespace HouseMap.Crawler.Controllers
{
    public class TestController : Controller
    {
        private ZuberCrawler _zuber;

        public TestController(ZuberCrawler zuber)
        {
            _zuber = zuber;
        }

        public IActionResult Index(string source)
        {
            _zuber.Run();
            return Json(new { success = true });
        }

    }
}
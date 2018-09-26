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

namespace HouseMap.Crawler.Controllers
{
    public class TestController : Controller
    {
        private ZuberCrawler _zuber;

        private HouseDataContext _context;


        public TestController(ZuberCrawler zuber, HouseDataContext context)
        {
            _zuber = zuber;
            _context = context;
        }

        public IActionResult Index(string source)
        {

            return Json(new { success = true });
        }

    }
}
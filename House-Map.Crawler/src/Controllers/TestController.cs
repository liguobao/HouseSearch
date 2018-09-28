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
using Newtonsoft.Json.Linq;
using HouseMap.Common;

namespace HouseMap.Crawler.Controllers
{
    public class TestController : Controller
    {
        private ZuberCrawler _zuber;

        private readonly ConfigDapper _configDapper;

        private HouseDataContext _context;


        public TestController(ZuberCrawler zuber, HouseDataContext context,
        ConfigDapper configDapper)
        {
            _zuber = zuber;
            _context = context;
            _configDapper = configDapper;
        }

        public IActionResult Index(string source)
        {
            // if (source != "init")
            // {
            //     var configs = _configDapper.FindAll();
            //     foreach (var config in configs)
            //     {
            //         var dbConfig = new DbConfig();
            //         var configJson = JToken.Parse(config.ConfigurationValue);
            //         dbConfig.City = configJson["cityname"] != null
            //             ? configJson["cityname"].ToString()
            //             : configJson["cityName"].ToString();
            //         dbConfig.Source = config.ConfigurationName;
            //         dbConfig.Json = config.ConfigurationValue;
            //         dbConfig.PageCount = configJson["pagecount"] != null ? configJson["pagecount"].ToObject<int>() : 20;
            //         dbConfig.CreateTime = DateTime.Now;
            //         dbConfig.Id = Tools.GetUUId();
            //         _context.Configs.Add(dbConfig);
            //     }
            //     _context.SaveChanges();
            // }
            return Json(new { success = true });
        }

    }
}
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
using Newtonsoft.Json.Linq;
using HouseMap.Common;
using SkyWalking.Context;
using SkyWalking.Context.Trace;
using SkyWalking.NetworkProtocol.Trace;
using SkyWalking.Context.Tag;
using Microsoft.Extensions.Caching.Distributed;

namespace HouseMap.Crawler.Controllers
{
    public class TestController : Controller
    {
         private readonly RedisTool _redis;

        private ZuberCrawler _zuber;

        private HouseDataContext _context;


        public TestController(ZuberCrawler zuber, HouseDataContext context,RedisTool redis)
        {
            _zuber = zuber;
            _context = context;
            _redis = redis;
        }

        public IActionResult Index(string source)
        {
            var data = _redis.ReadCache("user_173",0);
            var peer = "Action";
            var span = ContextManager.CreateExitSpan("TestController", peer);
            span.SetLayer(SpanLayer.CACHE);
            span.SetComponent(ComponentsDefine.AspNetCore);
            Tags.DbType.Set(span, "TestController");
            Tags.Url.Set(span, HttpContext.Request.QueryString.ToString());

            ContextManager.StopSpan();

            return Json(new { success = true });
        }

    }
}
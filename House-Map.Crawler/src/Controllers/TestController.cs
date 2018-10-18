using Microsoft.AspNetCore.Mvc;
using HouseMap.Dao;
using HouseMap.Common;

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
            return Json(new { success = true });
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using HouseMap.Dao;
using HouseMap.Common;

namespace HouseMap.Crawler.Controllers
{
    public class TestController : Controller
    {
         private readonly RedisTool _redis;


        private HouseMapContext _context;


        public TestController(HouseMapContext context,RedisTool redis)
        {
            _context = context;
            _redis = redis;
        }

        public IActionResult Index(string source)
        {
            return Json(new { success = true });
        }

    }
}
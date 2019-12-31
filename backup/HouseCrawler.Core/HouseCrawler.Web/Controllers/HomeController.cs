using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HouseCrawler.Web.Models;
using HouseCrawler.Web.Common;
using HouseCrawler.Web;
using HouseCrawler.Web.Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.Controllers
{
    public class HomeController : Controller
    {
        private HouseDapper _houseDapper;

        private HouseDashboardService _houseDashboardService;

        private ConfigDapper _configurationDapper;

        private UserCollectionDapper _userCollectionDapper;

        private UserService _useService;


        public HomeController(HouseDapper houseDapper,
                              HouseDashboardService houseDashboardService,
                              ConfigDapper configurationDapper,
                              UserCollectionDapper userCollectionDapper,
                              UserService useService)
        {
            _houseDapper = houseDapper;
            _houseDashboardService = houseDashboardService;
            _configurationDapper = configurationDapper;
            _userCollectionDapper = userCollectionDapper;
            _useService = useService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var idAndName = GetUserIDAndName();
            ViewBag.UserId = idAndName.Item1;
            ViewBag.UserName = idAndName.Item2;
            var dashboards = _houseDashboardService.LoadDashboard();
            return View(dashboards);
        }

        public IActionResult HouseList()
        {
            var token = HttpContext.Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }

            var userInfo = _useService.GetUserByToken(token);
            if (userInfo == null)
            {
                return View();
            }
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                                 {
                    new Claim(ClaimTypes.Name, userInfo.UserName),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.NameIdentifier, userInfo.ID.ToString())
                    }, CookieAuthenticationDefaults.AuthenticationScheme));
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)) // 有效时间
            }).Wait();
            return View();
        }

        public IActionResult GetHouseInfo(string cityName, string source = "", int houseCount = 600,
            int intervalDay = 14, string keyword = "", bool refresh = false, int page = 0)
        {
            try
            {
                var searchCondition = new HouseSearchCondition()
                {
                    CityName = cityName,
                    Source = source,
                    HouseCount = houseCount,
                    IntervalDay = intervalDay,
                    Keyword = keyword,
                    Page = page,
                    Refresh = refresh
                };
                var houseList = _houseDapper.SearchHouses(searchCondition);
                return Json(new { IsSuccess = true, HouseInfos = houseList });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, error = ex.ToString() });
            }
        }

        [HttpPost]
        [EnableCors("APICors")]
        public IActionResult Houses([FromBody] HouseSearchCondition search)
        {
            try
            {
                if (search == null || search.CityName == null)
                {
                    return Json(new { IsSuccess = false, error = "查询条件不能为null" });
                }
                var houseList = _houseDapper.SearchHouses(search);
                return Json(new { IsSuccess = true, HouseInfos = houseList });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, error = ex.ToString() });
            }
        }



        [EnableCors("APICors")]
        public IActionResult Houses(string cityName, string source = "", int houseCount = 600,
                    int intervalDay = 14, string keyword = "", bool refresh = false, int page = 0)
        {
            try
            {
                var searchCondition = new HouseSearchCondition()
                {
                    CityName = cityName,
                    Source = source,
                    HouseCount = houseCount,
                    IntervalDay = intervalDay,
                    Keyword = keyword,
                    Page = page,
                    Refresh = refresh
                };
                var houseList = _houseDapper.SearchHouses(searchCondition);
                return Json(new { success = true, houses = houseList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.ToString() });
            }
        }

        [EnableCors("APICors")]
        public IActionResult Dashboards()
        {
            var dashboards = _houseDashboardService.LoadDashboard();
            return Json(new { success = true, dashboards = dashboards });
        }


        public IActionResult AddDouBanGroup(string doubanGroup, string cityName)
        {
            if (string.IsNullOrEmpty(doubanGroup) || string.IsNullOrEmpty(cityName))
            {
                return Json(new { IsSuccess = false, error = "请输入豆瓣小组Group和城市名称。" });
            }
            var topics = DoubanHouseCrawler.GetHouseData(doubanGroup, cityName, 1);
            if (topics != null && topics.Count() > 0)
            {
                var cityInfo = $"{{ 'groupid':'{doubanGroup}','cityname':'{cityName}','pagecount':5}}";
                var doubanConfig = new CrawlerConfiguration();
                if (doubanConfig != null)
                {
                    return Json(new { IsSuccess = true });
                }
                var config = new CrawlerConfiguration()
                {
                    ConfigurationKey = 0,
                    ConfigurationValue = cityInfo,
                    ConfigurationName = ConstConfigName.Douban,
                    DataCreateTime = DateTime.Now,
                    IsEnabled = true,
                };
                _configurationDapper.Insert(config);
                return Json(new { IsSuccess = true });
            }
            else
            {
                return Json(new
                {
                    IsSuccess = false,
                    error = "保存失败!请检查豆瓣小组ID（如：XMhouse）/城市名称（如：厦门）是否正确..."
                });
            }



        }


        public IActionResult AddUserCollection(long houseId, string source)
        {
            var userID = GetUserID();
            if (userID == 0)
            {
                return Json(new { IsSuccess = false, error = "用户未登陆，无法进行操作。" });
            }
            var house = _houseDapper.GetHouseID(houseId, source);
            if (house == null)
            {
                return Json(new { successs = false, error = "房源信息不存在,请刷新页面后重试." });
            }
            var userCollection = new UserCollection();
            userCollection.UserID = userID;
            userCollection.HouseID = houseId;
            userCollection.Source = house.Source;
            userCollection.HouseCity = house.LocationCityName;
            _userCollectionDapper.InsertUser(userCollection);
            return Json(new { success = true, message = "收藏成功." }); ;
        }


        public IActionResult RemoveUserCollection(long id)
        {
            var userID = GetUserID();
            if (userID == 0)
            {
                return Json(new { IsSuccess = false, error = "用户未登陆，无法进行操作。" });
            }
            var userCollection = _userCollectionDapper.FindByIDAndUserID(id, userID);
            if (userCollection == null)
            {
                return Json(new { successs = false, error = "房源信息不存在,请刷新页面后重试." });
            }
            try
            {
                _userCollectionDapper.RemoveByIDAndUserID(id, userID);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.ToString() }); ;
            }
            return Json(new { success = true, message = "删除成功." }); ;
        }


        public IActionResult GetUserCollectionHouses(string cityName, string source = "")
        {
            try
            {
                var userID = GetUserID();
                if (userID == 0)
                {
                    return Json(new { IsSuccess = false, error = "用户未登陆，无法查看房源收藏。" });
                }
                var rooms = _userCollectionDapper.FindUserCollections(userID, cityName, source);
                return Json(new { IsSuccess = true, HouseInfos = rooms });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, error = ex.ToString() });
            }
        }

        private long GetUserID()
        {
            var identity = ((ClaimsIdentity)HttpContext.User.Identity);
            if (identity == null || identity.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return 0;
            }
            var userID = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(userID) || userID == "0")
            {
                return 0;
            }
            return long.Parse(userID);
        }

        public Tuple<long, string> GetUserIDAndName()
        {
            var identity = ((ClaimsIdentity)HttpContext.User.Identity);
            if (identity == null || identity.FindFirst(ClaimTypes.NameIdentifier) == null)
            {
                return Tuple.Create<long, string>(0, string.Empty);
            }
            var userID = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userName = identity.FindFirst(ClaimTypes.Name).Value;
            if (string.IsNullOrEmpty(userID) || userID == "0")
            {
                return Tuple.Create<long, string>(0, string.Empty);
            }
            return Tuple.Create<long, string>(long.Parse(userID), userName);
        }

        public IActionResult UserCollection()
        {
            return View();
        }

        public IActionResult UserHouseDashboard()
        {
            var houseDashboards = new List<HouseDashboard>();
            var userID = GetUserID();
            if (userID == 0)
            {
                return PartialView("UserHouseDashboard", houseDashboards);
            }
            houseDashboards = _userCollectionDapper.LoadUserHouseDashboard(userID);
            return PartialView("UserHouseDashboard", houseDashboards);
        }


        public IActionResult UserHouseList()
        {
            var userHouses = new List<HouseInfo>();
            var userID = GetUserID();
            if (userID == 0)
            {
                return PartialView("UserHouseList", userHouses);
            }
            userHouses = _userCollectionDapper.FindUserCollections(userID);
            return PartialView("UserHouseList", userHouses);
        }
    }
}

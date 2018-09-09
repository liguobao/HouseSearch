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
using Talk.OAuthClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v1/users/")]
    public class CollectionsController : ControllerBase
    {


        private HouseDapper houseDapper;

        private HouseDashboardService houseDashboardService;

        private ConfigDapper configurationDapper;

        private UserCollectionDapper userCollectionDapper;
        private UserService userService;


        public CollectionsController(HouseDapper houseDapper,
                              HouseDashboardService houseDashboardService,
                              ConfigDapper configurationDapper,
                              UserCollectionDapper userCollectionDapper,
                              UserService userService)
        {
            this.houseDapper = houseDapper;
            this.houseDashboardService = houseDashboardService;
            this.configurationDapper = configurationDapper;
            this.userCollectionDapper = userCollectionDapper;
            this.userService = userService;
        }

        [HttpGet("{userId}/[controller]/dashboard", Name = "GetUserDashboard")]
        [EnableCors("APICors")]
        public IActionResult GetUserDashboard(long userId, [FromHeader] string token)
        {
            var userInfo = userService.GetUserInfo(userId, token);
            if (userInfo == null)
            {
                return Ok(new { success = false, error = "用户未登陆，无法进行操作。" });
            }
            var id = 1;
            var list = userCollectionDapper.LoadUserHouseDashboard(userId)
            .GroupBy(d => d.CityName)
            .Select(i => new
            {
                id = id++,
                cityName = i.Key,
                sources = i.ToList()
            });
            return Ok(new { success = true, data = list });
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/[controller]/", Name = "ListCollection")]
        [HttpPost("{userId}/[controller]/", Name = "ListCollection")]
        public IActionResult List(long userId, [FromHeader] string token, string cityName, string source = "")
        {
            try
            {
                var userInfo = userService.GetUserInfo(userId, token);
                if (userInfo == null)
                {
                    return Ok(new { success = false, error = "用户未登陆，无法进行操作。" });
                }
                var rooms = userCollectionDapper.FindUserCollections(userId, cityName, source);
                return Ok(new { success = true, data = rooms });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.ToString() });
            }
        }

        [EnableCors("APICors")]
        [HttpGet("{userId}/[controller]/{id}", Name = "GetOne")]
        [HttpPost("{userId}/[controller]/{id}", Name = "GetOne")]
        public IActionResult GetOne(long userId, [FromHeader] string token, long id)
        {
            try
            {
                var userInfo = userService.GetUserInfo(userId, token);
                if (userInfo == null)
                {
                    return Ok(new { success = false, error = "用户未登陆，无法进行操作。" });
                }
                var houses = userCollectionDapper.FindUserCollections(userId);
                return Ok(new { success = true, data = houses.FirstOrDefault(h => h.Id == id) });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.ToString() });
            }
        }

        [EnableCors("APICors")]
        [HttpPost("{userId}/[controller]", Name = "CreateCollection")]
        public IActionResult Create(long userId, [FromBody] UserCollection userCollection, [FromHeader] string token)
        {
            var userInfo = userService.GetUserInfo(userId, token);
            if (userInfo == null)
            {
                return Ok(new { success = false, error = "用户未登陆，无法进行操作。" });
            }
            var house = houseDapper.GetHouseID(userCollection.HouseID, userCollection.Source);
            if (house == null)
            {
                return Ok(new { successs = false, error = "房源信息不存在,请刷新页面后重试." });
            }
            userCollection.UserID = userId;
            userCollection.Source = house.Source;
            userCollection.HouseCity = house.LocationCityName;
            userCollectionDapper.InsertUser(userCollection);
            return Ok(new { success = true, message = "收藏成功." }); ;
        }

        [EnableCors("APICors")]
        [HttpDelete("{userId}/[controller]/{id}", Name = "RemoveCollection")]
        public IActionResult Remove(long userId, long id, [FromHeader] string token)
        {
            var userInfo = userService.GetUserInfo(userId, token);
            if (userInfo == null)
            {
                return Ok(new { success = false, error = "用户未登陆，无法进行操作。" });
            }
            var userCollection = userCollectionDapper.FindByIDAndUserID(id, userId);
            if (userCollection == null)
            {
                return Ok(new { successs = false, error = "房源信息不存在,请刷新页面后重试." });
            }
            try
            {
                userCollectionDapper.RemoveByIDAndUserID(userCollection.ID, userCollection.UserID);
            }
            catch(Exception ex)
            {
                return Ok(new { success = false, error = ex.ToString() }); ;
            }

            return Ok(new { success = true, message = "删除成功." });
        }


    }

}
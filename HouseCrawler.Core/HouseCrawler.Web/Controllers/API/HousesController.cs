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
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HouseCrawler.Web.API.Controllers
{
    [Route("v1/[controller]/")]
    public class HousesController : ControllerBase
    {
        private HouseDapper houseDapper;

        private HouseDashboardService houseDashboardService;

        private ConfigDapper configurationDapper;

        private UserCollectionDapper userCollectionDapper;


        public HousesController(HouseDapper houseDapper,
                              HouseDashboardService houseDashboardService,
                              ConfigDapper configurationDapper,
                              UserCollectionDapper userCollectionDapper)
        {
            this.houseDapper = houseDapper;
            this.houseDashboardService = houseDashboardService;
            this.configurationDapper = configurationDapper;
            this.userCollectionDapper = userCollectionDapper;
        }



        [HttpPost("", Name = "Search")]
        [EnableCors("APICors")]
        public IActionResult Search([FromBody] HouseSearchCondition search)
        {
            try
            {
                if (search == null || search.CityName == null)
                {
                    return Ok(new { success = false, error = "查询条件不能为null" });
                }
                var houseList = houseDapper.SearchHouses(search);
                return Ok(new { success = true, data = houseList });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.ToString() });
            }
        }

        [EnableCors("APICors")]
        [HttpGet("dashboard")]
        public IActionResult Dashboards()
        {
            var id = 1;
            var dashboards = houseDashboardService.LoadDashboard()
            .GroupBy(d => d.CityName)
            .Select(i => new
            {
                id = id++,
                cityName = i.Key,
                sources = i.ToList()
            });
            return Ok(new { success = true, data = dashboards });
        }

        [EnableCors("APICors")]
        [HttpGet("citys")]
        public IActionResult Citys()
        {
            var id = 0;
            var citys = houseDashboardService.LoadDashboard().Select(d => d.CityName)
            .Distinct().Select(city => new { id = id++, Name = city }).ToList();
            return Ok(new { success = true, data = citys });
        }

        [EnableCors("APICors")]
        [HttpPost("douban")]
        public IActionResult AddDouBanGroup([FromBody]JToken model)
        {
            string doubanGroup = model?["groupId"].ToString();
            string cityName = model?["cityName"].ToString();
            if (string.IsNullOrEmpty(doubanGroup) || string.IsNullOrEmpty(cityName))
            {
                return Ok(new { success = false, error = "请输入豆瓣小组Group和城市名称。" });
            }
            var topics = DoubanHouseCrawler.GetHouseData(doubanGroup, cityName, 1);
            if (topics != null && topics.Count() > 0)
            {
                var cityInfo = $"{{ 'groupid':'{doubanGroup}','cityname':'{cityName}','pagecount':5}}";
                var doubanConfig = new CrawlerConfiguration();
                if (doubanConfig != null)
                {
                    return Ok(new { success = true });
                }
                var config = new CrawlerConfiguration()
                {
                    ConfigurationKey = 0,
                    ConfigurationValue = cityInfo,
                    ConfigurationName = ConstConfigName.Douban,
                    DataCreateTime = DateTime.Now,
                    IsEnabled = true,
                };
                configurationDapper.Insert(config);
                return Ok(new { success = true });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    error = "保存失败!请检查豆瓣小组ID（如：XMhouse）/城市名称（如：厦门）是否正确..."
                });
            }



        }

    }
}

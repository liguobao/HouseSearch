using System;
using System.Collections.Generic;
using System.Linq;
using HouseCrawler.Web.Models;

namespace HouseCrawler.Web.Service
{
    public class HouseDashboardService
    {
        private RedisService redisService;

        private HouseDapper houseDapper;

        public HouseDashboardService(RedisService redisService, HouseDapper houseDapper)
        {
            this.redisService =redisService;
            this.houseDapper = houseDapper;

        }

        public List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = redisService.ReadCache("HouseDashboard");
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = houseDapper.GetHouseDashboard();
                redisService.WriteCache("HouseDashboard", Newtonsoft.Json.JsonConvert.SerializeObject(dashboards));
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

    }
}

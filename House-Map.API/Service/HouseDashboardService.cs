using System;
using System.Collections.Generic;
using System.Linq;
using HouseMapAPI.Dapper;
using HouseMapAPI.Models;
using HouseMapAPI.Service;

namespace HouseMapAPI.Service
{
    public class HouseDashboardService
    {
        private RedisService redisService;

        private HouseDapper houseDapper;

        public HouseDashboardService(RedisService redisService, HouseDapper houseDapper)
        {
            this.redisService = redisService;
            this.houseDapper = houseDapper;

        }

        public List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = redisService.ReadCache("HouseDashboard");
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = houseDapper.GetHouseDashboard();
                redisService.WriteObject("HouseDashboard", dashboards);
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

    }
}

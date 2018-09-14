using System;
using System.Collections.Generic;
using System.Linq;
using HouseMapAPI.Common;
using HouseMapAPI.Dapper;
using HouseMapAPI.Models;
using HouseMapAPI.Service;

namespace HouseMapAPI.Service
{
    public class DashboardService
    {
        private RedisService redisService;

        private HouseDapper houseDapper;

        public DashboardService(RedisService redisService, HouseDapper houseDapper)
        {
            this.redisService = redisService;
            this.houseDapper = houseDapper;

        }

        public List<HouseDashboard> LoadDashboard()
        {
            var dashboards =  redisService.ReadCache<List<HouseDashboard>>(RedisKey.HouseDashboard.Key,
             RedisKey.HouseDashboard.DBName);
            if (dashboards ==null)
            {
                dashboards = houseDapper.GetHouseDashboard();
                redisService.WriteObject("HouseDashboard", dashboards);
            }
            return dashboards;
            
        }

    }
}

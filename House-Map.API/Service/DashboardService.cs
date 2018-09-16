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
            var dashboards = redisService.ReadCache<List<HouseDashboard>>(RedisKey.HouseDashboard.Key,
             RedisKey.HouseDashboard.DBName);
            if (dashboards == null)
            {
                dashboards = houseDapper.GetHouseDashboard();
                redisService.WriteObject(RedisKey.HouseDashboard.Key,
                 dashboards, RedisKey.HouseDashboard.DBName);
            }
            return dashboards;

        }
        
        public dynamic LoadCityDashboards()
        {
             var id = 1;
            var dashboards = LoadDashboard()
            .GroupBy(d => d.CityName)
            .Select(i => new
            {
                id = id++,
                cityName = i.Key,
                sources = i.ToList()
            });
            return dashboards;
        }

        public Object LoadCitys()
        {
            var id = 0;
            return LoadDashboard().Select(d => d.CityName)
            .Distinct().Select(city => new { id = id++, Name = city }).ToList();
        }

    }
}

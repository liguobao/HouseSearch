using System;
using System.Collections.Generic;
using System.Linq;
using HouseMap.Models;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using HouseMap.Common;
using HouseMapAPI.Service;
using Newtonsoft.Json.Linq;

namespace HouseMapAPI.Service
{
    public class DashboardService
    {
        private RedisTool _redisTool;


        private ConfigDapper _configDapper;

        public DashboardService(RedisTool RedisTool, ConfigDapper configDapper)
        {
            this._redisTool = RedisTool;
            _configDapper = configDapper;

        }

        public List<HouseDashboard> LoadDashboard()
        {
            var dashboards = _redisTool.ReadCache<List<HouseDashboard>>(RedisKey.HouseDashboard.Key,
             RedisKey.HouseDashboard.DBName);
            if (dashboards == null || dashboards.Count == 0)
            {
                dashboards = _configDapper.GetDashboards();
                _redisTool.WriteObject(RedisKey.HouseDashboard.Key, dashboards, RedisKey.HouseDashboard.DBName);
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
                sources = i.GroupBy(d =>d.Source).Select(item =>item.FirstOrDefault()).ToList()
            });
            return dashboards;
        }

        public Object LoadCities()
        {
            var id = 0;
            return LoadDashboard().Select(d => d.CityName)
            .Distinct().Select(city => new { id = id++, Name = city }).ToList();
        }

    }
}

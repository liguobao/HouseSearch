using System;
using System.Collections.Generic;
using System.Linq;
using HouseMap.Models;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Common;
using Newtonsoft.Json.Linq;

namespace HouseMap.Dao
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

        public List<HouseDashboard> LoadDashboards()
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

    }
}

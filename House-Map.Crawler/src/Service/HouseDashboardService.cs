using System;
using System.Collections.Generic;
using System.Linq;
using HouseMap.Models;
using HouseMap.Dao;

namespace HouseMap.Crawler.Service
{
    public class HouseDashboardService
    {

        private RedisService redis;

        private HouseStatDapper _statDapper;

        public HouseDashboardService(RedisService redis, HouseStatDapper statDapper)
        {
            this.redis = redis;
            this._statDapper =  statDapper;
        }


        public List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = redis.ReadCache("HouseDashboard");
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = _statDapper.GetHouseDashboard();
                redis.WriteCache("HouseDashboard", Newtonsoft.Json.JsonConvert.SerializeObject(dashboards));
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

    }
}

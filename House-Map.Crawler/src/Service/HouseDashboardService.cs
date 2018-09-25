using System;
using System.Collections.Generic;
using System.Linq;
using HouseMap.Models;
using HouseMap.Dao;
using HouseMap.Common;

namespace HouseMap.Crawler.Service
{
    public class HouseDashboardService
    {

        private RedisTool redis;

        private HouseStatDapper _statDapper;

        public HouseDashboardService(RedisTool redis, HouseStatDapper statDapper)
        {
            this.redis = redis;
            this._statDapper = statDapper;
        }


        public List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = redis.ReadCache<string>("HouseDashboard", 0);
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = _statDapper.GetHouseDashboard();
                redis.WriteObject("HouseDashboard", dashboards, 0);
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

    }
}

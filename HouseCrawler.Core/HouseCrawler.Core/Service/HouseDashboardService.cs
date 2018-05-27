using System;
using System.Collections.Generic;
using System.Linq;
using HouseCrawler.Core;
using HouseCrawler.Core.DataContent;
using HouseCrawler.Core.Models;

namespace HouseCrawler.Core.Service
{
    public class HouseDashboardService
    {


        public static List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = RedisService.ReadCache("HouseDashboard");
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = HouseDataDapper.GetHouseDashboard();
                RedisService.WriteCache("HouseDashboard", Newtonsoft.Json.JsonConvert.SerializeObject(dashboards));
                return dashboards;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(houseDashboardJson);
            }
        }

    }
}

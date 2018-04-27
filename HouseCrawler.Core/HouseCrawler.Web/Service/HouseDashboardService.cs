using System;
using System.Collections.Generic;
using System.Linq;
using HouseCrawler.Web.DataContent;
using HouseCrawler.Web.Models;

namespace HouseCrawler.Web.Service
{
    public class HouseDashboardService
    {

        private static CrawlerDataContent DataContent = new CrawlerDataContent();

        public static List<HouseDashboard> LoadDashboard()
        {
            string houseDashboardJson = RedisService.ReadCache("HouseDashboard");
            if (string.IsNullOrEmpty(houseDashboardJson))
            {
                List<HouseDashboard> dashboards = CrawlerDataDapper.GetHouseDashboard();
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

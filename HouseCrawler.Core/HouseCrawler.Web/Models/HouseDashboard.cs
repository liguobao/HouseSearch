using HouseCrawler.Web.DataContent;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Web.Models
{
    public class HouseDashboard
    {
        public string CityName { get; set; }

        public int HouseSum { get; set; }

        public string Source { get; set; }

        public DateTime UpdateTime { get; set; }

        private static CrawlerDataContent DataContent = new CrawlerDataContent();

        public static void RefreshDashboard()
        {
            var cityDashboards = CrawlerDataDapper.GetHouseDashboard();
            if (cityDashboards == null || cityDashboards.Count() == 0)
            {
                return;
            }
            var config = DataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.CityHouseInfo);
            if (config == null)
            {
                config = new BizCrawlerConfiguration
                {
                    ConfigurationKey = 0,
                    ConfigurationName = ConstConfigurationName.CityHouseInfo,
                    ConfigurationValue = Newtonsoft.Json.JsonConvert.SerializeObject(cityDashboards),
                    IsEnabled = true
                };
                DataContent.Add(config);
                DataContent.SaveChanges();
            }
            else
            {
                config.ConfigurationKey = 0;
                config.ConfigurationName = ConstConfigurationName.CityHouseInfo;
                config.ConfigurationValue = Newtonsoft.Json.JsonConvert.SerializeObject(cityDashboards);
                config.IsEnabled = true;
                DataContent.SaveChanges();
            }

        }


        public static List<HouseDashboard> LoadDashboard()
        {
            var lstCityHouseInfo = new List<HouseDashboard>();
            BizCrawlerConfiguration config = DataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.CityHouseInfo);
            if (config != null)
            {
                lstCityHouseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(config.ConfigurationValue);
            }
            return lstCityHouseInfo;
        }

    }



}

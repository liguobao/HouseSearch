
using HouseCrawler.Core.DataContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Models
{
    public class HouseDashboard
    {
        public string CityName { get; set; }

        public int HouseSum { get; set; }

        public string Source { get; set; }

        public DateTime UpdateTime { get; set; }

        private static readonly CrawlerDataContent DataContent = new CrawlerDataContent();


        public static void RefreshDashboard()
        {
            var lstCityHouse = CrawlerDataDapper.GetHouseSourceInfoList();
            var config = DataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.CityHouseInfo);
            if (config == null)
            {
                config = new BizCrawlerConfiguration
                {
                    ConfigurationKey = 0,
                    ConfigurationName = ConstConfigurationName.CityHouseInfo,
                    ConfigurationValue = Newtonsoft.Json.JsonConvert.SerializeObject(lstCityHouse),
                    IsEnabled = true
                };
                DataContent.Add(config);
                DataContent.SaveChanges();
            }
            else
            {
                config.ConfigurationKey = 0;
                config.ConfigurationName = ConstConfigurationName.CityHouseInfo;
                config.ConfigurationValue = Newtonsoft.Json.JsonConvert.SerializeObject(lstCityHouse);
                config.IsEnabled = true;
                DataContent.SaveChanges();
            }

        }


        public static List<HouseDashboard> LoadCityDashboards()
        {
            var lstCityHouseInfo = new List<HouseDashboard>();
            var config = DataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.CityHouseInfo);
            if (config != null)
            {
                lstCityHouseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseDashboard>>(config.ConfigurationValue);
            }
            return lstCityHouseInfo;
        }

    }


   
}

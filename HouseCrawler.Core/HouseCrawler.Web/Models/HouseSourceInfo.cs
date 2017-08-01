using HouseCrawler.Web.DBService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Web.Models
{
    public class HouseSourceInfo
    {
        public string CityName { get; set; }

        public int HouseSum { get; set; }

        public string Source { get; set; }

        public DateTime UpdateTime { get; set; }

        private static CrawlerDataContent dataContent = new CrawlerDataContent();


        public static void RefreshHouseSourceInfo()
        {
            var lstCityHouse = new DBHouseSourceInfoDAL().GetHouseSourceInfoList();
            BizCrawlerConfiguration config = dataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.CityHouseInfo);
            if (config == null)
            {
                config = new BizCrawlerConfiguration();
                config.ConfigurationKey = 0;
                config.ConfigurationName = ConstConfigurationName.CityHouseInfo;
                config.ConfigurationValue = Newtonsoft.Json.JsonConvert.SerializeObject(lstCityHouse);
                config.IsEnabled = true;
                dataContent.Add(config);
                dataContent.SaveChanges();
            }
            else
            {
                config.ConfigurationKey = 0;
                config.ConfigurationName = ConstConfigurationName.CityHouseInfo;
                config.ConfigurationValue = Newtonsoft.Json.JsonConvert.SerializeObject(lstCityHouse);
                config.IsEnabled = true;
                dataContent.SaveChanges();
            }

        }


        public static List<HouseSourceInfo> LoadCityHouseInfo()
        {
            var lstCityHouseInfo = new List<HouseSourceInfo>();
            BizCrawlerConfiguration config = dataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.CityHouseInfo);
            if (config != null)
            {
                lstCityHouseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HouseSourceInfo>>(config.ConfigurationValue);
            }
            return lstCityHouseInfo;
        }

    }


   
}

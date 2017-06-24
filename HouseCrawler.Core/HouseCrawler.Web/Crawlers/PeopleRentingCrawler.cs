using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Web
{
    public class PeopleRentingCrawler
    {
        private static CrawlerDataContent dataContent = new CrawlerDataContent();

        public static void CapturHouseInfo()
        {
            var peopleRentingConf = dataContent.CrawlerConfigurations.FirstOrDefault(conf=>conf.ConfigurationName== ConstConfigurationName.HuZhuZuFang);

            var pageCount = peopleRentingConf != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(peopleRentingConf.ConfigurationValue).pagecount.Value : 10;
            for (var pageIndex = 1;pageIndex< pageCount; pageIndex++)
            {
                try
                {
                    GetDataByWebAPI(pageIndex);
                }
                catch(Exception ex)
                {
                    LogHelper.Error("PeopleRentingCrawler CrawlerHouseInfo Exception", ex);
                }
               
            }

        }
        private static void GetDataByWebAPI(int pageNum)
        {
            var dicParameter = new JObject()
            {
                {"uid","" },
                {"pageNum",$"{pageNum}" },
                {"sortType","1" },
                {"sellRentType","2" },
                {"searchCondition","{}" }
            };
            var postHouseURL = $"http://www.huzhumaifang.com:8080/hzmf-integration/getHouseList.action?content={JsonConvert.SerializeObject(dicParameter)}";
            var resultJson = HTTPHelper.GetJsonResultByURL(postHouseURL);
            var resultJObject = JsonConvert.DeserializeObject<JObject>(resultJson);
            var lstHouseInfo = from houseInfo in resultJObject["houseList"]
                               select new
                               {
                                   houseCreateTime = houseInfo["houseCreateTime"],
                                   houseRentPrice = houseInfo["houseRentPrice"],
                                   houseDescript = houseInfo["houseDescript"],
                                   houseId = houseInfo["houseId"]
                               };

            var tmp = new List<BizHouseInfo>();

            HashSet<string> hsHouseOnlineURL = GetAllHouseOnlineURL();

            foreach (var houseInfo in lstHouseInfo)
            {
                var houseURL = $"http://www.huzhumaifang.com/Renting/house_detail/id/{houseInfo.houseId.ToObject<Int32>()}.html";
                if (hsHouseOnlineURL.Contains(houseURL))
                    continue;

                var desc = houseInfo.houseDescript.ToObject<string>().Replace("😄", "");
                dataContent.Add(new BizHouseInfo()
                {
                    HouseOnlineURL = houseURL,
                    HouseLocation = desc,
                    HousePrice = houseInfo.houseRentPrice.ToObject<Int32>(),
                    HouseText = desc,
                    DataCreateTime = DateTime.Now,
                    HouseTitle = desc,
                    DisPlayPrice = houseInfo.houseRentPrice.ToString(),
                    LocationCityName = "上海",
                    PubTime = houseInfo.houseCreateTime.ToObject<DateTime>(),
                    Source = ConstConfigurationName.HuZhuZuFang,
                });
            }
            dataContent.SaveChanges();
        }

        private static HashSet<string> GetAllHouseOnlineURL()
        {
            var hsHouseOnlineURL = new HashSet<string>();
            dataContent.HouseInfos.Where(h => !string.IsNullOrEmpty(h.HouseOnlineURL)).Select(h => h.HouseOnlineURL).Distinct().ToList().ForEach(url =>
            {
                if (!hsHouseOnlineURL.Contains(url))
                {
                    hsHouseOnlineURL.Add(url);
                }
            });
            return hsHouseOnlineURL;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseCrawler.Core.DBService.DAL;
using HouseCrawler.Core.DataContent;

namespace HouseCrawler.Core
{
    public class PeopleRentingCrawler
    {
        private static readonly CrawlerDataContent DataContent = new CrawlerDataContent();

        public static void CapturHouseInfo()
        {
            var peopleRentingConf = DataContent.CrawlerConfigurations.FirstOrDefault(conf=>conf.ConfigurationName== ConstConfigurationName.HuZhuZuFang);

            var pageCount = peopleRentingConf != null ? JsonConvert.DeserializeObject<dynamic>(peopleRentingConf.ConfigurationValue).pagecount.Value : 10;
            var hsHouseOnlineUrl = new CrawlerDAL().GetAllHuzhuzufangHouseOnlineURL();
            for (var pageIndex = 1;pageIndex< pageCount; pageIndex++)
            {
                var index = pageIndex;
                LogHelper.RunActionNotThrowEx(() => 
                {
                    GetDataByWebAPI(index, hsHouseOnlineUrl);
                }, "CapturHouseInfo", pageIndex);
            }

        }
        private static void GetDataByWebAPI(int pageNum, HashSet<string> hsHouseOnlineUrl)
        {
            var dicParameter = new JObject()
            {
                {"uid","" },
                {"pageNum",$"{pageNum}" },
                {"sortType","1" },
                {"sellRentType","2" },
                {"searchCondition","{}" }
            };
            var postHouseUrl = $"http://www.huzhumaifang.com:8080/hzmf-integration/getHouseList.action?content={JsonConvert.SerializeObject(dicParameter)}";
            var resultJson = HTTPHelper.GetJsonResultByURL(postHouseUrl);
            var resultJObject = JsonConvert.DeserializeObject<JObject>(resultJson);
            var lstHouseInfo = from houseInfo in resultJObject["houseList"]
                               select new
                               {
                                   houseCreateTime = houseInfo["houseCreateTime"],
                                   houseRentPrice = houseInfo["houseRentPrice"],
                                   houseDescript = houseInfo["houseDescript"],
                                   houseId = houseInfo["houseId"]
                               };

            var tmp = new List<MutualHouseInfo>();

          

            foreach (var houseInfo in lstHouseInfo)
            {
                var houseUrl = $"http://www.huzhumaifang.com/Renting/house_detail/id/{houseInfo.houseId.ToObject<Int32>()}.html";
                if (hsHouseOnlineUrl.Contains(houseUrl))
                    continue;

                var desc = houseInfo.houseDescript.ToObject<string>().Replace("😄", "");
                DataContent.MutualHouseInfos.Add(new MutualHouseInfo()
                {
                    HouseOnlineURL = houseUrl,
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
            DataContent.SaveChanges();
        }

       
    }
}

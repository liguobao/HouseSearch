using System;
using System.Collections.Generic;
using System.Linq;
using HouseCrawler.Core.Common;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Core
{
    public class DoubanHouseCrawler
    {
        private HouseDapper houseDapper;

        private ConfigDapper configDapper;
        public DoubanHouseCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.configDapper = configDapper;
        }

        public void Run()
        {
            try
            {
                int captrueHouseCount = 0;
                DateTime startTime = DateTime.Now;
                foreach (var doubanConf in configDapper.GetList(ConstConfigName.Douban))
                {
                    LogHelper.RunActionNotThrowEx(() =>
                    {
                        List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
                        var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                        for (var pageIndex = 0; pageIndex < confInfo.pagecount.Value; pageIndex++)
                        {
                            var lstHouseInfo = GetHouseData(confInfo.groupid.Value,
                                confInfo.cityname.Value, pageIndex);
                            houses.AddRange(lstHouseInfo);
                        }
                        captrueHouseCount = captrueHouseCount + houses.Count;
                        houseDapper.BulkInsertHouses(houses);
                    }, "DoubanHouseCrawler CaptureHouseInfo ", doubanConf);
                }
                LogHelper.Info($"DoubanHouseCrawler finish.本次共爬取到{captrueHouseCount}条数据，耗时{ (DateTime.Now - startTime).TotalSeconds}秒。");
            }
            catch (Exception ex)
            {
                LogHelper.Error("DoubanHouseCrawler CrawlerHouseInfo Exception", ex);
            }
        }

        public static List<BaseHouseInfo> GetHouseData(string groupID, string cityName, int pageIndex)
        {
            List<BaseHouseInfo> lstHouseInfo = new List<BaseHouseInfo>();
            var apiURL = $"https://api.douban.com/v2/group/{groupID}/topics?start={pageIndex * 50}&count=50";
            LogHelper.Debug($"url:{apiURL},groupID:{groupID}, city:{cityName}");
            var result = GetAPIResult(apiURL);
            if (string.IsNullOrEmpty(result))
            {
                return lstHouseInfo;
            }
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            foreach (var topic in resultJObject["topics"])
            {
                var housePrice = JiebaTools.GetHousePrice(topic["content"].ToString());
                var photos = topic["photos"]?.Select(photo => photo["alt"].ToString()).ToList();
                var house = new BaseHouseInfo()
                {
                    HouseLocation = topic["title"].ToString(),
                    HouseTitle = topic["title"].ToString(),
                    HouseOnlineURL = topic["share_url"].ToString(),
                    HouseText = topic["content"].ToString(),
                    HousePrice = housePrice,
                    IsAnalyzed = true,
                    DisPlayPrice = housePrice > 0 ? $"{housePrice}元" : "",
                    Source = ConstConfigName.Douban,
                    LocationCityName = cityName,
                    Status = 1,
                    PicURLs = JsonConvert.SerializeObject(photos),
                    PubTime = topic["created"].ToObject<DateTime>()
                };
                lstHouseInfo.Add(house);
            }

            return lstHouseInfo;
        }

        private static string GetAPIResult(string apiURL)
        {
            try
            {
                var client = new RestClient(apiURL);
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("connection", "keep-alive");
                request.AddHeader("x-requested-with", "XMLHttpRequest");
                request.AddHeader("referer", apiURL);
                request.AddHeader("accept", "*/*");
                request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("cookie", "bid=qLvbOle-G58; ps=y; ue=^\\^codelover^@qq.com^^; push_noty_num=0; push_doumail_num=0; __utmz=30149280.1521636704.1.1.utmcsr=(direct)^|utmccn=(direct)^|utmcmd=(none); __utmv=30149280.15460; ll=^\\^108296^^; _vwo_uuid_v2=D87414308A33790472DBB4D2B1DC0DE7B^|6a9fc300e5ea8c7485f9a922d87e820b; __utma=30149280.2046746446.1521636704.1522567163.1522675073.3");
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return response.Content;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAPIResult", ex);
            }
            return "";

        }

    }
}

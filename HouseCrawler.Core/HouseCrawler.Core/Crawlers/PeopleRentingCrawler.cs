using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseCrawler.Core.DataContent;
using RestSharp;

namespace HouseCrawler.Core
{
    public class PeopleRentingCrawler
    {
        private static readonly CrawlerDataContent DataContent = new CrawlerDataContent();


        public static void CaptureHouseInfo()
        {
            var peopleRentingConf = DataContent.CrawlerConfigurations.FirstOrDefault(conf => conf.ConfigurationName == ConstConfigurationName.HuZhuZuFang);

            var pageCount = peopleRentingConf != null ? JsonConvert.DeserializeObject<dynamic>(peopleRentingConf.ConfigurationValue).pagecount.Value : 10;
            var hsHouseOnlineUrl = new HashSet<string>();
            for (var pageNum = 1; pageNum < pageCount; pageNum++)
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    string result = getResultFromAPI(pageNum);
                    List<MutualHouseInfo> houseList = GetHouseData(result);
                    DataContent.MutualHouseInfos.AddRange(houseList);
                    DataContent.SaveChanges();

                }, "CapturHouseInfo", pageNum);
            }

        }
        private static List<MutualHouseInfo> GetHouseData(string result)
        {
            List<MutualHouseInfo> houseList = new List<MutualHouseInfo>();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            if (resultJObject == null || resultJObject["head"] == null || !resultJObject["head"]["success"].ToObject<Boolean>())
            {
                return houseList;
            }
            foreach (var item in resultJObject["houseList"])
            {
                MutualHouseInfo houseInfo = new MutualHouseInfo();
                var houseDesc = item["houseDescript"].ToObject<string>().Replace("😄", "");
                var houseURL = $"http://www.huzhumaifang.com/Renting/house_detail/id/{item["houseId"]}.html";
                if (DataContent.MutualHouseInfos.Any(h => h.HouseOnlineURL == houseURL))
                    continue;
                houseInfo.HouseOnlineURL = houseURL;
                houseInfo.HouseTitle = houseDesc;
                houseInfo.HouseLocation = houseDesc;
                houseInfo.HouseText = item.ToString();
                houseInfo.HousePrice = item["houseRentPrice"].ToObject<Int32>();
                houseInfo.DisPlayPrice = item["houseRentPrice"].ToString();
                houseInfo.DataCreateTime = DateTime.Now;
                houseInfo.PubTime = item["houseCreateTime"].ToObject<DateTime>();
                houseInfo.PicURLs = item["bigPicUrls"].ToString();
                houseInfo.Source = ConstConfigurationName.HuZhuZuFang;
                houseList.Add(houseInfo);
            }
            return houseList;

        }


        private static string getResultFromAPI(int pageNum)
        {
            var dicParameter = new JObject()
            {
                {"uid","" },
                {"pageNum",$"{pageNum}" },
                {"sortType","1" },
                {"sellRentType","2" },
                {"searchCondition","{}" }
            };
            var client = new RestClient("http://www.huzhumaifang.com:8080/hzmf-integration/getHouseList.action");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("user-agent", "Apache-HttpClient/UNAVAILABLE (java 1.4)");
            request.AddHeader("host", "www.huzhumaifang.com:8080");
            request.AddParameter("application/x-www-form-urlencoded", $"content={JsonConvert.SerializeObject(dicParameter)}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            return result;
        }

    }
}

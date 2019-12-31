using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using HouseCrawler.Core.Common;

namespace HouseCrawler.Core
{
    public class PeopleRentingCrawler
    {
        private HouseDapper houseDapper;

        private ConfigDapper configDapper;
        public PeopleRentingCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.configDapper = configDapper;
        }


        public  void Run()
        {
            int captrueHouseCount = 0;
            DateTime startTime = DateTime.Now;

            var peopleRentingConf = configDapper.GetList(ConstConfigName.HuZhuZuFang)
            .FirstOrDefault();

            var pageCount = peopleRentingConf != null
                ? JsonConvert.DeserializeObject<dynamic>(peopleRentingConf.ConfigurationValue).pagecount.Value
                : 10;
            var hsHouseOnlineUrl = new HashSet<string>();
            List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
            for (var pageNum = 1; pageNum < pageCount; pageNum++)
            {
                string result = getResultFromAPI(pageNum);
                houses.AddRange(GetHouseData(result));
            }
            houseDapper.BulkInsertHouses(houses);
            captrueHouseCount = captrueHouseCount + houses.Count;

            LogHelper.Info($"PeopleRentingCrawler finish.本次共爬取到{captrueHouseCount}条数据，耗时{ (DateTime.Now - startTime).TotalSeconds}秒。");

        }
        private static List<BaseHouseInfo> GetHouseData(string result)
        {
            List<BaseHouseInfo> houseList = new List<BaseHouseInfo>();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            if (resultJObject == null || resultJObject["head"] == null || !resultJObject["head"]["success"].ToObject<Boolean>())
            {
                return houseList;
            }
            foreach (var item in resultJObject["houseList"])
            {
                BaseHouseInfo houseInfo = new BaseHouseInfo();
                var houseDesc = item["houseDescript"].ToObject<string>().Replace("😄", "");
                var houseURL = $"http://www.huzhumaifang.com/Renting/house_detail/id/{item["houseId"]}.html";
                houseInfo.HouseOnlineURL = houseURL;
                houseInfo.HouseTitle = houseDesc;
                houseInfo.HouseLocation = houseDesc;
                houseInfo.HouseText = item.ToString();
                houseInfo.HousePrice = item["houseRentPrice"].ToObject<Int32>();
                houseInfo.DisPlayPrice = item["houseRentPrice"].ToString();
                houseInfo.LocationCityName = "上海";
                houseInfo.PubTime = item["houseCreateTime"].ToObject<DateTime>();
                houseInfo.PicURLs = item["bigPicUrls"].ToString();
                houseInfo.Source = ConstConfigName.HuZhuZuFang;
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

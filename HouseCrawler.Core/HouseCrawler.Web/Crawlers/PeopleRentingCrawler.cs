using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace HouseCrawler.Web
{
    public class PeopleRentingCrawler
    {
        public static List<HouseInfo> GetHouseData(int pageNum)
        {
            var result = getResultFromAPI(pageNum);

            var houseList = new List<HouseInfo>();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            if (resultJObject == null || resultJObject["head"] == null || !resultJObject["head"]["success"].ToObject<Boolean>())
            {
                return houseList;
            }
            foreach (var item in resultJObject["houseList"])
            {
                var houseInfo = new HouseInfo();
                var houseDesc = item["houseDescript"].ToObject<string>().Replace("😄", "");
                var houseURL = $"http://www.huzhumaifang.com/Renting/house_detail/id/{item["houseId"]}.html";
                houseInfo.HouseOnlineURL = houseURL;
                houseInfo.HouseTitle = houseDesc;
                houseInfo.HouseLocation = houseDesc;
                houseInfo.HouseText = item.ToString();
                houseInfo.HousePrice = item["houseRentPrice"].ToObject<Int32>();
                houseInfo.DisPlayPrice = item["houseRentPrice"].ToString();
                houseInfo.DataCreateTime = DateTime.Now;
                houseInfo.LocationCityName = "上海";
                houseInfo.PubTime = item["houseCreateTime"].ToObject<DateTime>();
              
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

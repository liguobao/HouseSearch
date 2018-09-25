using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Dapper;
using AngleSharp.Dom;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;

using Newtonsoft.Json.Linq;
using HouseMap.Models;

namespace HouseMap.Crawler
{

    public class HuzhuCrawler : BaseCrawler
    {

        public HuzhuCrawler(HouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = ConstConfigName.HuZhuZuFang;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            return getResultFromAPI(page);
        }

        public override List<HouseInfo> ParseHouses(JToken config, string data)
        {
            List<HouseInfo> houseList = new List<HouseInfo>();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            if (resultJObject == null || resultJObject["head"] == null || !resultJObject["head"]["success"].ToObject<Boolean>())
            {
                return houseList;
            }
            foreach (var item in resultJObject["houseList"])
            {
                HouseInfo houseInfo = new HouseInfo();
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
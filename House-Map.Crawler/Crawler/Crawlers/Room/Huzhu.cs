using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using Dapper;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;
using HouseMap.Common;
using Newtonsoft.Json.Linq;
using System.Net;

namespace HouseMap.Crawler
{

    public class Huzhu : BaseCrawler
    {

        public Huzhu(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic,  RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.HuZhuZuFang;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            return getResultFromAPI(page);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houseList = new List<DBHouse>();
            var resultJObject = JObject.Parse(data);
            if (resultJObject == null || resultJObject["head"] == null || !resultJObject["head"]["success"].ToObject<Boolean>())
            {
                return houseList;
            }
            foreach (var item in resultJObject["houseList"])
            {
                DBHouse house = new DBHouse();
                house.Id = Tools.GetGuid();
                var houseDesc = item["houseDescript"].ToObject<string>().Replace("😄", "");
                var houseURL = $"http://www.huzhumaifang.com/Renting/house_detail/id/{item["houseId"]}.html";
                house.OnlineURL = houseURL;
                house.Title = houseDesc;
                house.Location = houseDesc;
                house.Text = houseDesc;
                house.JsonData = item.ToString();
                house.Price = item["houseRentPrice"].ToObject<Int32>();
                house.City = config.City;
                house.RentType = GetRentType(houseDesc);
                house.PubTime = item["houseCreateTime"].ToObject<DateTime>();
                house.PicURLs = item["bigPicUrls"].ToString();
                house.Source = SourceEnum.HuZhuZuFang.GetSourceName();
                houseList.Add(house);
            }
            return houseList;
        }
        
        private int GetRentType(string houseDesc)
        {
            if (houseDesc.Contains("一室一厅") || houseDesc.Contains("1室1厅") || houseDesc.Contains("一室户") || houseDesc.Contains("1室户"))
            {
                return (int)RentTypeEnum.OneRoom;
            }
            else if (houseDesc.Contains("整租"))
            {
                return (int)RentTypeEnum.AllInOne;
            }
            else if (houseDesc.Contains("合租"))
            {
                return (int)RentTypeEnum.Shared;
            }
            return (int)RentTypeEnum.AllInOne;
        }

        private static string getResultFromAPI(int pageNum)
        {
            var page = pageNum + 1;
            var client = new RestClient("http://www.huzhumaifang.com:8080/hzmf-integration/getHouseList.action");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "1e596557-c9e1-aa61-1d32-321e1a786303");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("user-agent", "Apache-HttpClient/UNAVAILABLE (java 1.4)");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "www.huzhumaifang.com:8080");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "content=%7B%22searchCondition%22%3A%22%7B%7D%22%2C%22pageNum%22%3A%22"
            + page + "%22%2C%22sellRentType%22%3A%222%22%2C%22sortType%22%3A%221%22%2C%22uid%22%3A%22%22%7D", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
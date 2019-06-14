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

namespace HouseMap.Crawler
{

    public class Hezuzhaoshiyou : BaseCrawler
    {


        public Hezuzhaoshiyou(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic,  RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.Hezuzhaoshiyou;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var city = config.City;
            return GetData(city, page);
        }

        private static string GetData(string city, int page)
        {
            var client = new RestClient("https://d3hpv18j.engine.lncld.net/1.1/functions/listHouse");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "d3hpv18j.engine.lncld.net");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/70.0.3538.80 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x26070333) NetType/WIFI Language/zh_CN Process/appbrand2");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddHeader("x-lc-id", "D3HPV18JfBjR8FrQ2G8dQbVR-gzGzoHsz");
            request.AddHeader("x-lc-ua", "LeanCloud-JS-SDK/3.2.1 (Weapp)");
            request.AddHeader("x-lc-sign", "0559661013302d878cd054c742d5314d,1542813489189");
            request.AddHeader("x-lc-session", "r2uc039o7bksr5k7r3mp54lgh");
            request.AddHeader("referer", "https://servicewechat.com/wxf23fa5ba5a7e9b94/60/page-frame.html");
            request.AddHeader("x-lc-prod", "0");
            request.AddHeader("charset", "utf-8");
            request.AddParameter("application/json;charset=UTF-8", "{\"currCity\":\"" + city + "市\",\"currLoc\":\"\",\"pageNumber\":" + page + ",\"queryCond\":{},\"filterType\":\"house\"}",
             ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }


        private static string GetHouseData(string objectId)
        {
            var client = new RestClient("https://d3hpv18j.engine.lncld.net/1.1/functions/queryHouseById");
            var request = new RestRequest(Method.POST);
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "d3hpv18j.engine.lncld.net");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/70.0.3538.80 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x26070333) NetType/WIFI Language/zh_CN Process/appbrand2");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddHeader("x-lc-id", "D3HPV18JfBjR8FrQ2G8dQbVR-gzGzoHsz");
            request.AddHeader("x-lc-ua", "LeanCloud-JS-SDK/3.2.1 (Weapp)");
            request.AddHeader("x-lc-sign", "ad1ba3f8a57c3c56eea524a1bf7293d4,1542813212650");
            request.AddHeader("x-lc-session", "r2uc039o7bksr5k7r3mp54lgh");
            request.AddHeader("referer", "https://servicewechat.com/wxf23fa5ba5a7e9b94/60/page-frame.html");
            request.AddHeader("x-lc-prod", "0");
            request.AddHeader("charset", "utf-8");
            request.AddParameter("application/json;charset=UTF-8", "{\"houseId\":\"" + objectId + "\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful ? response.Content : "";

        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var result = JToken.Parse(data);
            if (result == null || result["result"].Count() == 0)
            {
                return houses;
            }
            foreach (var room in result["result"])
            {
                var roomDetailResult = GetHouseData(room["objectId"]?.ToString());
                if(string.IsNullOrEmpty(roomDetailResult) || roomDetailResult.Contains("Object not found"))
                {
                    continue;
                }
                var roomDetail = JToken.Parse(roomDetailResult)["result"];
                var house = new DBHouse();

                house.OnlineURL = roomDetail["qrImgUrl"]?.ToString();
                house.Text = roomDetail["houseDesc"]?.ToString();
                
                house.Title = room["title"]?.ToString();
                house.City = config.City;
                if (room["location"] != null)
                {
                    house.Longitude = Tools.SubLocation(room["location"]?["longitude"]?.ToString());
                    house.Latitude = Tools.SubLocation(room["location"]?["latitude"]?.ToString());
                    house.Location = room["location"]?["address"]?.ToString();
                }
            
                house.PubTime = room["createdAt"].ToObject<DateTime>();
                house.Price = !string.IsNullOrEmpty(room["price"]?.ToString()) ? room["price"].ToObject<int>() : 0;
                house.PicURLs = room["images"].ToString();
                house.Tags = room["district"]?.ToString();
                
                house.Id = Tools.GetGuid();
                house.Source = SourceEnum.Hezuzhaoshiyou.GetSourceName();
                house.JsonData = room.ToString();
                house.RentType = (int)RentTypeEnum.Undefined;
                houses.Add(house);
            }
            return houses;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["roomType"]?.ToString();
            if (roomType == "次卧" || roomType == "主卧")
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (roomType == "整租")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }

            return rentType;
        }
    }
}
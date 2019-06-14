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

    public class Pinshiyou : BaseCrawler
    {


        public Pinshiyou(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic,  RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.Pinshiyou;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var city = config.City;
            var offset = page * 10;
            return GetData(city, offset);
        }

        private static string GetData(string city, int offset)
        {
            var client = new RestClient("https://api.xiaozhuankeji.com/graphql");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "api.xiaozhuankeji.com");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/68.0.3440.91 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x2607033C) NetType/WIFI Language/zh_CN Process/toolsmp");
            request.AddHeader("token", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VySWQiOiI1YzE0Yzg0YTBkNWZkZjFmZmNiMWNhOTgifQ.aAGg1135LxUV6_O6TeeStOGz2-wTCV0DGHcYCzGKqho");
            request.AddHeader("accept", "*/*");
            request.AddHeader("n", "611481");
            request.AddHeader("platform", "android");
            request.AddHeader("version", "1.1.95");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("v", "1");
            request.AddHeader("t", "1544866521687");
            request.AddHeader("s", "80d93cc604f7ae29d02bcae4d6511fea");
            request.AddHeader("product", "roommate");
            request.AddHeader("referer", "https://servicewechat.com/wx0b017a14ebdca1ee/122/page-frame.html");
            request.AddHeader("charset", "utf-8");
            request.AddParameter("application/json", "[{\"operationName\":\"searchRoommateRequest\",\"variables\":{\"roomTypes\":[],\"searchText\":null,\"showVideoOnly\":false,\"locationCircle\":null,\"types\":[\"userRental\",\"transferRental\"],\"userToken\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VySWQiOiI1YzE0Yzg0YTBkNWZkZjFmZmNiMWNhOTgifQ.aAGg1135LxUV6_O6TeeStOGz2-wTCV0DGHcYCzGKqho\",\"city\":\""+city+"\",\"startingDate\":null,\"maxBudget\":0,\"targetGender\":null,\"realtorFree\":false,\"shortTerm\":false,\"internetIncluded\":false,\"userIds\":[]},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"07760e284d7163861b795964d412fef84c40dc9a774dd00e68117a4b4fe8ba19\"}}}]", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }

        private static string GetBody(string city, int offset)
        {
            return "[{\"operationName\": \"searchRoommateRequest\",\"variables\": {\t\"roomTypes\": [],\t\"searchText\": null,\t\"showVideoOnly\": false,\t\"locationCircle\": null,\t\"types\": [\"userRental\", \"transferRental\"],\t\"userToken\": \"\",\t\"city\": \""
                        + city + "\",\t\"startingDate\": null,\t\"maxBudget\": 0,\t\"targetGender\": null,\t\"realtorFree\": false,\t\"shortTerm\": false,\t\"internetIncluded\": false,\t\"offset\": "
                        + offset + "},\"extensions\": {\t\"persistedQuery\": {\t\t\"version\": 1,\t\t\"sha256Hash\": \"54093955bbd959cb2e01f56ed7d9691ea9b4a5b5941726a00f8fcca70925cc5b\"\t}}\n}]";
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var result = JToken.Parse(data);
            if (result == null || result.Count() == 0 || result[0]["data"]?["requests"] == null)
            {
                return houses;
            }
            foreach (var room in result[0]["data"]?["requests"])
            {
                var house = new DBHouse();
                house.Text = room["description"]?.ToString();
                house.Title = room["title"]?.ToString();
                house.City = room["city"]?.ToString();
                house.Location = room["locationName"]?.ToString();
                if (room["location"] != null)
                {
                    house.Longitude = room["location"]?["longitude"]?.ToString();
                    house.Latitude = room["location"]?["latitude"]?.ToString();
                }
                house.PubTime = room["createTime"].ToObject<DateTime>();
                house.Price = !string.IsNullOrEmpty(room["cost"]?.ToString()) ? room["cost"].ToObject<int>() : 0;
                house.PicURLs = room["images"].ToString();
                house.Tags = string.Join("|", room["tags"].Select(t => t.ToString()));
                house.OnlineURL = $"https://api.xiaozhuankeji.com/qrcode?product=roommate&page=RoommateRequest&id={room["id"].ToString()}";
                house.Id = Tools.GetGuid();
                house.Source = SourceEnum.Pinshiyou.GetSourceName();
                house.JsonData = room.ToString();
                house.RentType = GetRentType(room);
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
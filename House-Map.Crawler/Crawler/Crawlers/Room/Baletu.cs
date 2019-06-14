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

    public class Baletu : BaseCrawler
    {
        private readonly RestClient _restClient;
        public Baletu(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic, RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            _restClient = new RestClient("https://api.baletu.com/App401/Homes/guessLikeRecHouseList");
            this.Source = SourceEnum.Baletu;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var city_id = jsonData["city_id"].ToString();
            var request = new RestRequest("", Method.POST);
            request.AddHeader("cookie2", "=1");
            request.AddHeader("user-agent", "app:4.6.2(Linux; Android 9;Xiaomi MI 8 Build:PKQ1.180729.001;862756046928978)");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "api.baletu.com");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            var pageIndex = page + 1;
            request.AddParameter("application/x-www-form-urlencoded", $"P={pageIndex}&S=50&device_id=862756046928990&v=4.6.2&from=3&city_id={city_id}",
            ParameterType.RequestBody);
            IRestResponse response = _restClient.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }



        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var result = JToken.Parse(data);
            if (result == null || result.Count() == 0 || result["result"]?["list"] == null)
            {
                return houses;
            }
            var jsonData = JToken.Parse(config.Json);
            var pinyin = jsonData["pinyin"].ToString();

            foreach (var room in result["result"]?["list"])
            {
                var house = new DBHouse();
                house.Text = room["area_name"]?.ToString() + room["subway_desc"]?.ToString();
                house.Title = room["subdistrict_name"]?.ToString() + room["house_info_concat"]?.ToString();
                house.City = config.City;
                house.Location = room["subdistrict_name"]?.ToString();
                if (!string.IsNullOrEmpty(room["latlon"]?.ToString()))
                {
                    house.Latitude = room["latlon"]?.ToString().Split(",")[0];
                    house.Longitude = room["latlon"]?.ToString().Split(",")[1];
                }
                if (room["publish_date"]?.ToString() == "今天发布")
                {
                    house.PubTime = DateTime.Now.Date;
                }
                else if (room["publish_date"]?.ToString() == "昨天发布")
                {
                    house.PubTime = DateTime.Now.Date.AddDays(-1);
                }
                else if (room["publish_date"].ToString().Contains("天前发布"))
                {
                    house.PubTime = DateTime.Now.Date.AddDays(-int.Parse(room["publish_date"].ToString().Replace("天前发布", "").Trim()));
                }
                else
                {
                    house.PubTime = DateTime.Now;
                }
                house.Price = !string.IsNullOrEmpty(room["month_rent"]?.ToString()) ? room["month_rent"].ToObject<int>() : 0;
                house.PicURLs = Tools.GetPicURLs(room["house_main_image"]?.ToString().Replace("@!380_280.png", ""));
                house.Tags = string.Join("|", room["labels"].Select(t => t.ToString()));
                house.OnlineURL = $"https://m.baletu.com/{pinyin}/house/{room["house_id"]?.ToString()}.html";
                house.Id = Tools.GetGuid();
                house.Source = SourceEnum.Baletu.GetSourceName();
                house.JsonData = room.ToString();
                house.RentType = GetRentType(room);
                houses.Add(house);
            }
            return houses;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["room_type"]?.ToString();
            if (roomType == "2" || roomType == "1")
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (roomType == "0")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            return rentType;
        }
    }
}
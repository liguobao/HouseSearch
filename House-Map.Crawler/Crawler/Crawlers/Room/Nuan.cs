using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Threading;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Common;
using Newtonsoft.Json.Linq;

namespace HouseMap.Crawler
{

    public class Nuan : BaseCrawler
    {
        private readonly RestClient _restClient;
        public Nuan(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic, RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            _restClient = new RestClient("https://nuan.io");
            _restClient.AddDefaultHeader("user-agent", "Mozilla/5.0 (Linux; Android 5.1; vivo V3M A Build/LMY47I) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/39.0.0.0 Mobile Safari/537.36 MicroMessenger/7.0.3.1400(0x2700033C) Process/appbrand0 NetType/WIFI Language/zh_CN");
            this.Source = SourceEnum.Nuan;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var city_code = jsonData["city_code"].ToString();
            var pageIndex = page + 1;
            var resource = $"/get-room-results?searchType=text&city={city_code}&rentType=all&bedroomAll=true&sort=default&pageNo={pageIndex}&source=58&source=doubangroup&source=ganji&source=soufang&source=anjuke&source=nuan&source=smth&source=weibo";
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("host", "nuan.io");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 5.1; vivo V3M A Build/LMY47I) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/39.0.0.0 Mobile Safari/537.36 MicroMessenger/7.0.3.1400(0x2700033C) Process/appbrand0 NetType/WIFI Language/zh_CN");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("x-nuan-map-vendor", "tencent");
            request.AddHeader("x-nuan-platform", "wechat");
            request.AddHeader("x-nuan-has-mini-nav", "true");
            request.AddHeader("referer", "https://servicewechat.com/wx363afd5a1384b770/43/page-frame.html");
            request.AddHeader("charset", "utf-8");
            IRestResponse response = _restClient.Execute(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine($"resource:{resource}, response.Content:{response.Content}");
            }
            var random = new Random((int)Tools.GetMillisecondTimestamp()).Next(1, 10000);
            Thread.Sleep(random);
            Console.WriteLine($"get data finish, sleep {random / 1000} s");
            return response.IsSuccessful ? response.Content : "";
        }



        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            if (data.Contains("<html"))
            {
                return houses;
            }
            var result = JToken.Parse(data);
            if (result == null || result.Count() == 0 || result["rooms"] == null)
            {
                return houses;
            }
            var jsonData = JToken.Parse(config.Json);
            var cityCode = jsonData["city_code"].ToString();

            foreach (var room in result["rooms"])
            {
                try
                {
                    DBHouse house = ConvertToHouse(config, room);
                    houses.Add(house);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ConvertToHouse fail,StackTrace:{ex.StackTrace},room:{room.ToString()}");
                }
            }
            return houses;
        }

        private static DBHouse ConvertToHouse(DBConfig config, JToken houseData)
        {
            var house = new DBHouse();
            house.Title = houseData["title"].ToString();
            house.City = config.City;
            house.PubTime = DateTime.Parse(houseData["postTime"].ToString());
            house.Text = houseData["description"]?.ToString();
            house.Location = houseData["address"]?.ToString();
            if (houseData["coordinates"].ToObject<List<decimal>>() != null && houseData["coordinates"].ToObject<List<decimal>>().Count > 0)
            {
                house.Latitude = houseData["coordinates"].ToObject<List<decimal>>()[1].ToString();
                house.Longitude = houseData["coordinates"].ToObject<List<decimal>>()[0].ToString();
            }
            if (houseData["price"] != null)
            {
                house.Price = houseData["price"].ToObject<int>();
            }
            house.PicURLs = JsonConvert.SerializeObject(houseData["images"].Select(i => i["location"]?.ToString()).ToList()); ;
            house.OnlineURL = houseData?["url"]?.ToString();
            house.Id = Tools.GetGuid();
            house.Source = SourceEnum.Nuan.GetSourceName();
            house.JsonData = houseData.ToString();
            house.RentType = GetRentType(houseData);
            return house;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["rentType"]?.ToString();
            if (string.IsNullOrEmpty(roomType))
            {
                return rentType;
            }
            if (roomType == "shared")
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (roomType == "entire")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            return rentType;
        }
    }
}
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

    public class Qingke : BaseCrawler
    {
        private readonly RestClient _restClient;
        public Qingke(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic,  RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            _restClient = new RestClient("https://i.qk365.com/unit/findNewRoom");
            this.Source = SourceEnum.Qingke;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var city_code = jsonData["city_code"].ToString();
            var pageIndex = page + 1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("referer", $"https://i.qk365.com/{city_code}/list");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "https://i.qk365.com");
            request.AddHeader("pragma", "no-cache");
            request.AddParameter("application/x-www-form-urlencoded; charset=UTF-8", $"cityPinyin={city_code}&queryDto.siteCode=2&queryDto.district=&queryDto.cellArea=&queryDto.metroId=&queryDto.stationId=&queryDto.price=&queryDto.roomType=&queryDto.rentDateNum=&queryDto.inputWord=&queryDto.listModel=1&queryDto.pageNum={pageIndex}&queryDto.recom=false&queryDto.isActivity=&queryDto.sortColumn=&queryDto.sortDir=&queryDto.priceType=",
             ParameterType.RequestBody);
            IRestResponse response = _restClient.Execute(request);
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
            if (result == null || result["code"]?.ToString() != "0" || result["data"]?["rooms"] == null)
            {
                return houses;
            }
            var jsonData = JToken.Parse(config.Json);
            var cityCode = jsonData["city_code"].ToString();

            foreach (var room in result["data"]?["rooms"])
            {
                var id = room["romId"]?.ToString();
                var onlineURL = $"https://i.qk365.com/{cityCode}/easy/{id}";
                var house = new DBHouse();
                house.Id = Tools.GetGuid();
                house.OnlineURL = onlineURL;
                house.Title = room["romTitle"].ToString();
                house.City = config.City;
                house.PubTime = DateTime.Now;
                house.Location = room["village"]?.ToString();
                house.Latitude = room["villLat"].ToString();
                house.Longitude = room["villLon"].ToString();
                house.Price = room["romSpecialPrice"].ToObject<int>();
                List<string> picURLs = GetPicURLs(room);
                house.PicURLs = JsonConvert.SerializeObject(picURLs);
                house.Source = SourceEnum.Qingke.GetSourceName();
                house.RentType = GetRentType(room);
                house.Tags = string.Join("|", room["roomLabels"]);
                house.JsonData = room.ToString();
                houses.Add(house);

            }
            return houses;
        }

        private static List<string> GetPicURLs(JToken room)
        {
            var picURLs = new List<string>();
            picURLs.Add(room["cepUrl"].ToString());
            if (!string.IsNullOrEmpty(room["videoUrl"]?.ToString()))
            {
                picURLs.Add(room["videoUrl"]?.ToString());
            }

            return picURLs;
        }



        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["roomType"]?.ToString();
            if (string.IsNullOrEmpty(roomType))
            {
                return rentType;
            }
            if (roomType == "2")
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (roomType == "3")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            return rentType;
        }
    }
}
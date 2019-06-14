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

    public class Anjuke : BaseCrawler
    {
        private readonly RestClient _restClient;
        public Anjuke(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic, RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            _restClient = new RestClient($"https://m.anjuke.com");
            this.Source = SourceEnum.Anjuke;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var pinyin = jsonData["pinyin"].ToString();
            var pageIndex = page + 1;

            var request = new RestRequest($"/{pinyin}/rentlistbypage/all/a0_0-b0-0-0-f4/?page={pageIndex}&search_firstpage=1", Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("referer", $"https://m.anjuke.com/{pinyin}/rent/?from=anjuke_home");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("accept", "application/json");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("host", "m.anjuke.com");
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
            if (result == null || result.Count() == 0 || result["datas"]?["list_info"] == null)
            {
                return houses;
            }
            var jsonData = JToken.Parse(config.Json);
            var pinyin = jsonData["pinyin"].ToString();

            foreach (var room in result["datas"]?["list_info"])
            {
                var house = new DBHouse();
                house.Title = room["title"]?.ToString();
                house.City = config.City;
                // todo 回头通过详情API或者其他方式获取发布时间
                // text 同理
                house.PubTime = DateTime.Now.Date;
                house.Text = "";
                house.Location = room["comm_name"]?.ToString();
                house.Price = !string.IsNullOrEmpty(room["price"]?.ToString()) ? room["price"].ToObject<int>() : 0;
                house.PicURLs = Tools.GetPicURLs(room["img"]?.ToString().Replace("240x180", "960x720"));
                house.Tags = string.Join("|", room["tags"].Select(t => t.ToString()));
                house.OnlineURL = room["prop_url"]?.ToString();
                house.Id = Tools.GetGuid();
                house.Source = SourceEnum.Anjuke.GetSourceName();
                house.JsonData = room.ToString();
                house.RentType = GetRentType(room);
                houses.Add(house);
            }
            return houses;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["rent_type_name"]?.ToString();
            if (roomType == "合租")
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
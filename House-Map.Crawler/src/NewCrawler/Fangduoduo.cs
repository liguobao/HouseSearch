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
using HouseMap.Common;
using HouseMap.Crawler.Service;

namespace HouseMap.Crawler
{

    public class Fangduoduo : NewBaseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();
        public Fangduoduo(NewHouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService)
        : base(houseDapper, configDapper, elasticService)
        {
            this.Source = SourceEnum.Fangduoduo;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var configJson = JToken.Parse(config.Json);
            var fangPage = page + 1;
            return GetHouseListResult(configJson["cityId"]?.ToString(), fangPage, configJson["pinyin"]?.ToString());
        }

        private static string GetHouseListResult(string cityId, int page, string pinyin)
        {
            var client = new RestClient("https://m.fangdd.com/api/zufang/fetchRentList?cityId=121");
            var request = new RestRequest(Method.POST);
            request.AddHeader("referer", $"https://m.fangdd.com/{pinyin}/zufang-list-v3/");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("client", "m");
            request.AddHeader("fa", "FA1.0.1541334670781.4122388242");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "https://m.fangdd.com");
            request.AddHeader("city-id", cityId);
            request.AddHeader("user-id", "0");
            request.AddParameter("application/json;charset=UTF-8", "{\"cityId\":" + cityId + ",\"condition\":\"p" + page + "_i50_v3\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


        public override List<DBHouse> ParseHouses(DBConfig config, string jsonOrHTML)
        {
            var houses = new List<DBHouse>();
            if (jsonOrHTML.Contains("html>") || jsonOrHTML.Contains("<script>"))
            {
                return houses;
            }
            var result = JToken.Parse(jsonOrHTML);
            if (result?["code"].ToString() != "200" || result?["data"]?["list"] == null)
            {
                return houses;
            }
            var configJson = JToken.Parse(config.Json);
            var pinyin = configJson["pinyin"]?.ToString();

            foreach (var room in result?["data"]?["list"])
            {
                var roomId = room["id"].ToString();
                var house = new DBHouse();
                house.OnlineURL = $"https://m.fangdd.com/{pinyin}/zufang/{room["id"].ToString()}.html";
                house.Location = room["addressName"]?.ToString();
                house.City = room["cityName"].ToString();
                house.PicURLs = JsonConvert.SerializeObject(new List<string>() { room["coverImage"].ToString() });
                house.Title = GetTitle(room);
                house.Longitude = room["lng"].ToString();
                house.Latitude = room["lat"].ToString();
                house.Tags = string.Join("|", room["tagList"]);
                house.Price = room["minPrice"].ToObject<int>();
                house.PubTime = Tools.JavaTimeStampToDateTime(room["createTime"].ToObject<long>());
                house.Id = Tools.GetUUId();
                house.Text = GetText(room);
                house.Source = SourceEnum.Fangduoduo.GetSourceName();
                house.JsonData = room.ToString();
                house.RentType = GetRentType(room);
                houses.Add(house);
            }
            return houses;
        }

        private static string GetText(JToken room)
        {
            var houseText = $"{room["houseType"]?.ToString()}，{room["decoration"]?.ToString()}，位于{room["sectionName"]?.ToString()},{room["addressName"]?.ToString()}，";
            if (!string.IsNullOrEmpty(room["stationName"]?.ToString()))
            {
                houseText = houseText + $"临近{room["stationName"].ToString()}地铁站 ";
            }

            return houseText;
        }

        private static string GetTitle(JToken room)
        {
            var titleFields = new List<string>() { "districtName", "cellName", "direction" };
            var title = string.Join("-", titleFields.Select(f => room[f]?.ToString()).Where(v => !string.IsNullOrEmpty(v)));
            if (!string.IsNullOrEmpty(room["roomType"].ToString()))
            {
                title = title + "-" + room["roomType"].ToString();
            }

            return title;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = room["rentType"].ToObject<int>();
            if (rentType == 2)
            {
                return (int)RentTypeEnum.Shared;
            }
            else if (rentType == 1)
            {
                return (int)RentTypeEnum.AllInOne;
            }
            return 0;
        }
    }
}
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

namespace HouseMap.Crawler
{

    public class Fangduoduo : BaseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();
        public Fangduoduo(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService,  RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.Fangduoduo;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var configJson = JToken.Parse(config.Json);
            var condition = configJson["condition"]?.ToString();
            if (string.IsNullOrEmpty(condition))
            {
                condition = "p1_i20_v3";
            }
            return GetHouseListResult(configJson["cityId"]?.ToString(), page, configJson["pinyin"]?.ToString(), condition);
        }

        private static string GetHouseListResult(string cityId, int page, string pinyin, string condition)
        {
            var client = new RestClient("https://m.fangdd.com/api/data/zufangFetchRentList");
            var request = new RestRequest(Method.POST);
            request.AddHeader("referer", $"https://m.fangdd.com/{pinyin}/zufang-list-v3/");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1");
            request.AddHeader("session-id", "85741e90e02d11e89df59da5272c0b71");
            request.AddHeader("fdd_trace_id", "4da9f390e6db11e8aaccaf47ed81949d");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("client", "m");
            request.AddHeader("fa", "FA1.0.1541334670781.4122388242");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "https://m.fangdd.com");
            request.AddHeader("city-id", cityId);
            request.AddHeader("cookie", "deviceId=85741e90e02d11e89df59da5272c0b71; _fa=FA1.0.1541334670781.4122388242; _ga=GA1.2.2103099985.1541334671; cacheCity=guangzhou; TP_AGENT_ID=5be98035ec250fb6a2015bc6; cityName=\"%E6%B7%B1%E5%9C%B3\"; cityPinYin=shenzhen; cityPy=sz; city_id=1337; client=M; _F=m; _T=%E7%A7%9F%E6%88%BF%E6%88%BF%E6%BA%90%E8%AF%A6%E6%83%85; _gid=GA1.2.1662368975.1542029367; Hm_lvt_3c55bb2c7ecabdb4d55e6539eaa2f3ab=1541334671,1542029370; _ha=1542068945647.5216018004; Hm_lpvt_3c55bb2c7ecabdb4d55e6539eaa2f3ab=1542069000");
            request.AddHeader("user-id", "0");
            string query_condition = condition.Replace("p1", "p" + (page + 1));
            request.AddParameter("application/json;charset=UTF-8", "{\"cityId\":" + cityId + ",\"condition\":\"" + query_condition + "\"}", ParameterType.RequestBody);
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
                house.Longitude = room["lng"]?.ToString().Length > 10 ? room["lng"]?.ToString().Substring(0, 10) : room["lng"]?.ToString();
                house.Latitude = room["lat"]?.ToString().Length > 10 ? room["lat"]?.ToString().Substring(0, 10) : room["lat"]?.ToString();
                house.Tags = string.Join("|", room["tagList"]);
                house.Price = room["minPrice"].ToObject<int>();
                house.PubTime = Tools.JavaTimeStampToDateTime(room["createTime"].ToObject<long>());
                house.Id = Tools.GetGuid();
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
            var titleFields = new List<string>() { "districtName", "cellName" };
            var title = string.Join("-", titleFields.Select(f => room[f]?.ToString()).Where(v => !string.IsNullOrEmpty(v)));
            title = title + "-朝" + room["direction"]?.ToString();
            if (!string.IsNullOrEmpty(room["roomType"]?.ToString()))
            {
                title = title + "-" + room["roomType"]?.ToString();
            }

            return title;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = room["rentType"]?.ToObject<int>();
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
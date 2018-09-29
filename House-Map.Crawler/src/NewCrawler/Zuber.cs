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
using System.Security.Cryptography;
using System.Text;
using HouseMap.Models;
using HouseMap.Common;

namespace HouseMap.Crawler
{

    public class Zuber : NewBaseCrawler, INewCrawler
    {

        static string API_VERSION = "v3/";
        static string SCENE = "2567a5ec9705eb7ac2c984033e06189d";
        public Zuber(NewHouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = SourceEnum.Zuber;
        }

        public override string GetJsonOrHTML(DbConfig config, int page)
        {
            var cityName = JToken.Parse(config.Json)["cityname"].ToString();
            var sequence = JToken.Parse(config.Json)["sequence"]?.ToString();
            return GetAPIResult(cityName, sequence); ;
        }

        public override List<DBHouse> ParseHouses(DbConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            if (resultJObject["code"].ToString() != "0" || resultJObject["result"] == null)
                return houses;
            if (resultJObject["result"]["has_next_page"].ToObject<bool>())
            {
                var json = JToken.Parse(config.Json);
                json["sequence"] = resultJObject["result"]["sequence"].ToObject<string>();
                config.Json = json.ToString();
            }
            var cityName = config.City;
            foreach (var item in resultJObject["result"]["items"])
            {
                var house = ConvertHouse(cityName, item);
                houses.Add(house);
            }
            return houses;
        }

        private static DBHouse ConvertHouse(string cityName, JToken item)
        {
            var room = item["room"];
            var housePrice = room["cost1"].ToObject<int>();
            var house = new DBHouse()
            {
                Location = room["address"].ToString(),
                Title = room["summary"].ToString(),
                OnlineURL = $"http://www.zuber.im/app/room/{room["id"].ToString()}",
                Text = item.ToString(),
                Price = housePrice,
                Source = SourceEnum.Zuber.GetSourceName(),
                City = cityName,
                Longitude = room["longitude"].ToObject<decimal>(),
                Latitude = room["latitude"].ToObject<decimal>(),
                PicURLs = GetPhotos(room),
                JsonData = room.ToString(),
                PubTime = room["last_modify_time"].ToObject<DateTime>()
            };
            return house;
        }

        private static string GetPhotos(JToken room)
        {
            var photos = new List<String>();
            if (room["photo"] != null)
            {
                photos.Add(room["photo"].ToString().Replace("?imageView2/0/w/300", ""));
            }
            return JsonConvert.SerializeObject(photos);
        }
        private static string GetAPIResult(string cityName, string sequence)
        {
            try
            {
                var referer = "http://www.zuber.im/app/?house=1";
                string url = $"https://api.zuber.im/v3/rooms/search?city={cityName}&house=1&sex=&cost1=&cost2=&address=&start_time=&end_time=&longitude=&latitude=&subway_line=&subway_station=&short_rent=&type=&bed=true&room_type_affirm=&coords_type=gaode&sequence={sequence}";
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.AddHeader("connection", "keep-alive");
                request.AddHeader("referer", referer);
                request.AddHeader("accept", "application/json, text/plain, */*");
                request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("origin", "http://www.zuber.im");
                var _stamp = GetTimestamp().ToString();
                var _secret = "";
                var _token = md5(_stamp);
                var _scene = SCENE;
                var _method = "get";
                var _param = "{}";//(_method === 'get' || _method === 'delete') ? '{}' : JSON.stringify(param)
                var api = "rooms/search";
                var _source = "request_url=" + API_VERSION + api + "&" +
                    "content=" + _param + "&" +
                    "request_method=" + _method + '&' +
                    "timestamp=" + _stamp + '&' +
                    "secret=" + _secret;
                var _signature = md5(_source);
                var _auth = "timestamp=" + _stamp + ";" +
                    "oauth2=" + _token + ";"
                    + "signature=" + _signature + ";"
                    + "scene=" + _scene;
                request.AddHeader("authorization", _auth);
                LogHelper.Debug($"_source:{_source},_auth:{_auth}");
                IRestResponse response = client.Execute(request);
                return response.Content;

            }
            catch (Exception ex)
            {
                LogHelper.Error("ZuberHouseCrawler", ex);
                return string.Empty;
            }

        }

        private static string md5(string observedText)
        {
            string result;
            using (MD5 hash = MD5.Create())
            {
                result = String.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(observedText)
                    )
                    select ba.ToString("x2")
                );
            }
            return result;
        }

        private static int GetTimestamp()
        {
            return (int)(DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

    }
}
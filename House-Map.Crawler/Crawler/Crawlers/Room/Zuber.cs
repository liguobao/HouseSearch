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
using HouseMap.Common;

namespace HouseMap.Crawler
{

    public class Zuber : BaseCrawler, IHouseCrawler
    {

        static string API_VERSION = "v3/";

        static string API_VERSION_V2 = "client";
        static string SCENE = "2567a5ec9705eb7ac2c984033e06189d";

        static string SCENEV2 = "2567a5ec9705eb7ac2c984033e06189d";
        public Zuber(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService, RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.Zuber;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var cityName = JToken.Parse(config.Json)["cityname"].ToString();
            var sequence = JToken.Parse(config.Json)["sequence"]?.ToString();
            return GetResultV2(cityName, sequence); ;
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
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
            var city = config.City;
            foreach (var item in resultJObject["result"]["items"])
            {
                var house = ConvertHouses(city, item);
                houses.AddRange(house);
            }
            return houses;
        }

        private static List<DBHouse> ConvertHouses(string city, JToken item)
        {
            var houses = new List<DBHouse>();
            var room = item["room"];
            foreach (var bed in room["client_attr"]["beds"])
            {
                var housePrice = bed["money"].ToObject<int>();
                var house = new DBHouse()
                {
                    Location = room["region"]?.ToString() + room["road"]?.ToString() + room["street"]?.ToString(),
                    Title = room["full_title"]?.ToString() + "-" + bed["title"]?.ToString(),
                    OnlineURL = $"https://mobile.zuber.im/bed/{bed["id"]?.ToString()}?biz=false",
                    Text = bed["content"]?.ToString(),
                    Price = housePrice,
                    Source = SourceEnum.Zuber.GetSourceName(),
                    City = city,
                    RentType = ConvertToRentType(room["room_type_affirm"]?.ToString()),
                    Longitude = room["longitude"]?.ToString(),
                    Latitude = room["latitude"]?.ToString(),
                    PicURLs = GetBedPhotos(bed),
                    JsonData = item.ToString(),
                    Tags = $"{room["subway_line"]?.ToString()}|{room["subway_station"]?.ToString()}",
                    PubTime = room["last_modify_time"].ToObject<DateTime>(),
                    Id = Tools.GetGuid()
                };
                houses.Add(house);
            }

            return houses;
        }

        private static int ConvertToRentType(string summary)
        {
            if (summary.Contains("整租"))
                return (int)RentTypeEnum.AllInOne;
            if (summary.Contains("一室户"))
                return (int)RentTypeEnum.OneRoom;
            if (summary.Contains("次卧") || summary.Contains("主卧") || summary.Contains("两室") || summary.Contains("三室") || summary.Contains("四室"))
                return (int)RentTypeEnum.Shared;
            return (int)RentTypeEnum.Undefined;
        }


        private static string GetBedPhotos(JToken bed)
        {
            var photos = new List<String>();
            if (bed["photo"] != null)
            {
                photos.Add(bed["photo"]["src"].ToString().Replace("?imageView2/0/w/300", ""));
            }
            return JsonConvert.SerializeObject(photos);
        }


        private static string GetResultV2(string cityName, string sequence)
        {
            try
            {
                var client = new RestClient($"https://apiservices.zuber.im/client/search/bed?city={cityName}&cost1=0&cost2=0&has_short_rent=0&has_bathroom=0&has_video=0&region=&subway_line=&sex=&bed_count=&type=&room_type_affirm=&sequence={sequence}&longitude=&latitude=");
                var request = new RestRequest(Method.GET);
                request.AddHeader("terminal", "device_platform=web;app_version=4.6.8");
                request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");
                request.AddHeader("origin", "https://mobile.zuber.im");
                request.AddHeader("referer", "https://mobile.zuber.im/search/rent");
                request.AddHeader("accept", "application/json, text/plain, */*");

                var _stamp = GetTimestamp().ToString();
                var _secret = "";
                var _token = md5(_stamp);
                var _scene = SCENEV2;
                var _method = "get";
                var _param = "{}";//(_method === 'get' || _method === 'delete') ? '{}' : JSON.stringify(param)
                var api = "/search/bed";
                var _source = "request_url=" + API_VERSION_V2 + api + "&" +
                    "content=" + _param + "&" +
                    "request_method=" + _method + '&' +
                    "timestamp=" + _stamp + '&' +
                    "secret=" + _secret;
                var _signature = md5(_source);
                var _auth = "timestamp=" + _stamp + ";" +
                    "oauth2=" + _token + ";"
                    + "signature=" + _signature + ";"
                    + "scene=" + _scene + ";deployKey=";
                request.AddHeader("authorization", _auth);
                LogHelper.Debug($"_source:{_source},_auth:{_auth}");
                IRestResponse response = client.Execute(request);
                return response.IsSuccessful ? response.Content : "";

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
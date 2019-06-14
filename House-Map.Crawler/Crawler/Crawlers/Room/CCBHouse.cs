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
using Microsoft.Extensions.Options;
using HouseMap.Common;
using System.Net;

namespace HouseMap.Crawler
{

    public class CCBHouse : BaseCrawler
    {
        private readonly RestClient _restClient;

        public CCBHouse(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService, RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.CCBHouse;
            this._restClient = new RestClient("http://api.jiayuan.ccbhome.cn");
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            string cityShortCutName = jsonData["shortcutname"]?.ToString();
            string apiKey = jsonData["APIKey"]?.ToString();
            return GetResultByAPI(apiKey, cityShortCutName, page);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();

            var jsonData = JToken.Parse(config.Json);
            string cityShortCutName = jsonData["shortcutname"]?.ToString();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            if (resultJObject["items"] == null)
            {
                return houses;
            }
            foreach (var item in resultJObject["items"])
            {
                if (string.IsNullOrEmpty(item?["address"]?.ToString()) && string.IsNullOrEmpty(item?["haName"]?.ToString()))
                {
                    continue;
                }
                DBHouse houseInfo = new DBHouse();
                houseInfo.Id = Tools.GetGuid();
                houseInfo.OnlineURL = GetHouseOnlineURL(cityShortCutName, item);
                houseInfo.Location = !string.IsNullOrEmpty(item?["address"]?.ToString()) ? item?["address"]?.ToString() : item?["haName"]?.ToString();
                houseInfo.Title = item["headline"].ToObject<string>();
                houseInfo.City = item["cityName"].ToObject<string>();
                houseInfo.Text = item["headline"].ToObject<string>();
                houseInfo.Latitude = item["gps"]?.ToString().Split(",")[0];
                houseInfo.Longitude = item["gps"]?.ToString().Split(",")[1];
                houseInfo.JsonData = item.ToString();
                houseInfo.Price = item["totalPrice"].ToObject<Int32>();
                houseInfo.PubTime = item["publishTime"].ToObject<DateTime>();
                houseInfo.Source = SourceEnum.CCBHouse.GetSourceName();
                houseInfo.RentType = ConvertToRentType(item["chummage"].ToString());
                houses.Add(houseInfo);
            }

            return houses;
        }

        private int ConvertToRentType(string chummage)
        {
            if (chummage.Contains("整租"))
            {
                return (int)RentTypeEnum.AllInOne;
            }
            else if (chummage.Contains("按间"))
            {
                return (int)RentTypeEnum.Shared;
            }
            return (int)RentTypeEnum.Undefined;
        }

        private static string GetHouseOnlineURL(string cityShortCutName, JToken item)
        {
            var houseURL = "";
            if (!string.IsNullOrEmpty(item["web_url"].ToString()))
            {
                houseURL = item["web_url"].ToString();
            }
            else if (!string.IsNullOrEmpty(item["app_url"].ToString()))
            {
                houseURL = item["app_url"].ToString();
            }
            else
            {
                houseURL = $"http://{cityShortCutName}.jiayuan.home.ccb.com/lease/{item["dealCode"].ToString()}.html";
            }

            return houseURL;
        }


        private string GetResultByAPI(string apiKey, string cityShortCutName, int page)
        {
            var request = new RestRequest($"/hlsp/cityhouse/deal/search?apiKey={apiKey}&city={cityShortCutName}&saleOrLease=lease&pageSize=50&page={page + 1}&lang=zh-CN&tmflags=3", Method.GET);
            request.AddHeader("cookie2", "=1");
            request.AddHeader("cookie", "null=874709770.20480.0000");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "api.jiayuan.ccbhome.cn");
            IRestResponse response = _restClient.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            Console.WriteLine($"request:{request.Resource}, response:{response.Content}");
            return "";
        }



    }
}
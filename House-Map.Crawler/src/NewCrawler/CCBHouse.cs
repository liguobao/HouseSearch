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
using HouseMap.Models;
using HouseMap.Common;
using HouseMap.Crawler.Service;
using System.Net;

namespace HouseMap.Crawler
{

    public class CCBHouse : NewBaseCrawler
    {

        public CCBHouse(NewHouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService)
        : base(houseDapper, configDapper, elasticService)
        {
            this.Source = SourceEnum.CCBHouse;
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
                houseInfo.Id = Tools.GetUUId();
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
            //var queryText = "offer=0&apiKey=cef8222092f74b95a8b24bc4a9e694a0&city=sh&saleOrLease=lease&pageSize=10&page=2&propType=11&tmflags=3&_interfaceUrl=/hlsp/cityhouse/deal/search";

            // var queryText = $"offer=0&apiKey={apiKey}&city={cityShortCutName}&saleOrLease=lease&pageSize=100&page={page}&propType=11&tmflags=3&_interfaceUrl=/hlsp/cityhouse/deal/search";
            var body = "_reqParams=offer%3D" + page * 100 + "%26apiKey%3D" + apiKey + "%26city%3D" + cityShortCutName + "%26saleOrLease%3Dlease%26pageSize%3D100%26page%3D%" + page + "26propType%3D11%26tmflags%3D3&_interfaceUrl=%2Fhlsp%2Fcityhouse%2Fdeal%2Fsearch&_reqMethod=GET";
            var client = new RestClient("http://bankservice.home.ccb.com/LHECISM/LanHaiHttpResfulReqServlet");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("cookie2", "$Version=1");
            request.AddHeader("cookie", "BIGipServerccvcc_jt_197.1_80_web_pool=1277362954.20480.0000");
            request.AddHeader("host", "bankservice.home.ccb.com");
            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";
        }



    }
}
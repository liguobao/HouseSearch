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

namespace HouseMap.Crawler
{

    public class CCBHouse : NewBaseCrawler
    {

        public CCBHouse(NewHouseDapper houseDapper, ConfigDapper configDapper)
         : base(houseDapper, configDapper)
        {
            this.Source = SourceEnum.CCBHouse;
        }

        public override string GetJsonOrHTML(DbConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            string cityShortCutName = jsonData["shortcutname"]?.ToString();
            string apiKey = jsonData["APIKey"]?.ToString();
            return GetResultByAPI(apiKey, cityShortCutName, page);
        }

        public override List<DBHouse> ParseHouses(DbConfig config, string data)
        {
            var houseList = new List<DBHouse>();

            var jsonData = JToken.Parse(config.Json);
            string cityShortCutName = jsonData["shortcutname"]?.ToString();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            foreach (var item in resultJObject["items"])
            {
                DBHouse houseInfo = new DBHouse();
                houseInfo.Id = Tools.GetUUId();
                houseInfo.OnlineURL = GetHouseOnlineURL(cityShortCutName, item);
                houseInfo.Location = item["address"].ToObject<string>();
                houseInfo.Title = item["headline"].ToObject<string>();
                houseInfo.City = item["cityName"].ToObject<string>();
                houseInfo.Latitude = item["gps"]?.ToString().Split(",")[0];
                houseInfo.Longitude = item["gps"]?.ToString().Split(",")[1];
                houseInfo.JsonData = item.ToString();
                houseInfo.Price = item["totalPrice"].ToObject<Int32>();
                houseInfo.PubTime = item["publishTime"].ToObject<DateTime>();
                houseInfo.Source = SourceEnum.CCBHouse.GetSourceName();
                houseInfo.RentType = ConvertToRentType(item["chummage"].ToString());
                Console.WriteLine(item["chummage"].ToString());
                houseList.Add(houseInfo);
            }

            return houseList;
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
            //                _reqParams=offer%3D0%26apiKey%3Dcef8222092f74b95a8b24bc4a9e694a0%26city%3Dsh%26saleOrLease%3Dlease%26pageSize%3D10%26page%3D1%26propType%3D11%26tmflags%3D3&_interfaceUrl=%2Fhlsp%2Fcityhouse%2Fdeal%2Fsearch&_reqMethod=GET
                        //    _reqParams=offer%3D0%26apiKey%3Dcef8222092f74b95a8b24bc4a9e694a0%26city%3Dsh%26saleOrLease%3Dlease%26pageSize%3D10%26page%3D1%26propType%3D11%26tmflags%3D3&_interfaceUrl=%2Fhlsp%2Fcityhouse%2Fdeal%2Fsearch&_reqMethod=GET
            string formBody =$"_reqParams=apiKey%3D{apiKey}%26city%3D{cityShortCutName}%26saleOrLease%3Dlease%26pageSize%3D50%26page%3D{page}%26propType%3D11%26tmflags%3D3&_interfaceUrl=%2Fhlsp%2Fcityhouse%2Fdeal%2Fsearch&_reqMethod=GET";
            var client = new RestClient("http://bankservice.home.ccb.com/LHECISM/LanHaiHttpResfulReqServlet");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("cookie2", "$Version=1");
            request.AddHeader("cookie", "BIGipServerccvcc_jt_197.1_80_web_pool=1277362954.20480.0000");
            request.AddHeader("host", "bankservice.home.ccb.com");
            request.AddParameter("application/x-www-form-urlencoded", formBody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";
        }
    }
}
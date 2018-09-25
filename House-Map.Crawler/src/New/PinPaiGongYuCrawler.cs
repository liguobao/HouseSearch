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
using HouseMap.Models;

namespace HouseMap.Crawler
{

    public class PinPaiGongYuCrawler : BaseCrawler
    {

        public PinPaiGongYuCrawler(HouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = ConstConfigName.PinPaiGongYu;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            var shortCutName = config["shortcutname"].ToString();
            var cityName = config["cityname"].ToString();
            return GetDataFromAPI(shortCutName, page);
        }

        public override List<HouseInfo> ParseHouses(JToken config, string data)
        {
            List<HouseInfo> houseList = new List<HouseInfo>();
            var shortCutName = config["shortcutname"].ToString();
            var cityName = config["cityname"].ToString();

            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            foreach (var info in resultJObject["result"]["getListInfo"]["infolist"])
            {
                var houseInfo = ConvertToHouse(shortCutName, cityName, info);
                houseList.Add(houseInfo);
            }
            return houseList;
        }

        private static HouseInfo ConvertToHouse(string shortCutName, string cityName, JToken info)
        {
            var onlineUrl = $"https://{shortCutName}.58.com/pinpaigongyu/{info["infoID"].ToString()}x.shtml";
            var housePrice = decimal.Parse(info["minPrice"].ToString());
            var houseInfo = new HouseInfo
            {
                HouseTitle = $"{info["title"].ToString()}-{info["layout"].ToString()}",
                HouseOnlineURL = onlineUrl,
                DisPlayPrice = info["priceTitle"].ToString(),
                HouseLocation = GetHouseLocation(info),
                Source = ConstConfigName.PinPaiGongYu,
                HousePrice = housePrice,
                HouseText = info.ToString(),
                LocationCityName = cityName,
                PubTime = new DateTime(info["postDate"]["year"].ToObject<int>(),
                info["postDate"]["mon"].ToObject<int>(),
                info["postDate"]["mday"].ToObject<int>(),
                info["postDate"]["hours"].ToObject<int>(),
                info["postDate"]["minutes"].ToObject<int>(),
                info["postDate"]["seconds"].ToObject<int>()),
                PicURLs = info["picsUrl"].ToString()
            };
            return houseInfo;
        }

        private static string GetHouseLocation(JToken info)
        {
            var houseLocation = "";
            var title = info["title"].ToString();
            var titleList = title.Split(" ");
            if (titleList.Length >= 3)
            {
                houseLocation = titleList[2];
            }
            if (titleList.Length >= 4)
            {
                houseLocation = houseLocation + "-" + titleList[3];
            }
            if (string.IsNullOrEmpty(houseLocation))
            {
                houseLocation = info["titles"]["title"].ToString().Split("|")[1].Trim();
            }
            return houseLocation;
        }
        public static string GetDataFromAPI(string citySortName, int page)
        {
            string parameters = $"&localname={citySortName}&page={page}";
            var client = new RestClient("https://appgongyu.58.com/house/listing/gongyu?tabkey=allcity&action=getListInfo&curVer=8.6.5&appId=1&os=android&format=json&geotype=baidu&v=1"
            + parameters);
            var request = new RestRequest(Method.GET);
            request.AddHeader("user-agent", "okhttp/3.4.2");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "appgongyu.58.com");
            request.AddHeader("product", "58app");
            request.AddHeader("lat", "31.328703");
            request.AddHeader("r", "1920_1080");
            request.AddHeader("ua", "SM801");
            request.AddHeader("brand", "SMARTISAN");
            request.AddHeader("location", "2,6180,6348");
            request.AddHeader("58mac", "B4:0B:44:80:2E:B6");
            request.AddHeader("platform", "android");
            request.AddHeader("currentcid", "2");
            request.AddHeader("rnsoerror", "0");
            request.AddHeader("os", "android");
            request.AddHeader("owner", "baidu");
            request.AddHeader("deviceid", "57b4bf2216c7d1da");
            request.AddHeader("m", "B4:0B:44:80:2E:B6");
            request.AddHeader("cid", "2");
            request.AddHeader("androidid", "57b4bf2216c7d1da");
            request.AddHeader("apn", "WIFI");
            request.AddHeader("uniqueid", "0aa38c71a1f1192af301c5ac03aa0198");
            request.AddHeader("58ua", "58app");
            request.AddHeader("nettype", "wifi");
            request.AddHeader("osarch", "arm64-v8a");
            request.AddHeader("productorid", "1");
            request.AddHeader("version", "8.6.5");
            request.AddHeader("bangbangid", "1080866410605347478");
            request.AddHeader("bundle", "com.wuba");
            request.AddHeader("maptype", "2");
            request.AddHeader("totalsize", "24.7");
            request.AddHeader("rimei", "990006210059787");
            request.AddHeader("id58", "97987698730095");
            request.AddHeader("xxzl_deviceid", "IaqlqznImYdoMvhJpnkjFpsGfWr09FnsJscDy3FpeK+k+afS/XcvibL6qHQue6uz");
            request.AddHeader("marketchannelid", "1593");
            request.AddHeader("osv", "5.1.1");
            request.AddHeader("lon", "121.39829");
            request.AddHeader("official", "true");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


    }
}
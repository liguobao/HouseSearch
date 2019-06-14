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

    public class PinPaiGongYu : BaseCrawler
    {
        private readonly RestClient _restClient;

        public PinPaiGongYu(HouseDapper houseDapper, ConfigDapper configDapper,
         ElasticService elastic,  RedisTool redisTool) 
         : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.PinPaiGongYu;
            _restClient = new RestClient();
            _restClient.BaseUrl = new Uri("https://appgongyu.58.com/house/listing/");
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var json = JToken.Parse(config.Json);
            var shortCutName = json["shortcutname"].ToString();
            var cityName = json["cityname"].ToString();
            return GetDataFromAPI(shortCutName, page + 1);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var json = JToken.Parse(config.Json);
            var shortCutName = json["shortcutname"].ToString();
            var cityName = json["cityname"].ToString();

            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            foreach (var info in resultJObject["result"]["getListInfo"]["infolist"])
            {
                var house = ConvertToHouse(shortCutName, cityName, info);
                houses.Add(house);
            }
            return houses;
        }

        private static DBHouse ConvertToHouse(string shortCutName, string cityName, JToken info)
        {
            var onlineUrl = $"https://{shortCutName}.58.com/pinpaigongyu/{info["infoID"].ToString()}x.shtml";
            var housePrice = int.Parse(info["minPrice"].ToString());
            var houseInfo = new DBHouse
            {
                Id = Tools.GetGuid(),
                Title = $"{info["title"].ToString()}-{info["subTitle"].ToString()}",
                OnlineURL = onlineUrl,
                Location = info["areaName"]?.ToString() + info["districtCode"]?.ToString(),
                Source = SourceEnum.PinPaiGongYu.GetSourceName(),
                Price = housePrice,
                Longitude = info["longitude"]?.ToString().Length > 10 ? info["longitude"]?.ToString().Substring(0, 10) : info["longitude"]?.ToString(),
                Latitude = info["latitude"]?.ToString().Length > 10 ? info["latitude"]?.ToString().Substring(0, 10) : info["latitude"]?.ToString(),
                Labels = $"{info["companyName"].ToString()}|{info["label"].ToString()}",
                City = cityName,
                JsonData = info.ToString(),
                PubTime = new DateTime(info["postDate"]["year"].ToObject<int>(),
                info["postDate"]["mon"].ToObject<int>(),
                info["postDate"]["mday"].ToObject<int>(),
                info["postDate"]["hours"].ToObject<int>(),
                info["postDate"]["minutes"].ToObject<int>(),
                info["postDate"]["seconds"].ToObject<int>()),
                PicURLs = info["picsUrl"]?.ToString()
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
        private string GetDataFromAPI(string citySortName, int page)
        {
            string resource = $"gongyu?tabkey=allcity&action=getListInfo&curVer=8.6.5&appId=1&os=android&format=json&geotype=baidu&v=1&localname={citySortName}&page={page}";
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("user-agent", "okhttp/3.4.2");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "appgongyu.58.com");
            request.AddHeader("product", "58app");
            request.AddHeader("lat", "31.328703");
            request.AddHeader("r", "1920_1080");
            request.AddHeader("ua", "SM801");
            request.AddHeader("brand", "SMARTISAN");
            request.AddHeader("location", "2,6180,6348");
            request.AddHeader("58mac", "B4:0B:44:80:2E:B9");
            request.AddHeader("platform", "android");
            request.AddHeader("currentcid", "2");
            request.AddHeader("rnsoerror", "0");
            request.AddHeader("os", "android");
            request.AddHeader("owner", "baidu");
            request.AddHeader("m", "B4:0B:44:80:2E:B9");
            request.AddHeader("cid", "2");
            request.AddHeader("apn", "WIFI");
            request.AddHeader("58ua", "58app");
            request.AddHeader("nettype", "wifi");
            request.AddHeader("osarch", "arm64-v8a");
            request.AddHeader("productorid", "1");
            request.AddHeader("version", "8.6.5");
            request.AddHeader("bangbangid", "1080866410605347478");
            request.AddHeader("bundle", "com.wuba");
            request.AddHeader("maptype", "2");
            request.AddHeader("totalsize", "24.7");
            request.AddHeader("marketchannelid", "1593");
            request.AddHeader("osv", "5.1.1");
            request.AddHeader("lon", "121.39829");
            request.AddHeader("official", "true");
            IRestResponse response = _restClient.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }


    }
}
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
using System.IO;
using Microsoft.Extensions.Options;

namespace HouseMap.Crawler
{

    public class Hizhu : BaseCrawler
    {
        public Hizhu(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService,  RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.Hizhu;
        }


        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var city_code = JToken.Parse(config.Json)["city_code"].ToString();
            var pageno = page + 1;
            return GetData(city_code, pageno);

        }

        private static string GetData(string city_code, int pageno)
        {
            var client = new RestClient($"http://s.loulifang.com.cn/houselist.html?pageno={pageno}&city_code={city_code}&limit=50&sort=-1&money_max=999999&money_min=0&logicSort=0&region_id=&plate_id=&line_id=0&stand_id=0&type_no=0&search_id=&key=&key_self=0&latitude=&longitude=&distance=0&other_ids=&update_time=0");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "s.loulifang.com.cn");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/70.0.3538.80 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x26070333) NetType/WIFI Language/zh_CN Process/appbrand3");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("referer", "https://servicewechat.com/wx0ecb9bdbe4544574/27/page-frame.html");
            request.AddHeader("charset", "utf-8");
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var configJToken = JToken.Parse(config.Json);
            var pinyin = configJToken["pinyin"]?.ToString();
            var result = JToken.Parse(data);
            if (result?["status"]?.ToString() != "200" || result?["data"]?["house_list"].Count() == 0)
            {
                return houses;
            }
            foreach(var room in result["data"]["house_list"])
            {
                var onlineURL = $"http://www.hizhu.com/{pinyin}/roomDetail/{room["room_id"]?.ToString()}.html";
                var house = new DBHouse();
                house.OnlineURL = onlineURL;
                house.Source = SourceEnum.Hizhu.GetSourceName();
                house.Price = room["room_money"].ToObject<int>();
                house.Location = room["address"]?.ToString();
                house.Latitude = room["latitude"]?.ToString();
                house.Longitude = room["longitude"]?.ToString();
                house.Title = $"{room["region_name"]?.ToString()}区-{room["estate_name"]?.ToString()}-{room["room_num"]?.ToString()}居室-{room["room_name"]?.ToString()}";
                house.Text = $"{room["room_area"]?.ToString()}平米/{room["address"]?.ToString()}";
                house.PicURLs = Tools.GetPicURLs(room["main_img_path"]?.ToString());
                house.PubTime = Tools.JavaTimeStampToDateTime(room["page_time"].ToObject<long>()*1000);
                house.RentType =  GetRentType(room);
                house.JsonData = room.ToString();
                house.City = config.City;
                house.Id = Tools.GetGuid();
                houses.Add(house);
            }
            return houses;

        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            if (room["room_num"]?.ToString() == "1")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            else
            {
                rentType = (int)RentTypeEnum.Shared;
            }

            return rentType;
        }
    }
}
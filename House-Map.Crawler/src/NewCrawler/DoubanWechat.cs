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
using System.IO;

namespace HouseMap.Crawler
{

    public class DoubanWechat : NewBaseCrawler
    {

        private readonly string _key = "Ddg54q;sg]^3lka(72%2./as+d^823sD";
        public DoubanWechat(NewHouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = SourceEnum.DoubanWechat;
        }


        public override string GetJsonOrHTML(DbConfig config, int page)
        {
            var content = GetAPIResult(config, page);
            Console.WriteLine(content);
            return content;
        }

        private static string GetAPIResult(DbConfig config, int page)
        {
            try
            {
                var client = new RestClient($"https://fang.douban.com/api/topics?city={config.City}&district_tags=[]&subway_tags=[]&rent_type=&house_type=&bedroom_type=&rent_fee=[]&sort=&query_text=&page={page}&limit=50");
                var request = new RestRequest(Method.GET);
                request.AddHeader("host", "fang.douban.com");
                request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.91 Mobile Safari/537.36 MicroMessenger/6.7.2.1340(0x26070233) NetType/WIFI Language/zh_CN");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cookie", "bid=");
                request.AddHeader("referer", "https://servicewechat.com/wxaf9e2c0b8829cf6c/63/page-frame.html");
                request.AddHeader("authorization", "Bearer ");
                request.AddHeader("charset", "utf-8");
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DoubanWechat", ex, config);
                return string.Empty;
            }
        }
        public override List<DBHouse> ParseHouses(DbConfig config, string data)
        {
            Console.WriteLine(data);
            return new List<DBHouse>();
        }





        




       

    }

   
}
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
using System.Xml.Linq;
using System.Net;

namespace HouseMap.Crawler
{

    public class Fangtianxia : BaseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();
        public Fangtianxia(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService,  RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.Fangtianxia;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var configJson = JToken.Parse(config.Json);
            var city = WebUtility.UrlEncode(config.City);
            var fangPage = page + 1;
            var client = new RestClient("https://soufunappesf.3g.fang.com/http/sfservice.jsp?city=%E4%B8%8A%E6%B5%B7&gettype=android&housetype=jx&jkVersion=2&maptype=baidu&messagename=zflist&page=3&pagesize=20&purpose=%E4%BD%8F%E5%AE%85&subwayinfo=1&wirelesscode=AA4D0DFE6583C215AD7DBBC2BFAAD387");
            var request = new RestRequest(Method.GET);
            request.AddHeader("host", "soufunappesf.3g.fang.com");
            request.AddHeader("posmode", "gps%2Cwifi");
            request.AddHeader("company", "31009");
            request.AddHeader("model", "MIX");
            request.AddHeader("x1", "121.466702");
            request.AddHeader("imei", "861413030535382");
            request.AddHeader("y1", "31.206607");
            request.AddHeader("ispos", "1");
            request.AddHeader("iscard", "1");
            request.AddHeader("osversion", "8.0.0");
            request.AddHeader("wirelesscheckcode", "af52bb5e052444b975d871d6c87512f6f8c9772b4754c6ba");
            request.AddHeader("connmode", "Wifi");
            request.AddHeader("app-name", "Android_UnMap");
            request.AddHeader("version", "8.7.0");
            request.AddHeader("networktype", "wifi");
            request.AddHeader("language", "zh_CN");
            request.AddHeader("user-agent", "Android_UnMap%7EMIX%7E8.0.0");
            request.AddHeader("city", "%E4%B8%8A%E6%B5%B7");
            request.AddHeader("shop-project", "fang-app-android");
            request.AddHeader("pagesc", "zflist");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var xDoc = XDocument.Parse(data);
            var houses = new List<DBHouse>();
            var domain = JToken.Parse(config.Json)["domain"]?.ToString();
            foreach (var element in xDoc.Element("houses").Elements().Where(e => e.Name == "houseinfo"))
            {
                var house = new DBHouse();
                house.Id = Tools.GetGuid();
                var houseFields = element.Elements();
                var houseId = GetFieldValue(houseFields, "houseid");
                house.OnlineURL = $"http://{domain}/chuzu/1_{houseId}_-1.htm";
                house.City = GetFieldValue(houseFields, "city");
                house.Location = GetFieldValue(houseFields, "address");
                house.Longitude = GetFieldValue(houseFields, "coord_x");
                house.Latitude = GetFieldValue(houseFields, "coord_y");
                house.Price = int.Parse(GetFieldValue(houseFields, "price"));
                house.Title = GetFieldValue(houseFields, "title");
                house.PicURLs = JsonConvert.SerializeObject(new List<string>() { GetFieldValue(houseFields, "titleimage") });
                house.PubTime = DateTime.Parse(GetFieldValue(houseFields, "addtime"));
                house.JsonData = element.ToString();
                house.Title = GetTitle(houseFields); ;
                house.Tags = GetFieldValue(houseFields, "tags");
                house.RentType = GetRentType(houseFields);
                house.Source = SourceEnum.Fangtianxia.GetSourceName();
                Console.WriteLine(house.Title);
                houses.Add(house);
            }
            return houses;
        }

        private int GetRentType(IEnumerable<XElement> houseFields)
        {
            var rentType = 0;
            var rentway = GetFieldValue(houseFields, "rentway");
            if (rentway == "整租")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            else if (rentway.Contains("合租"))
            {
                rentType = (int)RentTypeEnum.Shared;
            }

            return rentType;
        }

        private string GetTitle(IEnumerable<XElement> houseFields)
        {
            var title = $"【{GetFieldValue(houseFields, "district")}区】【{GetFieldValue(houseFields, "comarea")}地区】【{GetFieldValue(houseFields, "projname")}】【{GetFieldValue(houseFields, "address")}】";
            if (!string.IsNullOrEmpty(GetFieldValue(houseFields, "subway")))
            {
                title = title + $"，临近【{GetFieldValue(houseFields, "subway")}】/【{GetFieldValue(houseFields, "subwaydistance")}】";
            }
            var forward = GetFieldValue(houseFields, "forward");
            if (forward != "朝")
            {
                title = title + $"【{(forward.Length > 1 ? forward + "通透" : "朝" + forward)}】";
            }

            return title;
        }

        private string GetFieldValue(IEnumerable<XElement> houseFields, string field)
        {
            return houseFields.FirstOrDefault(i => i.Name == field)?.Value;
        }


    }
}
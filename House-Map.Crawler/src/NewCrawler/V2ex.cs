using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using Dapper;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;
using HouseMap.Common;
using Newtonsoft.Json.Linq;
using HouseMap.Crawler.Service;
using System.Net;
using AngleSharp.Parser.Html;

namespace HouseMap.Crawler
{

    public class V2ex : NewBaseCrawler
    {

        private static HtmlParser htmlParser = new HtmlParser();

        public V2ex(NewHouseDapper houseDapper, ConfigDapper configDapper,
         ElasticService elastic) : base(houseDapper, configDapper, elastic)
        {
            this.Source = SourceEnum.V2ex;

        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var pinyin = jsonData["pinyin"].ToString();
            var client = new RestClient($"https://www.v2ex.com/go/{pinyin}?p={page + 1}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("authority", "www.v2ex.com");
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var jsonData = JToken.Parse(config.Json);
            var city = config.City;
            var htmlDoc = htmlParser.Parse(data);
            var topics = htmlDoc?.QuerySelector("#TopicsNode")?.QuerySelectorAll("div");
            if (topics == null || topics.Count() == 0)
            {
                return houses;
            }
            foreach (var topic in topics)
            {
                var title = topic.QuerySelector("span.item_title")?.QuerySelector("a");
                if (CheckTopic(title))
                {
                    var house = new DBHouse();
                    house.Title = title?.TextContent;
                    house.Location = title?.TextContent;
                    var path = title.GetAttribute("href");
                    if (path.Contains("#"))
                    {
                        path = path.Split("#").First();
                    }
                    house.OnlineURL = $"https://www.v2ex.com{path}";
                    house.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(new { html = topic.OuterHtml });
                    house.Source = SourceEnum.V2ex.GetSourceName();
                    house.Price = JiebaTools.GetHousePrice(title?.TextContent);
                    house.Id = Tools.GetGuid();
                    house.City = city;
                    house.PicURLs = Tools.GetPicURLs("");
                    house.PubTime = DateTime.Now;
                    houses.Add(house);
                }
            }
            FillGoodHouseLocation(city, houses);
            return houses;
        }

        private static bool CheckTopic(AngleSharp.Dom.IElement title)
        {
            return !string.IsNullOrEmpty(title?.TextContent)
                            && (title.TextContent.Contains("出租")
                            || title.TextContent.Contains("转租")
                            || title.TextContent.Contains("直租")
                            || title.TextContent.Contains("求室友")
                            || title.TextContent.Contains("招室友")
                            || title.TextContent.Contains("找室友"));
        }

        private static void FillGoodHouseLocation(string city, List<DBHouse> goodHouses)
        {
            for (int page = 0; page <= goodHouses.Count / 10; page++)
            {
                var dhHouses = goodHouses.Skip(page * 10).Take(10).ToList();
                try
                {
                    if (dhHouses.Count == 0)
                    {
                        break;
                    }
                    JToken geocodes = GetGeocodes(city, dhHouses);
                    if (geocodes == null || geocodes.Count() == 0 || geocodes.Count() != dhHouses.Count())
                    {
                        continue;
                    }
                    for (var index = 0; index < dhHouses.Count(); index++)
                    {
                        var house = dhHouses[index];
                        var geocode = geocodes[index];
                        FillHousePriceAndLocation(house, geocode);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("FillGoodHouseLocation", ex, city);
                }

            }
        }

        private static void FillHousePriceAndLocation(DBHouse house, JToken geocode)
        {
            if (geocode["location"].ToString().Split(",").Count() == 2)
            {
                house.Longitude = geocode["location"].ToString().Split(",")[0];
                house.Latitude = geocode["location"].ToString().Split(",")[1];
                house.Location = geocode["formatted_address"].ToString();
                house.Tags = geocode["district"].ToString();
            }
            house.UpdateTime = DateTime.Now;
            if (geocode["location"].ToString().Split(",").Count() < 2 && house.Price == 0)
            {
                house.Status = (int)HouseStatusEnum.Deleted;
            }
        }

        private static JToken GetGeocodes(string city, List<DBHouse> houses)
        {
            var address = string.Join("|", houses.Select(h =>
            {
                return h.Title.Replace("|", "").Replace(" ", "").Trim();
            }));
            var client = new RestClient($"https://restapi.amap.com/v3/geocode/geo?address={address}&output=json&key=478aea7d5ba0cfb604106db0a33c7119&city={city}&batch=true");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var geocodes = JToken.Parse(response.Content)?["geocodes"];
            return geocodes;
        }
    }
}
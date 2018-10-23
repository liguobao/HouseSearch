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
using Microsoft.Extensions.Options;
using HouseMap.Crawler.Service;

namespace HouseMap.Crawler
{

    public class Douban : NewBaseCrawler
    {
        private readonly HouseMapContext _houseDataContext;

        public Douban(NewHouseDapper houseDapper, ConfigDapper configDapper, IOptions<AppSettings> configuration,
         HouseMapContext houseDataContext, ElasticService elasticsearch)
        : base(houseDapper, configDapper, elasticsearch)
        {
            this.Source = SourceEnum.Douban;
            _houseDataContext = houseDataContext;
        }


        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var groupID = jsonData["groupid"]?.ToString();
            var apiURL = $"https://api.douban.com/v2/group/{groupID}/topics?start={page * 50}&count=100";
            return GetAPIResult(apiURL);

        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            var lowHouses = new List<DBHouse>();
            var goodHouses = new List<DBHouse>();
            FillHouses(resultJObject, config.City, lowHouses, goodHouses);
            FillGoodHouseLocation(config.City, goodHouses);
            lowHouses.AddRange(goodHouses);
            return lowHouses;
        }

        private static void FillGoodHouseLocation(string city, List<DBHouse> goodHouses)
        {
            for (int page = 0; page < goodHouses.Count / 10; page++)
            {
                var dhHouses = goodHouses.Skip(page * 10).Take(10).ToList();
                try
                {
                    if (dhHouses.Count == 0)
                    {
                        break;
                    }
                    JToken geocodes = GetGeocodes(city, dhHouses);
                    if (geocodes == null || geocodes.Count() == 0)
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
                    LogHelper.Error("AnalyzeHouse", ex, city);
                }

            }
        }

        private void FillHouses(JObject resultJObject, string city, List<DBHouse> lowHouses, List<DBHouse> goodHouses)
        {
            foreach (var topic in resultJObject["topics"])
            {
                DBHouse house = ConvertToHouse(city, topic);
                if (ProCheck(topic) || string.IsNullOrEmpty(GetGeoText(house)))
                {
                    // 基本可以判定为无用的信息,回头可以考虑作为样本去做机器学习
                    house.Status = (int)HouseStatusEnum.LowGrade;
                    lowHouses.Add(house);
                }
                else
                {
                    goodHouses.Add(house);
                }
            }
        }

        private static bool ProCheck(JToken topic)
        {
            return topic["photos"]?.Select(photo => photo["alt"].ToString()).Any() != true
                            || string.IsNullOrEmpty(topic["content"]?.ToString()) ||
                            topic["content"].ToString().Count() <= 20;
        }

        private DBHouse ConvertToHouse(string city, JToken topic)
        {
            var housePrice = -1;
            var photos = topic["photos"]?.Select(photo => photo["alt"].ToString()).ToList();
            var house = new DBHouse()
            {
                Id = Tools.GetUUId(),
                Location = topic["title"].ToString(),
                Title = topic["title"].ToString(),
                OnlineURL = topic["share_url"].ToString(),
                Text = topic["content"]?.ToString(),
                JsonData = topic.ToString(),
                Price = housePrice,
                Source = SourceEnum.Douban.GetSourceName(),
                City = city,
                RentType = GetRentType(topic["content"].ToString()),
                PicURLs = JsonConvert.SerializeObject(photos),
                PubTime = topic["created"].ToObject<DateTime>(),
            };
            return house;
        }

        public int GetRentType(string content)
        {
            if (content == null)
            {
                return (int)RentTypeEnum.Undefined;
            }
            if (content.Contains("单间") || content.Contains("一室户") || content.Contains("1室户") || content.Contains("1室1厅"))
            {
                return (int)RentTypeEnum.OneRoom;
            }
            else if (content.Contains("合租") || content.Contains("找室友") || content.Contains("招室友") || content.Contains("室友"))
            {
                return (int)RentTypeEnum.Shared;
            }
            else if (content.Contains("整租") || content.Contains("房东出租"))
            {
                return (int)RentTypeEnum.AllInOne;
            }
            return (int)RentTypeEnum.Undefined;
        }

        private static string GetAPIResult(string apiURL)
        {
            try
            {
                var client = new RestClient(apiURL);
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("connection", "keep-alive");
                request.AddHeader("x-requested-with", "XMLHttpRequest");
                request.AddHeader("referer", apiURL);
                request.AddHeader("accept", "*/*");
                request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("cookie", "bid=qLvbOle-G58; ps=y; ue=^\\^codelover^@qq.com^^; push_noty_num=0; push_doumail_num=0; __utmz=30149280.1521636704.1.1.utmcsr=(direct)^|utmccn=(direct)^|utmcmd=(none); __utmv=30149280.15460; ll=^\\^108296^^; _vwo_uuid_v2=D87414308A33790472DBB4D2B1DC0DE7B^|6a9fc300e5ea8c7485f9a922d87e820b; __utma=30149280.2046746446.1521636704.1522567163.1522675073.3");
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return response.Content;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAPIResult", ex);
            }
            return "";

        }


        public override void AnalyzeHouse(DateTime fromDate, DateTime toDate)
        {
            var cities = _configDapper.LoadBySource("douban").Select(c => c.City).Distinct();
            foreach (var city in cities)
            {
                LogHelper.Info($"AnalyzeHouse-{city}-start");
                var houseQuery = _houseDataContext.DoubanHouses
                .Where(h => h.Price == -1 && h.CreateTime >= fromDate.Date && h.CreateTime <= toDate.Date && h.City == city && h.Status == (int)HouseStatusEnum.Created);
                for (var page = 0; page <= 100; page++)
                {
                    try
                    {
                        var houses = houseQuery.Skip(page * 10).Take(10).OrderBy(h => h.CreateTime).ToList();
                        if (houses.Count == 0)
                        {
                            break;
                        }
                        JToken geocodes = GetGeocodes(city, houses);
                        if (geocodes == null || geocodes.Count() == 0)
                        {
                            continue;
                        }
                        for (var index = 0; index < houses.Count(); index++)
                        {
                            var house = houses[index];
                            var geocode = geocodes[index];
                            FillHousePriceAndText(house, geocode);
                        }
                        _houseDataContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("AnalyzeHouse", ex, city);
                    }

                }
                LogHelper.Info($"AnalyzeHouse-{city}-finish");
            }

        }

        private static void FillHousePriceAndText(DoubanHouse house, JToken geocode)
        {
            if (geocode["location"].ToString().Split(",").Count() == 2)
            {
                house.Longitude = geocode["location"].ToString().Split(",")[0];
                house.Latitude = geocode["location"].ToString().Split(",")[1];
                house.Location = geocode["formatted_address"].ToString();
                house.Tags = geocode["district"].ToString();
            }
            house.Text = RemoveImgLabels(house.Text, house.Pictures.Count);
            house.Price = GetHousePrice(house);
            house.UpdateTime = DateTime.Now;
            if (geocode["location"].ToString().Split(",").Count() < 2 && house.Price == 0)
            {
                house.Status = (int)HouseStatusEnum.Deleted;
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
            house.Text = RemoveImgLabels(house.Text, house.Pictures.Count);
            house.Price = GetHousePrice(house);
            house.UpdateTime = DateTime.Now;
            if (geocode["location"].ToString().Split(",").Count() < 2 && house.Price == 0)
            {
                house.Status = (int)HouseStatusEnum.Deleted;
            }
        }

        private static JToken GetGeocodes(string city, List<DoubanHouse> houses)
        {
            var address = string.Join("|", houses.Select(h =>
            {
                return GetGeoText(h);
            }));
            var client = new RestClient($"https://restapi.amap.com/v3/geocode/geo?address={address}&output=json&key=fed53efe358677305ad9a9cad2b93b8b&city={city}&batch=true");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var geocodes = JToken.Parse(response.Content)?["geocodes"];
            return geocodes;
        }

        private static JToken GetGeocodes(string city, List<DBHouse> houses)
        {
            var address = string.Join("|", houses.Select(h =>
            {
                return GetGeoText(h);
            }));
            var client = new RestClient($"https://restapi.amap.com/v3/geocode/geo?address={address}&output=json&key=fed53efe358677305ad9a9cad2b93b8b&city={city}&batch=true");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                LogHelper.Info("GetGeocodes error,response:" + response.ErrorMessage);
                return null;
            }
            var geocodes = JToken.Parse(response.Content)?["geocodes"];
            return geocodes;
        }


        private static string GetGeoText(DoubanHouse h)
        {
            var text = "";
            if (!string.IsNullOrEmpty(h.Text) && !h.Text.Contains("www.douban.com"))
            {
                text = RemoveImgLabels(h.Text, h.Pictures.Count);
                if (string.IsNullOrEmpty(text))
                {
                    text = h.Title;
                }
            }
            else
            {
                text = h.Title;
            }
            text = text.Replace("\n", "").Replace("|", "");
            if (text.Count() >= 100)
            {
                return text.Substring(0, 100);
            }
            return text;
        }

        private static string GetGeoText(DBHouse h)
        {
            var text = "";
            if (!string.IsNullOrEmpty(h.Text) && !h.Text.Contains("www.douban.com"))
            {
                text = RemoveImgLabels(h.Text, h.Pictures.Count);
                if (string.IsNullOrEmpty(text))
                {
                    text = h.Title;
                }
            }
            else
            {
                text = h.Title;
            }
            text = text.Replace("\n", "").Replace("|", "");
            if (text.Count() >= 100)
            {
                return text.Substring(0, 100);
            }
            return text;
        }

        private static string RemoveImgLabels(string text, int picturesCount)
        {
            for (var index = 1; index <= picturesCount; index++)
            {
                text = text.Replace($"<图片{index}>", "");
            }
            return text.Replace("|", "");
        }

        private static int GetHousePrice(DoubanHouse house)
        {
            var price = JiebaTools.GetHousePrice(house.Title);
            if (price == 0 && !string.IsNullOrEmpty(house.Text))
            {
                price = JiebaTools.GetHousePrice(house.Text);
            }

            return price;
        }

        private static int GetHousePrice(DBHouse house)
        {
            var price = JiebaTools.GetHousePrice(house.Title);
            if (price == 0 && !string.IsNullOrEmpty(house.Text))
            {
                price = JiebaTools.GetHousePrice(house.Text);
            }

            return price;
        }
    }


}
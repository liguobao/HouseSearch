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
        private readonly HouseDapper _oldHouseDapper;

        private readonly ElasticsearchService _elasticsearch;

        public Douban(NewHouseDapper houseDapper, ConfigDapper configDapper, IOptions<AppSettings> configuration,
         HouseDapper oldHouseDapper, ElasticsearchService elasticsearch)
        : base(houseDapper, configDapper)
        {
            this.Source = SourceEnum.Douban;
            _oldHouseDapper = oldHouseDapper;
            _elasticsearch = elasticsearch;
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
            var houses = new List<DBHouse>();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            var city = config.City;
            foreach (var topic in resultJObject["topics"])
            {
                // todo 
                //var housePrice = JiebaTools.GetHousePrice(topic["content"].ToString());
                DBHouse house = ConvertToHouse(city, topic);
                houses.Add(house);
            }
            return houses;
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
                Text = topic["content"].ToString(),
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

        public override void SyncHouses()
        {
            foreach (var config in _configDapper.LoadBySource(SourceEnum.Douban.GetSourceName()).GroupBy(c => c.City))
            {
                LogHelper.RunActionNotThrowEx(() =>
                {

                    List<HouseInfo> oldHouses = _oldHouseDapper.SearchHouses(new HouseCondition()
                    {
                        Source = SourceEnum.Douban.GetSourceName(),
                        IntervalDay = 1000,
                        HouseCount = 300000,
                        CityName = config.Key
                    }).ToList();
                    if (oldHouses == null)
                    {
                        return;
                    }
                    LogHelper.Info($"{config.Key} SyncHouse start,count={oldHouses.Count}");
                    var houses = new List<DBHouse>();
                    foreach (var house in oldHouses)
                    {
                        var one = new DBHouse()
                        {
                            Id = Tools.GetUUId(),
                            Title = house.HouseTitle,
                            Text = house.HouseText,
                            Location = house.HouseLocation,
                            City = house.LocationCityName,
                            PicURLs = house.PicURLs,
                            Price = (int)house.HousePrice,
                            RentType = GetRentType(house.HouseText),
                            PubTime = house.PubTime,
                            CreateTime = house.DataCreateTime,
                            Source = SourceEnum.Douban.GetSourceName(),
                            OnlineURL = house.HouseOnlineURL,
                        };
                        houses.Add(one);
                    }
                    var result = _houseDapper.BulkInsertHouses(houses);
                    LogHelper.Info($"{config.Key} SyncHouse finish,result:{result}");
                }, "SyncHouse", config.FirstOrDefault());

            }


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









    }


}
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
using System.Threading;
using Microsoft.Extensions.Options;

namespace HouseMap.Crawler
{

    public class Douban : BaseCrawler
    {
        private readonly RestClient _restClient;

        public Douban(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticsearch, RedisTool redisTool)
        : base(houseDapper, configDapper, elasticsearch, redisTool)
        {
            this.Source = SourceEnum.Douban;
            _restClient = new RestClient("http://10.3.255.179");
            //_restClient = new RestClient("https://api.douban.com");
        }


        public override string GetJsonOrHTML(DBConfig config, int page)
        {

            var jsonData = JToken.Parse(config.Json);
            var groupID = jsonData["groupid"]?.ToString();
            try
            {
                var request = new RestRequest($"/douban/groups?groupId={groupID}&page={page}", Method.GET);
                request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
                IRestResponse response = _restClient.Execute(request);
                if (response.IsSuccessful)
                {
                    return response.Content;
                }
                else
                {
                    Console.WriteLine($"GetJsonOrHTML fail, request:{request.Resource},response.Content:{response.Content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAPIResult:config:{config},ex:{ex.ToString()}");
            }
            return "";

        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            if (string.IsNullOrEmpty(data) || !data.IsValidJson())
            {
                Console.WriteLine($"data 无效, config:{JToken.FromObject(config).ToString()},data:{data}");
                return new List<DBHouse>();
            }
            var resultJObject = JToken.Parse(data);
            var houses = new List<DBHouse>();
            if (resultJObject["topics"] == null)
            {
                Console.WriteLine($"data 无效, config:{JToken.FromObject(config).ToString()},data:{data}");
                return new List<DBHouse>();
            }
            foreach (var topic in resultJObject["topics"])
            {
                DBHouse house = ConvertToHouse(config.City, topic);
                house.Status = ProCheck(topic) ? (int)HouseStatusEnum.LowGrade : (int)HouseStatusEnum.Created;
                houses.Add(house);
            }
            return houses;
        }

        private static bool ProCheck(JToken topic)
        {
            return topic["photos"]?.Select(photo => photo["alt"].ToString()).Any() != true
                            || string.IsNullOrEmpty(topic["content"]?.ToString()) ||
                            topic["content"].ToString().Count() <= 20;

        }

        private DBHouse ConvertToHouse(string city, JToken topic)
        {
            var photos = topic["photos"]?.Select(photo => photo["alt"].ToString()).ToList();
            var house = new DBHouse()
            {
                Id = Tools.GetGuid(),
                Location = topic["title"].ToString(),
                Title = topic["title"].ToString(),
                OnlineURL = topic["share_url"].ToString(),
                Text = topic["content"]?.ToString(),
                JsonData = topic.ToString(),
                Source = SourceEnum.Douban.GetSourceName(),
                City = city,
                RentType = GetRentType(topic["content"].ToString()),
                PicURLs = JsonConvert.SerializeObject(photos),
                PubTime = topic["created"].ToObject<DateTime>(),
            };
            house.Price = -1;
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

        public override void AnalyzeData()
        {
            var houses = _houseDapper.FindDoubanNotPriceData();
            if (houses == null || houses.Count == 0)
            {
                return;
            }
            Console.WriteLine($"待处理数据总量:{houses?.Count}");
            foreach (var house in houses)
            {
                Console.WriteLine($"[{DateTime.Now}]|正在分析【{house.Title}】");
                house.Price = GetHousePrice(house);
            }
            _houseDapper.UpdateDoubanPrices(houses);
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
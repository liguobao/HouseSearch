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
using System.Web;

namespace HouseMap.Crawler
{

    public class Douban : BaseCrawler
    {
        private readonly RestClient _restClient;

        public Douban(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticsearch, RedisTool redisTool)
        : base(houseDapper, configDapper, elasticsearch, redisTool)
        {
            this.Source = SourceEnum.Douban;
            _restClient = new RestClient();
            _restClient.BaseUrl = new Uri("https://frodo.douban.com");
            _restClient.Timeout = 120 * 1000;
            _restClient.UserAgent = "api-client/1 com.douban.frodo/7.1.0(205) Android/29 product/perseus vendor/Xiaomi model/Mi MIX 3  rom/miui6  network/wifi  udid/a0f9cde79ec841a748625f766273e8f4333ed9c1  platform/mobile nd/1";
            //_restClient = new RestClient("https://api.douban.com");
        }

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        public string HMACSHA1Text(string text, string key)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }

        private string BuildDoubanSign(string groupID, long tsMillisecond)
        {
            var signURL = $"GET&%2Fapi%2Fv2%2Fgroup%2F{groupID}%2Ftopics&{tsMillisecond}";
            var _sig = HttpUtility.UrlEncode(HMACSHA1Text(signURL, "aaabbbccc"));
            return _sig;
        }



        public override string GetJsonOrHTML(DBConfig config, int page)
        {

            var jsonData = JToken.Parse(config.Json);
            var groupID = jsonData["groupid"]?.ToString();
            var startIndex = page * 100;
            try
            {
                var tsMillisecond = Tools.GetMillisecondTimestamp() / 1000;
                string _sig = BuildDoubanSign(groupID, tsMillisecond);
                var resource = $"/api/v2/group/{groupID}/topics?start={startIndex}&count=100&sortby=new&apple=11e8961cf7ae829bd1e80ae5941533dd&mooncake=0f607264fc6318a92b9e13c65db7cd3c&webview_ua=Mozilla%2F5.0%20%28Linux%3B%20Android%2010%3B%20Mi%20MIX%203%20Build%2FQKQ1.190828.002%3B%20wv%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Version%2F4.0%20Chrome%2F88.0.4324.181%20Mobile%20Safari%2F537.36&screen_width=1080&screen_height=2296&sugar=46001&longitude=121.46027&latitude=31.200183&apikey=0dad551ec0f84ed02907ff5c42e8ec70&channel=DoubanTest&udid=a0f9cde79ec841a748625f766273e8f4333ed9c1&os_rom=miui6&timezone=Asia%2FShanghai&_sig={_sig}&_ts={tsMillisecond}";
                var restRequest = new RestRequest(resource, Method.GET).AddHeader("Authorization", "");
                restRequest.Timeout = 60 * 1000;
                IRestResponse response = _restClient.Execute(restRequest);
                if (response.IsSuccessful)
                {
                    return response.Content;
                }
                else
                {
                    Console.WriteLine($"GetJsonOrHTML fail, request:{restRequest.Resource},response.Content:{response.Content}");
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
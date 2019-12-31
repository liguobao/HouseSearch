using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using Dapper;
using AngleSharp.Dom;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;
using Newtonsoft.Json.Linq;
using HouseMap.Common;

namespace HouseMap.Crawler
{

    public class AnXuan : BaseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        private readonly RestClient _restClient;
        public AnXuan(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService, RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.Anxuan;
            _restClient = new RestClient();
            _restClient.BaseUrl = new Uri("http://192.168.1.2");
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var configJson = JToken.Parse(config.Json);
            var shortCutName = configJson["shortcutname"].ToString();
            var pageIndex = page + 1;
            var request = new RestRequest($"/58-anxuan?city={shortCutName}&page={pageIndex}", Method.GET);
            IRestResponse response = _restClient.Execute(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine(response.Content);
            }
            return response.IsSuccessful ? response.Content : "";
        }


        public override List<DBHouse> ParseHouses(DBConfig config, string jsonOrHTML)
        {
            var houses = new List<DBHouse>();
            var configJson = JToken.Parse(config.Json);
            foreach (var item in JToken.Parse(jsonOrHTML))
            {
                var house = new DBHouse();
                house.Id = Tools.GetGuid();
                house.City = config.City;
                house.Title = item["title"].ToString();
                house.Location = item["title"].ToString();
                house.Text = item["text"].ToString();
                house.PicURLs = Tools.GetPicURLs(item["pic_url"].ToString());
                house.OnlineURL = item["link"].ToString();
                house.PubTime = DateTime.Now;
                house.Source = SourceEnum.Anxuan.GetSourceName();
                house.Price = item["price"].ToObject<int>();
                house.RentType = GetRentType(item["title"].ToString());
                houses.Add(house);
            }
            return houses;
        }

        private static int GetRentType(string title)
        {
            var rentType = 0;
            if (title.Contains("整租"))
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            else if (title.Contains("合租"))
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (title.Contains("1室"))
            {
                rentType = (int)RentTypeEnum.OneRoom;
            }
            return rentType;
        }

    }
}
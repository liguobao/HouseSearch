using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using AngleSharp.Dom;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;
using Newtonsoft.Json.Linq;
using HouseMap.Common;
using HouseMap.Crawler.Service;

namespace HouseMap.Crawler
{
    public class NewBaseCrawler : INewCrawler
    {

        protected SourceEnum Source;

        protected readonly ConfigDapper _configDapper;

        protected NewHouseDapper _houseDapper;

        protected ElasticService _elasticService;


        public NewBaseCrawler(NewHouseDapper houseDapper, ConfigDapper configDapper,ElasticService elasticService)
        {
            this._houseDapper = houseDapper;
            this._configDapper = configDapper;
            _elasticService = elasticService;
        }

        public virtual string GetJsonOrHTML(DBConfig config, int page)
        {
            throw new NotImplementedException();
        }

        public virtual List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            throw new NotImplementedException();
        }



        public void Run()
        {

            foreach (var config in _configDapper.LoadBySource(Source.GetSourceName()))
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    for (var pageNum = 0; pageNum < config.PageCount; pageNum++)
                    {
                        var htmlOrJson = GetJsonOrHTML(config, pageNum);
                        if (string.IsNullOrEmpty(htmlOrJson))
                        {
                            return;
                        }
                        var houses = ParseHouses(config, htmlOrJson);
                        _houseDapper.BulkInsertHouses(houses);
                        _elasticService.SaveHouses(houses);
                    }
                }, Source.GetSourceName(), config);
            }
        }

        SourceEnum INewCrawler.GetSource()
        {
            return Source;
        }

        public virtual void SyncHouses()
        {
            throw new NotImplementedException();
        }

        public virtual void AnalyzeHouse(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }



        protected void FillGoodHouseLocation(string key,string city, List<DBHouse> goodHouses)
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
                    JToken geocodes = GetGeocodes(key,city, dhHouses);
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

        private static JToken GetGeocodes(string key,string city, List<DBHouse> houses)
        {
            var address = string.Join("|", houses.Select(h =>
            {
                return h.Title.Replace("|", "").Replace(" ", "").Trim();
            }));
            var client = new RestClient($"https://restapi.amap.com/v3/geocode/geo?address={address}&output=json&key={key}&city={city}&batch=true");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var geocodes = JToken.Parse(response.Content)?["geocodes"];
            return geocodes;
        }
  
    }
}
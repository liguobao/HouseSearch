using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using AngleSharp.Dom;
using HouseMap.Crawler.Dapper;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.DBEntity;
using Newtonsoft.Json.Linq;

namespace HouseMap.Crawler
{
    public class BaseCrawler
    {

        protected string Source;
        private ConfigDapper _configDapper;

        private HouseDapper _houseDapper;

        public BaseCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this._houseDapper = houseDapper;
            this._configDapper = configDapper;
        }

        public virtual string GetJsonOrHTML(JToken config, int page)
        {
            throw new NotImplementedException();
        }

        public virtual List<BaseHouseInfo> ParseHouses(JToken config, string data)
        {
            throw new NotImplementedException();
        }



        public void Run()
        {

            foreach (var config in _configDapper.GetList(Source))
            {
                var confInfo = JsonConvert.DeserializeObject<JToken>(config.ConfigurationValue);
                for (var pageNum = 1; pageNum < confInfo["pagecount"].ToObject<int>(); pageNum++)
                {
                    var htmlOrJson = GetJsonOrHTML(confInfo, pageNum);
                    var houses = ParseHouses(confInfo, htmlOrJson);
                    _houseDapper.BulkInsertHouses(houses);
                }
            }
        }
    }
}
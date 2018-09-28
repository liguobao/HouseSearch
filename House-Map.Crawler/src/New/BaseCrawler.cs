using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using AngleSharp.Dom;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;

using Newtonsoft.Json.Linq;
using HouseMap.Models;

namespace HouseMap.Crawler
{
    public class BaseCrawler
    {

        protected string Source;

        private readonly ConfigService _configService;

        private HouseDapper _houseDapper;

        public BaseCrawler(HouseDapper houseDapper, ConfigService configService)
        {
            this._houseDapper = houseDapper;
            this._configService = configService;
        }

        public virtual string GetJsonOrHTML(JToken config, int page)
        {
            throw new NotImplementedException();
        }

        public virtual List<HouseInfo> ParseHouses(JToken config, string data)
        {
            throw new NotImplementedException();
        }



        public void Run()
        {

            foreach (var config in _configService.LoadBySource(Source))
            {
                var confInfo = JsonConvert.DeserializeObject<JToken>(config.Json);
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
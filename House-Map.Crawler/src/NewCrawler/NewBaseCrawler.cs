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
using HouseMap.Common;

namespace HouseMap.Crawler
{
    public class NewBaseCrawler : INewCrawler
    {

        protected SourceEnum Source;

        private readonly ConfigDapper _configDapper;

        private NewHouseDapper _houseDapper;

        public NewBaseCrawler(NewHouseDapper houseDapper, ConfigDapper configDapper)
        {
            this._houseDapper = houseDapper;
            this._configDapper = configDapper;
        }

        public virtual string GetJsonOrHTML(DbConfig config, int page)
        {
            throw new NotImplementedException();
        }

        public virtual List<DBHouse> ParseHouses(DbConfig config, string data)
        {
            throw new NotImplementedException();
        }



        public void Run()
        {

            foreach (var config in _configDapper.LoadBySource(Source.GetSourceName()))
            {
                for (var pageNum = 1; pageNum < config.PageCount; pageNum++)
                {
                    var htmlOrJson = GetJsonOrHTML(config, pageNum);
                    if (string.IsNullOrEmpty(htmlOrJson))
                    {
                        continue;
                    }
                    var houses = ParseHouses(config, htmlOrJson);
                    _houseDapper.BulkInsertHouses(houses);
                }
            }
        }

        SourceEnum INewCrawler.GetSource()
        {
            return Source;
        }
    }
}
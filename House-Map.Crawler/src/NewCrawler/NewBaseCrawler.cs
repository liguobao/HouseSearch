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
    }
}
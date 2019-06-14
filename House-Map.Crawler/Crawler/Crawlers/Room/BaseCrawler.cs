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
using System.Globalization;

namespace HouseMap.Crawler
{
    public class BaseCrawler : IHouseCrawler
    {

        protected SourceEnum Source;

        protected readonly ConfigDapper _configDapper;

        protected readonly HouseDapper _houseDapper;

        protected readonly ElasticService _elasticService;

        private readonly RedisTool _redisTool;


        public BaseCrawler(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService, RedisTool redisTool)
        {
            this._houseDapper = houseDapper;
            this._configDapper = configDapper;
            _elasticService = elasticService;
            _redisTool = redisTool;
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
            Console.WriteLine($"【{Source.GetSourceName()}】 running");
            _redisTool.WriteHash(RedisKeys.CurrentCrawler, Source.GetSourceName(), "running");
            var configs = _configDapper.LoadBySource(Source.GetSourceName());
            Console.WriteLine($"configs count:{configs.Count}");
            foreach (var config in configs)
            {
                try
                {
                    Console.WriteLine($"开始抓取【{config.City}】房源数据.");
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    var crawlerKey = RedisKeys.CrawlerState.CopyOne(config.Source);
                    _redisTool.WriteHash(crawlerKey, config.City, "running");
                    for (var pageNum = 0; pageNum < config.PageCount; pageNum++)
                    {
                        var htmlOrJson = GetJsonOrHTML(config, pageNum);
                        if (string.IsNullOrEmpty(htmlOrJson))
                        {
                            Console.WriteLine($"[{DateTime.Now}]|当前页数:{pageNum}抓取失败.");
                            break;
                        }
                        var houses = ParseHouses(config, htmlOrJson);
                        Console.WriteLine($"[{DateTime.Now}]|当前页数:{pageNum},共抓取:{houses.Count}条数据.");
                        _houseDapper.BulkInsertHouses(houses);
                        _elasticService.SaveHouses(houses);
                    }
                    sw.Stop();
                    var time = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
                    Console.WriteLine($"完成{config.City}房源数据抓取, 耗时:{time}");
                    _redisTool.WriteHash(crawlerKey, config.City, "finish");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ex:{ex.ToString()},config:{config.City}");
                }
            }
            _redisTool.WriteHash(RedisKeys.CurrentCrawler, Source.GetSourceName(), "stop");
            Console.WriteLine($"{Source.GetSourceName()} stop");
        }

        SourceEnum IHouseCrawler.GetSource()
        {
            return Source;
        }

        public virtual void AnalyzeData()
        {
            throw new NotImplementedException();
        }
    }
}
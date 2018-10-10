using System;
using System.Collections.Generic;
using System.Linq;
using HouseMap.Models;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Common;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace HouseMap.Dao
{
    public class ConfigService
    {
        private readonly RedisTool _redisTool;

        private readonly ConfigDapper _configDapper;

        public ConfigService(RedisTool RedisTool, ConfigDapper configDapper)
        {
            this._redisTool = RedisTool;
            _configDapper = configDapper;
        }

        public List<DBConfig> LoadConfigs(string city = "")
        {
            var configs = _redisTool.ReadCache<List<DBConfig>>(RedisKey.CrawlerConfig.Key + city, RedisKey.CrawlerConfig.DBName);
            if (configs == null)
            {
                var configQuery = _configDapper.LoadAll(city);
                configs = configQuery.OrderByDescending(c => c.Score).ToList();
                _redisTool.WriteObject(RedisKey.CrawlerConfig.Key + city, configs,
                 RedisKey.CrawlerConfig.DBName, RedisKey.CrawlerConfig.Minutes);
            }
            return configs;
        }


        public List<DBConfig> LoadSources(string city)
        {
            return LoadConfigs(city).GroupBy(c => c.Source).Select(i => i.First()).ToList();
        }

        public Dictionary<string, List<DBConfig>> LoadCitySources(string city="")
        {
            return LoadConfigs(city).GroupBy(c => c.City)
            .ToDictionary(item => item.Key, items => items.GroupBy(c => c.Source).Select(i => i.First()).ToList());
        }

        public List<DBConfig> LoadBySource(string source)
        {
            return _configDapper.LoadBySource(source);
        }

        //待废弃
        public dynamic LoadDashboard()
        {
            var dashboards = _redisTool.ReadCache<dynamic>(RedisKey.CityDashboards.Key,
             RedisKey.CityDashboards.DBName);
            if (dashboards == null || dashboards.Count == 0)
            {
                var id = 1;
                dashboards = LoadConfigs().Select(c => new HouseDashboard()
                {
                    CityName = c.City,
                    HouseSum = 9999,
                    Source = c.Source
                }).GroupBy(d => d.CityName)
                .Select(i => new
                {
                    id = id++,
                    cityName = i.Key,
                    sources = i.GroupBy(d => d.Source).Select(item => item.FirstOrDefault()).ToList()
                });
                _redisTool.WriteObject(RedisKey.CityDashboards.Key, dashboards, RedisKey.CityDashboards.DBName);
            }
            return dashboards;

        }

        public void BulkInsert(List<DBConfig> configs)
        {
            _configDapper.BulkInsert(configs);
        }


    }
}
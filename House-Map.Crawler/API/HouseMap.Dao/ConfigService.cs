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
        private RedisTool _redisTool;

        private HouseDataContext _dataContext;

        public ConfigService(RedisTool RedisTool, HouseDataContext dataContext)
        {
            this._redisTool = RedisTool;
            _dataContext = dataContext;
        }

        public List<DbConfig> LoadConfigs(string city = "")
        {
            var configs = _redisTool.ReadCache<List<DbConfig>>(RedisKey.CrawlerConfig.Key + city, RedisKey.CrawlerConfig.DBName);
            if (configs == null)
            {
                var configQuery = _dataContext.Configs.AsNoTracking().AsQueryable();
                if (!string.IsNullOrEmpty(city))
                {
                    configQuery = configQuery.Where(c => c.City == city);
                }
                configs = configQuery.OrderByDescending(c => c.Score).ToList();
                _redisTool.WriteObject(RedisKey.CrawlerConfig.Key + city, configs,
                 RedisKey.CrawlerConfig.DBName, RedisKey.CrawlerConfig.Minutes);
            }
            return configs;
        }


        public List<DbConfig> LoadSources(string city)
        {
            return LoadConfigs(city).GroupBy(c => c.Source).Select(i => i.First()).ToList();
        }

        public Dictionary<string, List<DbConfig>> LoadCitySources()
        {
            return LoadConfigs().GroupBy(c => c.City)
            .ToDictionary(item => item.Key, items => items.GroupBy(c => c.Source).Select(i => i.First()).ToList());
        }

        public List<DbConfig> LoadBySource(string source)
        {
            return _dataContext.Configs.AsNoTracking().Where(c => c.Source == source).ToList();
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


    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Common;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;

namespace HouseMap.Dao
{

    public class HouseService
    {

        private readonly RedisTool _redisTool;



        private readonly HouseDapper _newHouseDapper;

        private readonly ElasticService _elasticService;


        private readonly ConfigService _configService;

        public HouseService(RedisTool RedisTool, ConfigService configService,
         HouseDapper newHouseDapper, ElasticService elasticService)
        {
            _redisTool = RedisTool;
            _configService = configService;
            _newHouseDapper = newHouseDapper;
            _elasticService = elasticService;
        }

        private List<DBHouse> HouseQuery(HouseCondition condition)
        {
            if (condition == null || condition.City == null)
            {
                throw new Exception("查询条件不能为null");
            }
            var houses = _redisTool.ReadCache<List<DBHouse>>(condition.RedisKey, RedisKeys.NewHouses.DBName);
            if (houses == null || condition.Refresh)
            {
                houses = !string.IsNullOrEmpty(condition.Keyword) ? _elasticService.Query(condition) : _newHouseDapper.SearchHouses(condition);
                if (houses != null && houses.Any())
                {
                    _redisTool.WriteObject(condition.RedisKey, houses, RedisKeys.NewHouses.DBName);
                }
            }
            return houses;
        }

        public void RefreshSearch(HouseCondition condition)
        {
            condition.Refresh = true;
            Search(condition);
            LogHelper.RunActionTaskNotThrowEx(() =>
            {

            }, "RefreshSearch", condition);

        }


        public IEnumerable<DBHouse> Search(HouseCondition condition)
        {
            if (condition == null)
            {
                return default(List<DBHouse>);
            }
            if (string.IsNullOrEmpty(condition.Source) && string.IsNullOrEmpty(condition.Keyword))
            {
                var houseList = new List<DBHouse>();
                // 获取当前城市的房源配置
                var cityConfigs = _configService.LoadSources(condition.City);
                if (cityConfigs.Count == 0)
                {
                    return houseList;
                }
                // var limitCount = condition.Size / cityConfigs.Count;

                var limitCount = condition.Size > 0 ? condition.Size / cityConfigs.Count : 100;
                foreach (var config in cityConfigs)
                {
                    condition.Source = config.Source;
                    condition.Size = limitCount;
                    houseList.AddRange(HouseQuery(condition));
                }
                return houseList.OrderByDescending(h => h.PubTime);
            }
            else
            {
                if (condition.Size == 0)
                {
                    condition.Size = 600;
                }
                return HouseQuery(condition);
            }
        }


        public DBHouse FindById(string houseId)
        {
            var redisKey = RedisKeys.HouseDetail;
            var house = _redisTool.ReadCache<DBHouse>(redisKey.Key + houseId, redisKey.DBName);
            if (house == null)
            {
                house = _elasticService.QueryById(houseId);
                house = (house == null ? _newHouseDapper.FindById(houseId) : house);
                if (house == null)
                {
                    return null;
                }
                _redisTool.WriteObject(redisKey.Key + houseId, house, redisKey.DBName, (int)redisKey.ExpireTime.TotalMinutes);

            }
            return house;
        }

        public void UpdateHousesLngLat(List<DBHouse> houses)
        {
            LogHelper.RunActionTaskNotThrowEx(() =>
            {
                foreach (var house in houses)
                {
                    _newHouseDapper.UpdateLngLat(house);
                }
            });

        }

        public void RefreshHouseV2()
        {
            Console.WriteLine("开始RefreshHouseV2...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = _configService.LoadCitySources();
            foreach (var item in cityDashboards)
            {
                var condition = new HouseCondition() { City = item.Key, Refresh = true };
                foreach (var dashboard in item.Value)
                {
                    //指定来源,每次拉600条,一般用于地图页
                    for (var page = 0; page <= 3; page++)
                    {
                        condition.Size = 600;
                        condition.Page = page;
                        condition.Source = dashboard.Source;
                        Console.WriteLine($"正在刷新[{condition.RedisKey}]缓存");
                        Search(condition);
                    }

                    // 指定来源,每次拉20条,前30页,一般用于小程序/移动端列表页
                    for (var page = 0; page <= 30; page++)
                    {
                        condition.Size = 20;
                        condition.Source = dashboard.Source;
                        condition.Page = page;
                        Console.WriteLine($"正在刷新[{condition.RedisKey}]缓存");
                        this.Search(condition);
                    }
                }
            }
            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine("RefreshHouseV2结束，花费时间：" + copyTime);
        }


        public void RefreshHouseV3()
        {
            Console.WriteLine("RefreshHouseV3...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = _configService.LoadCitySources();
            foreach (var item in cityDashboards)
            {
                //无指定来源,前600条数据
                var condition = new HouseCondition() { City = item.Key, Refresh = true };
                for (var page = 0; page <= 5; page++)
                {
                    condition.Page = page;
                    Console.WriteLine($"正在刷新[{condition.RedisKey}]缓存");
                    Search(condition);
                }

                //无指定来源,每次拉180条,一共10页,一般用于移动端地图
                for (var page = 0; page <= 10; page++)
                {
                    condition.Source = "";
                    condition.Size = 180;
                    condition.Page = page;
                    Console.WriteLine($"正在刷新[{condition.RedisKey}]缓存");
                    this.Search(condition);
                }

                //无指定来源,每次拉20条,一共30页,一般用于小程序或者移动端列表
                for (var page = 0; page <= 30; page++)
                {
                    condition.Source = "";
                    condition.Size = 20;
                    condition.Page = page;
                    Console.WriteLine($"正在刷新[{condition.RedisKey}]缓存");
                    this.Search(condition);
                }
            }
            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            Console.WriteLine("RefreshHouseV2结束，花费时间：" + copyTime);
        }

    }

}
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

        private RedisTool _redisTool;



        private NewHouseDapper _newHouseDapper;


        private ConfigService _configService;

        public HouseService(RedisTool RedisTool, ConfigService configService, NewHouseDapper newHouseDapper)
        {
            _redisTool = RedisTool;
            _configService = configService;
            _newHouseDapper = newHouseDapper;
        }

        private List<DBHouse> NewDBSearch(NewHouseCondition condition)
        {
            // LogHelper.Info($"Search start,key:{condition.RedisKey}");
            if (condition == null || condition.City == null)
            {
                throw new Exception("查询条件不能为null");
            }
            var houses = _redisTool.ReadCache<List<DBHouse>>(condition.RedisKey, RedisKey.NewHouses.DBName);
            if (houses == null || houses.Count == 0 || condition.Refresh)
            {
                houses = _newHouseDapper.SearchHouses(condition);
                if (houses != null && houses.Count > 0)
                {
                    _redisTool.WriteObject(condition.RedisKey, houses, RedisKey.NewHouses.DBName);
                }
            }
            return houses;
        }


        public IEnumerable<DBHouse> NewSearch(NewHouseCondition condition)
        {
            if (string.IsNullOrEmpty(condition.Source))
            {
                var houseList = new List<DBHouse>();
                // 获取当前城市的房源配置
                var cityConfigs = _configService.LoadSources(condition.City);
                if (cityConfigs.Count == 0)
                {
                    return houseList;
                }
                var limitCount = condition.Size / cityConfigs.Count;
                foreach (var config in cityConfigs)
                {
                    //建荣家园数据质量比较差,默认不出
                    if (config.Source == SourceEnum.CCBHouse.GetSourceName())
                    {
                        continue;
                    }
                    condition.Source = config.Source;
                    condition.Size = limitCount;
                    houseList.AddRange(NewDBSearch(condition));
                }
                return houseList.OrderByDescending(h => h.PubTime);
            }
            else
            {
                return NewDBSearch(condition);
            }
        }


        public DBHouse FindById(string houseId)
        {
            var redisKey = RedisKey.HouseDetail;
            var house = _redisTool.ReadCache<DBHouse>(redisKey.Key + houseId, redisKey.DBName);
            if (house == null)
            {
                house = _newHouseDapper.FindById(houseId);
                if (house == null)
                {
                    return null;
                }
                _redisTool.WriteObject(redisKey.Key + houseId, house, redisKey.DBName, (int)redisKey.ExpireTime.TotalMinutes);

            }
            return house;
        }

        public void RefreshHouseV2()
        {
            LogHelper.Info("开始RefreshHouseV2...");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var cityDashboards = _configService.LoadCitySources();
            foreach (var item in cityDashboards)
            {
                //聚合房源的缓存,前600条数据
                var search = new NewHouseCondition() { City = item.Key, Size = 600, IntervalDay = 14, Refresh = true };
                NewSearch(search);
                foreach (var dashboard in item.Value)
                {
                    //每类房源的默认缓存,前600条数据
                    search.Size = 600;
                    search.Source = dashboard.Source;
                    NewSearch(search);

                    // 为小程序做的缓存,每次拉10条,一共20页
                    for (var page = 0; page <= 30; page++)
                    {
                        search.Size = 20;
                        search.Source = dashboard.Source;
                        search.Page = page;
                        this.NewSearch(search);
                    }
                }
                //为移动端做的缓存,每次拉180条,一共10页
                for (var page = 0; page <= 10; page++)
                {
                    search.Source = "";
                    search.Size = 180;
                    search.Page = page;
                    this.NewSearch(search);
                }

                //为小程序做的缓存,每次拉20条,一共30页
                for (var page = 0; page <= 30; page++)
                {
                    search.Source = "";
                    search.Size = 20;
                    search.Page = page;
                    this.NewSearch(search);
                }
            }

            sw.Stop();
            string copyTime = sw.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            LogHelper.Info("RefreshHouseV2结束，花费时间：" + copyTime);
        }



    }

}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Common;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace HouseMap.Dao
{

    public class HouseService
    {

        private RedisTool _redisTool;

        private HouseDapper _houseDapper;

        private NewHouseDapper _newHouseDapper;


        private ConfigService _configService;

        public HouseService(RedisTool RedisTool, HouseDapper houseDapper, ConfigService configService, NewHouseDapper newHouseDapper)
        {
            _redisTool = RedisTool;
            _houseDapper = houseDapper;
            _configService = configService;
            _newHouseDapper= newHouseDapper;
        }

        private IEnumerable<HouseInfo> DBSearch(HouseCondition condition)
        {
            // LogHelper.Info($"Search start,key:{condition.RedisKey}");
            if (condition == null || condition.CityName == null)
            {
                throw new Exception("查询条件不能为null");
            }
            var houses = _redisTool.ReadCache<List<HouseInfo>>(condition.RedisKey, RedisKey.Houses.DBName);
            if (houses == null || houses.Count == 0 || condition.Refresh)
            {
                houses = _houseDapper.SearchHouses(condition).ToList();
                if (houses != null && houses.Count > 0)
                {
                    _redisTool.WriteObject(condition.RedisKey, houses, RedisKey.Houses.DBName);
                }
            }
            return houses;
        }


        public IEnumerable<HouseInfo> Search(HouseCondition condition)
        {
            if (string.IsNullOrEmpty(condition.Source))
            {
                var houseList = new List<HouseInfo>();
                // 获取当前城市的房源配置
                var cityConfigs = _configService.LoadSources(condition.CityName);
                var limitCount = condition.HouseCount / cityConfigs.Count;
                foreach (var config in cityConfigs)
                {
                    //建荣家园数据质量比较差,默认不出
                    if (config.Source == ConstConfigName.CCBHouse)
                    {
                        continue;
                    }
                    condition.Source = config.Source;
                    condition.HouseCount = limitCount;
                    houseList.AddRange(DBSearch(condition));
                }
                return houseList.OrderByDescending(h => h.PubTime);
            }
            else
            {
                return DBSearch(condition);
            }
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
                var limitCount = condition.Size / cityConfigs.Count;
                foreach (var config in cityConfigs)
                {
                    //建荣家园数据质量比较差,默认不出
                    if (config.Source == ConstConfigName.CCBHouse)
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


    
    }

}
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

        private HouseDapper houseDapper;

        public HouseService(RedisTool RedisTool, HouseDapper houseDapper)
        {
            this._redisTool = RedisTool;
            this.houseDapper = houseDapper;
        }

        public IEnumerable<HouseInfo> Search(HouseCondition condition)
        {
            LogHelper.Info($"Search start,key:{condition.RedisKey}");
            if (condition == null || condition.CityName == null)
            {
                throw new Exception("查询条件不能为null");
            }
            var houses = _redisTool.ReadCache<List<HouseInfo>>(condition.RedisKey, RedisKey.Houses.DBName);
            if (houses == null || houses.Count == 0 || condition.Refresh)
            {
                houses = houseDapper.SearchHouses(condition).ToList();
                if (houses != null && houses.Count > 0)
                {
                    _redisTool.WriteObject(condition.RedisKey, houses, RedisKey.Houses.DBName);
                }
            }
            return houses;
        }

    }

}
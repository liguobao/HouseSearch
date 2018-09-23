using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMapAPI.Common;
using HouseMapAPI.CommonException;
using HouseMapAPI.Dapper;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;

namespace HouseMapAPI.Service
{

    public class HouseService
    {

        private RedisService redisService;

        private HouseDapper houseDapper;

        public HouseService(RedisService redisService, HouseDapper houseDapper)
        {
            this.redisService = redisService;
            this.houseDapper = houseDapper;
        }

        public IEnumerable<HouseInfo> Search(HouseCondition condition)
        {
            if (condition == null || condition.CityName == null)
            {
                throw new Exception("查询条件不能为null");
            }
            return houseDapper.SearchHouses(condition);
        }

    }

}
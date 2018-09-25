using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMap.Common;
using HouseMapAPI.CommonException;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;
using HouseMap.Common;

namespace HouseMapAPI.Service
{

    public class HouseService
    {

        private RedisTool RedisTool;

        private HouseDapper houseDapper;

        public HouseService(RedisTool RedisTool, HouseDapper houseDapper)
        {
            this.RedisTool = RedisTool;
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
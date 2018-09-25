using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using HouseMapAPI.CommonException;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Models;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Talk.OAuthClient;

namespace HouseMapAPI.Service
{

    public class ConfigService
    {
        private ConfigDapper _dapper;



        public ConfigService(ConfigDapper dapper)
        {
            _dapper = dapper;

        }

        public void AddDoubanConfig(string groupId, string cityName)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(cityName))
            {
                throw new Exception("请输入豆瓣小组Group和城市名称。");
            }
            var topics = DoubanService.GetHouseData(groupId, cityName, 1);
            if (topics == null)
            {
                throw new Exception("保存失败!请检查豆瓣小组ID（如：XMhouse）/城市名称（如：厦门）是否正确...");
            }
            var cityInfo = $"{{ 'groupid':'{groupId}','cityname':'{cityName}','pagecount':5}}";
            var doubanConfig = new CrawlerConfig();
            if (doubanConfig != null)
            {
                return;
            }
            var config = new CrawlerConfig()
            {
                ConfigurationKey = 0,
                ConfigurationValue = cityInfo,
                ConfigurationName = ConstConfigName.Douban,
                DataCreateTime = DateTime.Now,
                IsEnabled = true,
            };
            _dapper.Insert(config);
            return;
        }



    }

}